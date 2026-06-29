<?php

declare(strict_types=1);

namespace WorkBC\InnovibeJobIndexer\Elastic;

use GuzzleHttp\Client;
use GuzzleHttp\Exception\GuzzleException;
use Monolog\Logger;
use WorkBC\InnovibeJobIndexer\Config\AppConfig;

/**
 * Thin Guzzle wrapper around the Elasticsearch HTTP API.
 *
 * Ports the ES-touching bits of the legacy C#:
 *   - ElasticRequestService     (PUT / DELETE / POST with basic auth, proxy bypass)
 *   - CreateIndexService        (read jobs_index.json, substitute ##SYNONYM##)
 *   - DeleteIndexService        (DELETE index)
 *   - ReIndexingService         (_close / _open to reload synonyms)
 *   - IndexCheckerService       (scroll query to list indexed federal JobIds)
 *
 * Document type is "_doc" (General.ElasticDocType). The ES host is always
 * addressed directly (no forward proxy), matching the C# WebProxy
 * BypassProxyOnLocal behaviour.
 */
final class ElasticClient
{
    private const DOC_TYPE = '_doc';
    private const SCROLL_TTL = '1m';
    private const SCROLL_PAGE = 5000;

    private Client $http;
    private string $server;
    private string $resourceDir;

    public function __construct(
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {
        $this->server = $config->elasticUrl;
        $this->resourceDir = dirname(__DIR__, 2) . '/resources';

        $options = [
            'base_uri' => $this->server . '/',
            'timeout' => $config->httpTimeout,
            'http_errors' => false,
            // Never route Elasticsearch traffic through the forward proxy.
            'proxy' => ['no' => ['*']],
        ];
        if ($config->elasticUser !== '') {
            $options['auth'] = [$config->elasticUser, $config->elasticPassword];
        }

        $this->http = new Client($options);
    }

    // ── Document operations ────────────────────────────────────────

    /**
     * PUT a single document. Returns the lowercase ES result string
     * ("created" / "updated") on success, or "error" on any failure.
     */
    public function putDocument(string $index, string $id, string $json): string
    {
        try {
            $response = $this->http->put($this->docPath($index, $id), [
                'headers' => ['Content-Type' => 'application/json'],
                'body' => $json,
            ]);
        } catch (GuzzleException $e) {
            $this->log->error("ES PUT {$index}/{$id} transport error: {$e->getMessage()}");
            return 'error';
        }

        $status = $response->getStatusCode();
        if ($status < 200 || $status >= 300) {
            $this->log->warning("ES PUT {$index}/{$id} returned HTTP {$status}: "
                . substr((string) $response->getBody(), 0, 500));
            return 'error';
        }

        $body = json_decode((string) $response->getBody(), true);
        $result = is_array($body) && isset($body['result']) ? (string) $body['result'] : 'ok';
        return strtolower($result);
    }

    /**
     * DELETE a single document. A 404 (already gone) is treated as success.
     */
    public function deleteDocument(string $index, string $id): bool
    {
        try {
            $response = $this->http->delete($this->docPath($index, $id));
        } catch (GuzzleException $e) {
            $this->log->error("ES DELETE {$index}/{$id} transport error: {$e->getMessage()}");
            return false;
        }
        $status = $response->getStatusCode();
        return ($status >= 200 && $status < 300) || $status === 404;
    }

    // ── Index maintenance ──────────────────────────────────────────

    /**
     * Recreate an index from resources/jobs_index.json.template, substituting
     * the ##SYNONYM## placeholder with the contents of
     * synonym_file.json.template (production) or
     * synonym_predefined.json.template (unit-test style).
     *
     * These resources carry a ".template" suffix because they are NOT valid
     * JSON on their own — the ##SYNONYM## placeholder (and the bare-entry
     * synonym fragments) only become valid once substituted here. The suffix
     * keeps JSON validators / IDEs from flagging them.
     */
    public function createIndex(string $index, bool $useSynonymFile = true): bool
    {
        $structure = $this->readResource('jobs_index.json.template');
        $synonyms = $useSynonymFile
            ? $this->readResource('synonym_file.json.template')
            : $this->readResource('synonym_predefined.json.template');
        $structure = str_replace('##SYNONYM##', $synonyms, $structure);

        try {
            $response = $this->http->put($index, [
                'headers' => ['Content-Type' => 'application/json'],
                'body' => $structure,
            ]);
        } catch (GuzzleException $e) {
            $this->log->error("ES create index {$index} transport error: {$e->getMessage()}");
            return false;
        }

        $status = $response->getStatusCode();
        if ($status < 200 || $status >= 300) {
            $this->log->error("Failed to create index {$this->server}/{$index} (HTTP {$status}): "
                . substr((string) $response->getBody(), 0, 500));
            return false;
        }
        return true;
    }

    /** DELETE an index. A 404 (doesn't exist) is treated as success. */
    public function deleteIndex(string $index): bool
    {
        try {
            $response = $this->http->delete($index);
        } catch (GuzzleException $e) {
            $this->log->warning("ES delete index {$index} transport error: {$e->getMessage()}");
            return false;
        }
        $status = $response->getStatusCode();
        if ($status === 404) {
            $this->log->info("Index {$index} was not removed (it probably doesn't exist)");
            return true;
        }
        return $status >= 200 && $status < 300;
    }

    /** Close then re-open an index to trigger a synonym-file reload. */
    public function closeAndReopen(string $index): void
    {
        foreach (['_close', '_open'] as $action) {
            try {
                $this->http->post("{$index}/{$action}");
            } catch (GuzzleException $e) {
                $this->log->warning("ES {$action} on {$index} failed: {$e->getMessage()}");
            }
        }
    }

    // ── Read path (purge support) ──────────────────────────────────

    /**
     * Scroll the whole index and return every JobId for the given source
     * (IsFederalJob = $isFederal). Mirrors the IndexCheckerService
     * GetIndexed / GetActive helpers (the Wanted indexer uses $isFederal=false).
     *
     * @return list<string>
     */
    public function getJobIdsBySource(string $index, bool $isFederal, bool $excludeExpired = false): array
    {
        $query = $excludeExpired
            ? [
                'bool' => [
                    'must' => [['term' => ['IsFederalJob' => $isFederal]]],
                    'filter' => [
                        'range' => [
                            'ExpireDate' => ['gte' => 'now', 'time_zone' => 'America/Vancouver'],
                        ],
                    ],
                ],
            ]
            : ['term' => ['IsFederalJob' => $isFederal]];

        $body = json_encode([
            'size' => self::SCROLL_PAGE,
            '_source' => ['JobId'],
            'query' => $query,
        ]);

        $ids = [];
        try {
            $response = $this->http->post("{$index}/_search?scroll=" . self::SCROLL_TTL, [
                'headers' => ['Content-Type' => 'application/json'],
                'body' => $body,
            ]);
            $decoded = json_decode((string) $response->getBody(), true);
            $scrollId = $decoded['_scroll_id'] ?? null;
            $hits = $decoded['hits']['hits'] ?? [];

            while (!empty($hits)) {
                foreach ($hits as $hit) {
                    if (isset($hit['_id'])) {
                        $ids[] = (string) $hit['_id'];
                    }
                }

                if ($scrollId === null) {
                    break;
                }

                $scrollResponse = $this->http->post('_search/scroll', [
                    'headers' => ['Content-Type' => 'application/json'],
                    'body' => json_encode(['scroll' => self::SCROLL_TTL, 'scroll_id' => $scrollId]),
                ]);
                $decoded = json_decode((string) $scrollResponse->getBody(), true);
                $scrollId = $decoded['_scroll_id'] ?? $scrollId;
                $hits = $decoded['hits']['hits'] ?? [];
            }

            if ($scrollId !== null) {
                $this->clearScroll($scrollId);
            }
        } catch (GuzzleException $e) {
            $this->log->error("getFederalJobIds({$index}) failed: {$e->getMessage()}");
        }

        return $ids;
    }

    private function clearScroll(string $scrollId): void
    {
        try {
            $this->http->delete('_search/scroll', [
                'headers' => ['Content-Type' => 'application/json'],
                'body' => json_encode(['scroll_id' => [$scrollId]]),
            ]);
        } catch (GuzzleException) {
            // Best-effort cleanup; the scroll context expires on its own.
        }
    }

    // ── helpers ────────────────────────────────────────────────────

    private function docPath(string $index, string $id): string
    {
        return $index . '/' . self::DOC_TYPE . '/' . rawurlencode($id);
    }

    private function readResource(string $name): string
    {
        $path = $this->resourceDir . '/' . $name;
        $contents = file_get_contents($path);
        if ($contents === false) {
            throw new \RuntimeException("Unable to read resource file: {$path}");
        }
        return $contents;
    }
}
