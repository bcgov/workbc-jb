<?php

declare(strict_types=1);

namespace WorkBC\InnovibeJobIndexer\Config;

/**
 * Strongly-typed env-var configuration for the Wanted/Innovibe V2 indexer.
 *
 * Mirrors the legacy C# appsettings sections consumed by
 * WorkBC.Indexers.Wanted / WorkBC.ElasticSearch.Indexing:
 *   - ConnectionStrings:DefaultConnection   → DB_*
 *   - ConnectionStrings:ElasticSearchServer → ELASTIC_URL
 *   - IndexSettings:DefaultIndex            → INDEX_EN (documents are written here)
 *   - IndexSettings:ElasticUser/Password    → ELASTIC_USER / ELASTIC_PASSWORD
 *   - WantedSettings:JobExpiryDays          → WANTED_JOB_EXPIRY_DAYS
 *
 * Unlike the Federal indexer the Wanted path needs no geocoding (lat/lon come
 * straight from the Innovibe JSON), so there is no Google Maps / proxy config.
 * Wanted documents are written to a single index (INDEX_EN); INDEX_FR is only
 * used by the shared --reindex maintenance which recreates both indexes.
 */
final class AppConfig
{
    public readonly string $dbDsn;
    public readonly string $dbUser;
    public readonly string $dbPassword;

    public readonly string $elasticUrl;
    public readonly string $elasticUser;
    public readonly string $elasticPassword;

    public readonly string $indexEn;
    public readonly string $indexFr;

    public readonly int $wantedJobExpiryDays;

    public readonly int $httpTimeout;
    public readonly int $retryDelayMs;

    public readonly string $logLevel;

    public function __construct()
    {
        $host = $this->env('DB_HOST', 'postgres');
        $port = $this->env('DB_PORT', '5432');
        $name = $this->env('DB_NAME', 'jobboard');
        $this->dbDsn = "pgsql:host={$host};port={$port};dbname={$name}";
        $this->dbUser = $this->env('DB_USER', 'workbc');
        $this->dbPassword = $this->env('DB_PASSWORD', 'workbc');

        $this->elasticUrl = rtrim($this->env('ELASTIC_URL', 'http://elasticsearch:9200'), '/');
        $this->elasticUser = $this->env('ELASTIC_USER', '');
        $this->elasticPassword = $this->env('ELASTIC_PASSWORD', '');

        $this->indexEn = $this->env('INDEX_EN', 'jobs_en');
        $this->indexFr = $this->env('INDEX_FR', 'jobs_fr');

        // Matches General.DefaultWantedJobExpiryDays (90). The C# parser adds this
        // many days to the refreshed date to compute ExpireDate.
        $this->wantedJobExpiryDays = max(1, (int) $this->env('WANTED_JOB_EXPIRY_DAYS', '90'));

        $this->httpTimeout = max(5, (int) $this->env('HTTP_TIMEOUT', '30'));
        // Matches the 15s pause the C# PostToElasticSearch() waits on a
        // WebException before retrying.
        $this->retryDelayMs = max(0, (int) $this->env('RETRY_DELAY_MS', '15000'));

        $this->logLevel = strtoupper($this->env('LOG_LEVEL', 'INFO'));
    }

    private function env(string $key, string $default = ''): string
    {
        $value = $_ENV[$key] ?? getenv($key);
        if ($value === false || $value === null || $value === '') {
            return $default;
        }
        return trim((string) $value);
    }
}
