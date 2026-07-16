<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Elastic;

use GuzzleHttp\Client;
use GuzzleHttp\Exception\GuzzleException;
use WorkBC\JobAlertNotifier\Config\AppConfig;

/**
 * Read-only Elasticsearch helper — port of the QueryElasticSearch() path of
 * WorkBC.Shared.Utilities.ElasticHttpHelper as used by JobSearchQuery.
 *
 * The search URL keeps the legacy "{index}/_doc/_search" shape
 * (General.ElasticDocType) that the C# used against ES7/OpenSearch.
 * ES traffic never goes through the forward proxy (BypassProxyOnLocal).
 */
final class ElasticSearchClient
{
    private const DOC_TYPE = '_doc';

    private Client $http;

    public function __construct(private readonly AppConfig $config)
    {
        $options = [
            'base_uri' => $config->elasticUrl . '/',
            'timeout' => $config->httpTimeout,
            'http_errors' => false,
        ];
        if ($config->elasticUser !== '') {
            $options['auth'] = [$config->elasticUser, $config->elasticPassword];
        }
        $this->http = new Client($options);
    }

    /**
     * POST a search body and return the decoded response.
     *
     * @return array<string, mixed>
     */
    public function search(string $index, string $json): array
    {
        $url = $index . '/' . self::DOC_TYPE . '/_search';

        try {
            $response = $this->http->post($url, [
                'headers' => ['Content-Type' => 'application/json'],
                'body' => $json,
            ]);
        } catch (GuzzleException $e) {
            throw new \RuntimeException("Elasticsearch request to {$url} failed: {$e->getMessage()}", 0, $e);
        }

        $status = $response->getStatusCode();

        if ($status === 401) {
            $maskedPassword = preg_replace('/\S/', '*', $this->config->elasticPassword);
            throw new \RuntimeException(
                "Elasticsearch returned an Unauthorized status code\n"
                . "url={$url}\nuser={$this->config->elasticUser}\npwd={$maskedPassword}"
            );
        }

        if ($status < 200 || $status >= 300) {
            throw new \RuntimeException(
                "Elasticsearch returned a {$status} status code\n"
                . "url={$url}\nresponse=" . (string) $response->getBody()
            );
        }

        $decoded = json_decode((string) $response->getBody(), true);
        if (!is_array($decoded)) {
            throw new \RuntimeException("Elasticsearch returned a non-JSON body for {$url}");
        }

        return $decoded;
    }
}
