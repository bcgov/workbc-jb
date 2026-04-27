<?php

declare(strict_types=1);

namespace WorkBC\FederalJobImporter\Api;

use DOMDocument;
use DOMElement;
use DOMXPath;
use GuzzleHttp\Client;
use GuzzleHttp\Exception\GuzzleException;
use GuzzleHttp\HandlerStack;
use GuzzleHttp\Pool;
use GuzzleHttp\Psr7\Request;
use Monolog\Logger;
use Psr\Http\Message\ResponseInterface;
use WorkBC\FederalJobImporter\Config\AppConfig;

/**
 * HTTP / XML client for the Government of Canada Job Bank feed.
 *
 *  Endpoints used (all under {@see AppConfig::$federalXmlRoot}, default
 *  https://www.jobbank.gc.ca/xmlfeed):
 *
 *      GET /en/bc?includevirtual=true   — list every BC + virtual job
 *      GET /en/{jobId}.xml              — full English posting
 *      GET /fr/{jobId}.xml              — full French posting
 *
 * Feature parity with the legacy C# `XmlImportService`:
 *   - Optional outbound proxy (PROXY_*).
 *   - Optional `IgnoreSslErrors` (matches legacy ProxySettings flag).
 *   - `Cookie` header from FEDERAL_AUTH_COOKIE (the IP-whitelisted feed
 *     requires this cookie value).
 *   - Retry once with a configurable backoff if the first request fails
 *     (the C# importer waits 5s and retries — same default here).
 *   - GZip / Deflate decompression.
 *   - Concurrent EN/FR fetches via Guzzle's Pool — equivalent to
 *     `Task.WhenAll(GetWebResponse(en), GetWebResponse(fr))` in C#.
 */
final class FederalApiClient
{
    private Client $http;
    private AppConfig $cfg;
    private Logger $log;

    public function __construct(AppConfig $cfg, Logger $logger)
    {
        $this->cfg = $cfg;
        $this->log = $logger;

        $headers = [
            'Accept'          => 'application/xml,text/xml;q=0.9,*/*;q=0.8',
            'Accept-Encoding' => 'gzip, deflate',
            'User-Agent'      => 'WorkBC-FederalImporter/2.0 (+https://www.workbc.ca)',
        ];
        if ($cfg->federalAuthCookie !== '') {
            $headers['Cookie'] = $cfg->federalAuthCookie;
        }

        $clientOptions = [
            'base_uri'        => $cfg->federalXmlRoot . '/',
            'timeout'         => $cfg->httpTimeout,
            'connect_timeout' => max(5, (int) ($cfg->httpTimeout / 3)),
            'headers'         => $headers,
            'http_errors'     => true,
            'decode_content'  => true,
            'handler'         => HandlerStack::create(),
        ];

        if ($cfg->proxyUse && $cfg->proxyHost !== '' && $cfg->proxyPort > 0) {
            $clientOptions['proxy'] = [
                'http'  => sprintf('http://%s:%d', $cfg->proxyHost, $cfg->proxyPort),
                'https' => sprintf('http://%s:%d', $cfg->proxyHost, $cfg->proxyPort),
                'no'    => ['localhost', '127.0.0.1'],
            ];
        }

        if ($cfg->proxyIgnoreSslErrors) {
            $clientOptions['verify'] = false;
        }

        $this->http = new Client($clientOptions);
    }

    /**
     * Fetches every job advertised in the configured province (default: BC).
     *
     * @return array<int, array{id:string, fileUpdateDate:string}>
     */
    public function fetchJobIndex(): array
    {
        $path = ltrim($this->cfg->federalProvincePath, '/');
        $url = $path . ($this->cfg->federalIncludeVirtual ? '?includevirtual=true' : '');
        $fullUrl = $this->cfg->federalXmlRoot . '/' . $url;
        $this->log->info("GET {$fullUrl}");

        $body = $this->getWithRetry($url);
        if ($body === null) {
            $this->log->error('Failed to load federal job index');
            return [];
        }

        $doc = $this->loadXml($body);
        if ($doc === null) {
            return [];
        }

        $xpath = new DOMXPath($doc);
        $found = (int) ($xpath->evaluate('string(/SolrResponse/Header/numFound)') ?: '0');
        if ($found <= 0) {
            $this->log->info('Federal feed returned 0 jobs');
            return [];
        }

        $jobs = [];
        foreach ($xpath->query('/SolrResponse/Documents/Document') ?: [] as $node) {
            if (!$node instanceof DOMElement) {
                continue;
            }
            $id = $this->childText($node, 'jobs_id');
            $updated = $this->childText($node, 'file_update_date');
            if ($id === '' || $updated === '') {
                continue;
            }
            $jobs[] = [
                'id'             => $id,
                'fileUpdateDate' => $updated,
            ];
        }

        $this->log->info(count($jobs) . ' federal jobs found in BC or virtual');
        return $jobs;
    }

    /**
     * Fetches the EN + FR XML for a single job in parallel.
     *
     * @return array{english:?string, french:?string}
     */
    public function fetchJobXml(string $jobId): array
    {
        $jobId = trim($jobId);
        if ($jobId === '') {
            return ['english' => null, 'french' => null];
        }

        $requests = [
            'english' => new Request('GET', 'en/' . rawurlencode($jobId) . '.xml'),
            'french'  => new Request('GET', 'fr/' . rawurlencode($jobId) . '.xml'),
        ];

        $results = ['english' => null, 'french' => null];

        $pool = new Pool($this->http, $requests, [
            'concurrency' => 2,
            'fulfilled' => function (ResponseInterface $resp, string $key) use (&$results): void {
                $results[$key] = (string) $resp->getBody();
            },
            'rejected' => function (\Throwable $reason, string $key) use ($jobId): void {
                $this->log->warning("Failed to fetch {$key} XML for job {$jobId}: " . $reason->getMessage());
            },
        ]);
        $pool->promise()->wait();

        // Retry whichever leg failed (mirrors C# 5-second wait + single retry)
        foreach ($results as $key => $value) {
            if ($value !== null) {
                continue;
            }
            if ($this->cfg->retryDelayMs > 0) {
                usleep($this->cfg->retryDelayMs * 1000);
            }
            $lang = $key === 'english' ? 'en' : 'fr';
            $body = $this->getWithRetry($lang . '/' . rawurlencode($jobId) . '.xml', false);
            $results[$key] = $body;
        }

        // Validate XML — a 502 / HTML error page must not be persisted.
        if ($results['english'] !== null && !$this->isWellFormedJobXml($results['english'])) {
            $results['english'] = null;
        }
        if ($results['french'] !== null && !$this->isWellFormedJobXml($results['french'])) {
            $results['french'] = null;
        }

        return $results;
    }

    /**
     * Reads `display_until` from a single job-posting XML body (used by the
     * importer to decide whether to bother persisting the row).
     */
    public function extractDisplayUntil(string $xml): ?string
    {
        $doc = $this->loadXml($xml);
        if ($doc === null) {
            return null;
        }
        $xpath = new DOMXPath($doc);
        $found = (int) ($xpath->evaluate('string(/SolrResponse/Header/numFound)') ?: '0');
        if ($found !== 1) {
            return null;
        }
        $value = trim((string) ($xpath->evaluate('string(/SolrResponse/Documents/Document/display_until)') ?: ''));
        return $value === '' ? null : $value;
    }

    private function getWithRetry(string $relativeUrl, bool $logBody = true): ?string
    {
        try {
            $response = $this->http->get($relativeUrl);
            return (string) $response->getBody();
        } catch (GuzzleException $first) {
            if ($this->cfg->retryDelayMs > 0) {
                $this->log->info('Retrying after ' . $this->cfg->retryDelayMs . 'ms: ' . $first->getMessage());
                usleep($this->cfg->retryDelayMs * 1000);
            }
            try {
                $response = $this->http->get($relativeUrl);
                return (string) $response->getBody();
            } catch (GuzzleException $second) {
                if ($logBody) {
                    $this->log->error("HTTP failure for {$relativeUrl}: " . $second->getMessage());
                }
                return null;
            }
        }
    }

    private function loadXml(string $body): ?DOMDocument
    {
        if ($body === '') {
            return null;
        }
        $previous = libxml_use_internal_errors(true);
        $doc = new DOMDocument();
        $doc->preserveWhiteSpace = false;
        $loaded = $doc->loadXML($body, LIBXML_NONET | LIBXML_NOERROR | LIBXML_NOWARNING);
        libxml_clear_errors();
        libxml_use_internal_errors($previous);
        return $loaded ? $doc : null;
    }

    private function isWellFormedJobXml(string $body): bool
    {
        $doc = $this->loadXml($body);
        if ($doc === null) {
            return false;
        }
        $xpath = new DOMXPath($doc);
        return $xpath->query('/SolrResponse/Documents/Document')->length > 0;
    }

    private function childText(DOMElement $parent, string $name): string
    {
        $children = $parent->getElementsByTagName($name);
        if ($children->length === 0) {
            return '';
        }
        return trim((string) $children->item(0)?->textContent);
    }
}
