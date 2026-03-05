<?php

declare(strict_types=1);

namespace WorkBC\JobImporter\Api;

use GuzzleHttp\Client;
use Monolog\Logger;
use WorkBC\JobImporter\Config\AppConfig;

final class InnovibeApiClient
{
    private Client $http;
    private Logger $log;
    private AppConfig $config;

    public function __construct(AppConfig $config, Logger $logger)
    {
        $this->log = $logger;
        $this->config = $config;
        $this->http = new Client([
            'base_uri' => $config->apiBaseUrl . '/',
            'timeout' => 120,
            'headers' => [
                'x-api-key' => $config->apiKey,
                'Accept' => 'application/json',
            ],
        ]);
    }

    /**
     * Returns yesterday's date in the America/Vancouver (BC) timezone.
     */
    public function getBcYesterdayDate(): string
    {
        $tz = new \DateTimeZone('America/Vancouver');
        return (new \DateTime('yesterday', $tz))->format('Y-m-d');
    }

    /** @return \Generator<int, array> */
    public function fetchAllJobs(int $pageSize): \Generator
    {
        $cursor = null;
        $page = 0;

        do {
            $page++;
            $query = [
                'limit' => $pageSize,
                'includeExpired' => 'false',
                'includeNocUnmatched' => $this->config->includeNocUnmatched ? 'true' : 'false',
                'state' => 'British Columbia',
                // 'includeNoSalary' => 'false',        // set to 'true' to include jobs without salary
                // 'company'         => '',              // narrow to single company by name
                // 'excludeCompany'  => ['name1'],       // exclude companies by name (array)
                // 'excludeCompanyIds' => ['id1','id2'], // exclude specific company IDs (array)
            ];

            // Date filtering: by default only fetch jobs from yesterday (BC time)
            if (!$this->config->bulkImport) {
                $query['postedFrom'] = $this->getBcYesterdayDate();
            }

            if ($cursor) {
                $query['cursor'] = $cursor;
            }

            $fullUrl = $this->config->apiBaseUrl . '/jobs?' . http_build_query($query);
            $this->log->info("GET {$fullUrl}");

            try {
                $response = $this->http->get('jobs', ['query' => $query]);
                $body = json_decode((string)$response->getBody(), true, 512, JSON_THROW_ON_ERROR);
            }
            catch (\Throwable $e) {
                $this->log->error("API page {$page} failed: {$e->getMessage()}");
                break;
            }

            $jobs = $body['data'] ?? [];
            if (empty($jobs)) {
                break;
            }

            yield from$jobs;

            $cursor = $body['pagination']['nextCursor'] ?? null;
            $this->log->info("Page {$page}: " . count($jobs) . ' jobs');
        } while ($cursor);
    }

    /**
     * Fetches expired job IDs from the Innovibe API.
     * Calls GET /jobs/expired/ids?date=YYYY-MM-DD
     *
     * @return string[] Array of expired job ID strings
     */
    public function fetchExpiredJobIds(string $date): array
    {
        $query = [
            'state' => 'British Columbia',
            'includeNocUnmatched' => $this->config->includeNocUnmatched ? 'true' : 'false',
            'postedFrom' => $date,
            'postedTo' => $date,
        ];
        $fullUrl = $this->config->apiBaseUrl . '/jobs/expired/ids?' . http_build_query($query);
        $this->log->info("GET {$fullUrl}");

        try {
            $response = $this->http->get('jobs/expired/ids', ['query' => $query]);
            $body = json_decode((string)$response->getBody(), true, 512, JSON_THROW_ON_ERROR);
        }
        catch (\Throwable $e) {
            $this->log->error("Expired-jobs API call failed: {$e->getMessage()}");
            return [];
        }

        // Resolve the ID array from whichever key the API uses
        $ids = $this->resolveIdArray($body);

        if (empty($ids)) {
            return [];
        }

        // Normalise to string IDs (handles both plain strings and object formats)
        $result = array_map(
        fn($item) => is_array($item) ? (string)($item['id'] ?? $item['jobId'] ?? '') : (string)$item,
            $ids
        );

        return array_values(array_filter($result, fn($id) => $id !== ''));
    }

    /**
     * Resolves the ID array from the API response body,
     * trying common keys: data, ids, or a plain list.
     */
    private function resolveIdArray(array $body): array
    {
        if (isset($body['data']) && is_array($body['data'])) {
            return $body['data'];
        }
        if (isset($body['ids']) && is_array($body['ids'])) {
            return $body['ids'];
        }
        if (array_is_list($body)) {
            return $body;
        }

        // Fallback: use the first array value found
        foreach ($body as $value) {
            if (is_array($value)) {
                return $value;
            }
        }

        return [];
    }
}