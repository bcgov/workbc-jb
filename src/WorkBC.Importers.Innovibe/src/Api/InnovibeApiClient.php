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
     * Used for both job import (postedFrom) and expired-jobs API calls.
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
                // 'includeCompanyIds' => ['id1','id2'], // include specific company IDs (array)
                // 'excludeCompanyIds' => ['id1','id2'], // exclude specific company IDs (array)
            ];

            // Date filtering: by default only fetch jobs from yesterday (BC time)
            // Set BULK_IMPORT=true to fetch all jobs without date filtering
            if (!$this->config->bulkImport) {
                $query['postedFrom'] = $this->getBcYesterdayDate();
            // $query['postedTo']   = date('Y-m-d');
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
     * Fetches the list of expired job IDs from the Innovibe API.
     * Calls GET /jobs/expired/ids?date=YYYY-MM-DD
     *
     * @return string[] Array of expired job ID strings
     */
    public function fetchExpiredJobIds(string $date): array
    {
        $query = ['date' => $date];

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

        $ids = $body['data'] ?? $body;
        if (!is_array($ids)) {
            $this->log->warning('Unexpected expired-jobs response format');
            return [];
        }

        // Flatten in case the API returns objects instead of plain IDs
        $result = array_map(fn($item) => is_array($item) ? (string)($item['id'] ?? $item['jobId'] ?? '') : (string)$item, $ids);
        $result = array_filter($result, fn($id) => $id !== '');

        $this->log->info(count($result) . ' expired job IDs returned by API');
        return array_values($result);
    }
}