<?php

declare(strict_types=1);

namespace WorkBC\JobImporter\Config;

final class AppConfig
{
    public readonly string $dbDsn;
    public readonly string $dbUser;
    public readonly string $dbPassword;
    public readonly string $apiBaseUrl;
    public readonly string $apiKey;
    public readonly int $pageSize;
    public readonly string $logLevel;
    public readonly bool $includeNocUnmatched;
    public bool $bulkImport = false;

    public function __construct()
    {
        $host = $this->env('DB_HOST', 'postgres');
        $port = $this->env('DB_PORT', '5432');
        $name = $this->env('DB_NAME', 'jobboard');

        $this->dbDsn = "pgsql:host={$host};port={$port};dbname={$name}";
        $this->dbUser = $this->env('DB_USER', 'workbc');
        $this->dbPassword = $this->env('DB_PASSWORD', 'workbc');
        $this->apiBaseUrl = rtrim($this->env('API_BASE_URL', 'https://api-prod.jobs.innovibe.ca/api/v1'), '/');
        $this->apiKey = $this->env('API_KEY', '');
        $this->pageSize = (int) $this->env('PAGE_SIZE', '100');
        $this->includeNocUnmatched = strtolower($this->env('INCLUDE_NOC_UNMATCHED', 'false')) === 'true';
        $this->logLevel = strtoupper($this->env('LOG_LEVEL', 'INFO'));
    }

    private function env(string $key, string $default = ''): string
    {
        return trim((string) ($_ENV[$key] ?? getenv($key) ?: $default));
    }
}