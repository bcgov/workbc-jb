<?php

declare(strict_types=1);

require_once __DIR__ . '/../vendor/autoload.php';

use Monolog\Formatter\LineFormatter;
use Monolog\Handler\StreamHandler;
use Monolog\Level;
use Monolog\Logger;
use WorkBC\FederalJobImporter\Api\FederalApiClient;
use WorkBC\FederalJobImporter\Config\AppConfig;
use WorkBC\FederalJobImporter\Service\JobImportService;

if (file_exists(__DIR__ . '/../.env')) {
    Dotenv\Dotenv::createImmutable(__DIR__ . '/..')->safeLoad();
}

$config = new AppConfig();

$argv = $argv ?? [];
foreach ($argv as $i => $arg) {
    if ($i === 0) {
        continue;
    }
    if ($arg === '--reimport' || $arg === '-r') {
        $config->reImport = true;
        continue;
    }
    if ($arg === '--maxjobs' && isset($argv[$i + 1])) {
        $config->effectiveMaxJobs = max(1, (int) $argv[$i + 1]);
        continue;
    }
    if (str_starts_with($arg, '--maxjobs=')) {
        $config->effectiveMaxJobs = max(1, (int) substr($arg, strlen('--maxjobs=')));
    }
}

$logger = new Logger('federal-importer');
$handler = new StreamHandler('php://stdout', Level::fromName($config->logLevel));
$handler->setFormatter(new LineFormatter("[%datetime%] %level_name%  %message%\n", 'H:i:s'));
$logger->pushHandler($handler);

if ($config->federalAuthCookie === '') {
    $logger->warning('FEDERAL_AUTH_COOKIE is not set — jobbank.gc.ca will likely reject requests from non-whitelisted hosts.');
}

try {
    $pdo = new PDO($config->dbDsn, $config->dbUser, $config->dbPassword, [
        PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
        PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    ]);
} catch (Throwable $e) {
    $logger->critical('Failed to connect to PostgreSQL: ' . $e->getMessage());
    exit(1);
}

$api = new FederalApiClient($config, $logger);
$service = new JobImportService($pdo, $api, $config, $logger);

try {
    $service->run();
    exit(0);
} catch (Throwable $e) {
    $logger->error('IMPORTER FAILED: ' . $e->getMessage());
    $logger->error($e->getTraceAsString());
    exit(2);
}
