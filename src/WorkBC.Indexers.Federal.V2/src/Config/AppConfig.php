<?php

declare(strict_types=1);

namespace WorkBC\FederalJobIndexer\Config;

/**
 * Strongly-typed env-var configuration for the Federal V2 indexer.
 *
 * Mirrors the legacy C# appsettings sections consumed by
 * WorkBC.Indexers.Federal / WorkBC.ElasticSearch.Indexing:
 *   - ConnectionStrings:DefaultConnection   → DB_*
 *   - ConnectionStrings:ElasticSearchServer → ELASTIC_URL
 *   - IndexSettings:ElasticUser/Password    → ELASTIC_USER / ELASTIC_PASSWORD
 *   - AppSettings:GoogleMapsIPApi           → GOOGLE_MAPS_API_KEY
 *   - ProxySettings                         → PROXY_*
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

    public readonly string $googleMapsApiKey;

    public readonly bool $proxyUse;
    public readonly string $proxyHost;
    public readonly int $proxyPort;
    public readonly bool $proxyIgnoreSslErrors;

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

        $this->googleMapsApiKey = $this->env('GOOGLE_MAPS_API_KEY', '');

        $this->proxyUse = $this->boolEnv('PROXY_USE', false);
        $this->proxyHost = $this->env('PROXY_HOST', '');
        $this->proxyPort = (int) $this->env('PROXY_PORT', '0');
        $this->proxyIgnoreSslErrors = $this->boolEnv('PROXY_IGNORE_SSL_ERRORS', true);

        $this->httpTimeout = max(5, (int) $this->env('HTTP_TIMEOUT', '30'));
        // Matches the 15s pause the C# CreateJob() waits on a WebException before retrying.
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

    private function boolEnv(string $key, bool $default): bool
    {
        $raw = $_ENV[$key] ?? getenv($key);
        if ($raw === false || $raw === null || $raw === '') {
            return $default;
        }
        return in_array(strtolower(trim((string) $raw)), ['1', 'true', 'yes', 'on'], true);
    }
}
