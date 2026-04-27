<?php

declare(strict_types=1);

namespace WorkBC\FederalJobImporter\Config;

/**
 * Strongly-typed env-var configuration for the Federal V2 importer.
 *
 * Mirrors the legacy C# `appsettings.json` sections:
 *   - ConnectionStrings:DefaultConnection → DB_*
 *   - FederalSettings                     → FEDERAL_*
 *   - ProxySettings                       → PROXY_*
 *   - WorkBC.Importers.Federal CLI flags  → MAX_JOBS / MAX_ERRORS
 */
final class AppConfig
{
    public readonly string $dbDsn;
    public readonly string $dbUser;
    public readonly string $dbPassword;

    public readonly string $federalXmlRoot;
    public readonly string $federalProvincePath;
    public readonly bool $federalIncludeVirtual;
    public readonly string $federalAuthCookie;

    public readonly bool $proxyUse;
    public readonly string $proxyHost;
    public readonly int $proxyPort;
    public readonly bool $proxyIgnoreSslErrors;

    public readonly int $maxJobs;
    public readonly int $maxErrors;
    public readonly int $httpTimeout;
    public readonly int $retryDelayMs;

    public readonly string $logLevel;

    /** Set by CLI flag `--reimport` / `-r`. Skips per-job XML fetches. */
    public bool $reImport = false;

    /** Resolved from CLI `--maxjobs` (overrides env var if provided). */
    public int $effectiveMaxJobs;

    public function __construct()
    {
        $host = $this->env('DB_HOST', 'postgres');
        $port = $this->env('DB_PORT', '5432');
        $name = $this->env('DB_NAME', 'jobboard');
        $this->dbDsn = "pgsql:host={$host};port={$port};dbname={$name}";
        $this->dbUser = $this->env('DB_USER', 'workbc');
        $this->dbPassword = $this->env('DB_PASSWORD', 'workbc');

        $this->federalXmlRoot = rtrim($this->env('FEDERAL_XML_ROOT', 'https://www.jobbank.gc.ca/xmlfeed'), '/');
        $this->federalProvincePath = '/' . ltrim($this->env('FEDERAL_PROVINCE_PATH', '/en/bc'), '/');
        $this->federalIncludeVirtual = $this->boolEnv('FEDERAL_INCLUDE_VIRTUAL', true);
        $this->federalAuthCookie = $this->env('FEDERAL_AUTH_COOKIE', '');

        $this->proxyUse = $this->boolEnv('PROXY_USE', false);
        $this->proxyHost = $this->env('PROXY_HOST', '');
        $this->proxyPort = (int) $this->env('PROXY_PORT', '0');
        $this->proxyIgnoreSslErrors = $this->boolEnv('PROXY_IGNORE_SSL_ERRORS', true);

        $this->maxJobs = max(1, (int) $this->env('MAX_JOBS', '20000'));
        $this->maxErrors = max(1, (int) $this->env('MAX_ERRORS', '25'));
        $this->httpTimeout = max(5, (int) $this->env('HTTP_TIMEOUT', '30'));
        $this->retryDelayMs = max(0, (int) $this->env('RETRY_DELAY_MS', '5000'));

        $this->effectiveMaxJobs = $this->maxJobs;
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
