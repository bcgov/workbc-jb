<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Config;

/**
 * Strongly-typed env-var configuration for the Job Alert Notifier.
 *
 * Mirrors the legacy C# appsettings sections consumed by
 * WorkBC.Notifications.JobAlerts:
 *   - ConnectionStrings:DefaultConnection    → DB_*
 *   - ConnectionStrings:ElasticSearchServer  → ELASTIC_URL
 *   - IndexSettings:DefaultIndex             → INDEX_EN
 *   - IndexSettings:ElasticUser/Password     → ELASTIC_USER / ELASTIC_PASSWORD
 *   - AppSettings:IsProduction               → IS_PRODUCTION
 *   - AppSettings:SendEmailTestingTo         → SEND_EMAIL_TESTING_TO
 *   - AppSettings:JbSearchUrl                → JB_SEARCH_URL
 *   - AppSettings:GoogleMapsIPApi            → GOOGLE_MAPS_API_KEY
 *   - EmailSettings:*                        → EMAIL_*
 *   - ProxySettings:*                        → PROXY_* (SendGrid + Google Maps only,
 *     never Elasticsearch — matches the C# BypassProxyOnLocal behaviour)
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

    public readonly bool $isProduction;
    public readonly string $sendEmailTestingTo;
    public readonly string $jbSearchUrl;
    public readonly string $googleMapsApiKey;

    public readonly bool $emailUseSes;
    public readonly string $emailSesRegion;
    public readonly bool $emailUseSmtp;
    public readonly string $emailSmtpServer;
    public readonly int $emailSmtpPort;
    public readonly string $emailSendGridKey;
    public readonly string $emailSendGridFromEmail;
    public readonly string $emailFromEmail;
    public readonly string $emailFromName;

    public readonly bool $proxyUse;
    public readonly string $proxyHost;
    public readonly int $proxyPort;
    public readonly bool $proxyIgnoreSslErrors;

    public readonly int $httpTimeout;
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

        $this->isProduction = $this->boolEnv('IS_PRODUCTION', false);
        $this->sendEmailTestingTo = $this->env('SEND_EMAIL_TESTING_TO', '');
        $this->jbSearchUrl = $this->env('JB_SEARCH_URL', '');
        $this->googleMapsApiKey = $this->env('GOOGLE_MAPS_API_KEY', '');

        $this->emailUseSes = $this->boolEnv('EMAIL_USE_SES', false);
        $this->emailSesRegion = $this->env('EMAIL_SES_REGION', 'ca-central-1');
        $this->emailUseSmtp = $this->boolEnv('EMAIL_USE_SMTP', false);
        $this->emailSmtpServer = $this->env('EMAIL_SMTP_SERVER', '');
        $this->emailSmtpPort = (int) $this->env('EMAIL_SMTP_PORT', '25');
        $this->emailSendGridKey = $this->env('EMAIL_SENDGRID_KEY', '');
        $this->emailSendGridFromEmail = $this->env('EMAIL_SENDGRID_FROM_EMAIL', '');
        $this->emailFromEmail = $this->env('EMAIL_FROM_EMAIL', 'noreply@gov.bc.ca');
        $this->emailFromName = $this->env('EMAIL_FROM_NAME', 'WorkBC.ca No Reply');

        $this->proxyUse = $this->boolEnv('PROXY_USE', false);
        $this->proxyHost = $this->env('PROXY_HOST', '');
        $this->proxyPort = (int) $this->env('PROXY_PORT', '80');
        $this->proxyIgnoreSslErrors = $this->boolEnv('PROXY_IGNORE_SSL_ERRORS', false);

        $this->httpTimeout = max(5, (int) $this->env('HTTP_TIMEOUT', '60'));
        $this->logLevel = strtoupper($this->env('LOG_LEVEL', 'INFO'));
    }

    /** Guzzle proxy option for outbound (SendGrid / Google Maps) requests. */
    public function outboundProxy(): array|string|null
    {
        if (!$this->proxyUse || $this->proxyHost === '') {
            return null;
        }
        return "http://{$this->proxyHost}:{$this->proxyPort}";
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
        $value = strtolower($this->env($key, $default ? 'true' : 'false'));
        return in_array($value, ['true', '1', 'yes'], true);
    }
}
