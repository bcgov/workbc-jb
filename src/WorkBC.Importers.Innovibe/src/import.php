<?php

declare(strict_types=1);

require_once __DIR__ . '/../vendor/autoload.php';

use Monolog\Logger;
use Monolog\Handler\StreamHandler;
use Monolog\Formatter\LineFormatter;
use Monolog\Level;
use WorkBC\JobImporter\Config\AppConfig;
use WorkBC\JobImporter\Api\InnovibeApiClient;
use WorkBC\JobImporter\Service\JobImportService;

if (file_exists(__DIR__ . '/../.env')) {
    Dotenv\Dotenv::createImmutable(__DIR__ . '/..')->safeLoad();
}

$config = new AppConfig();

// CLI flag: --bulk to import ALL jobs (no date filtering)
if (in_array('--bulk', $argv ?? [], true)) {
    $config->bulkImport = true;
}

$logger = new Logger('job-importer');
$handler = new StreamHandler('php://stdout', Level::fromName($config->logLevel));
$handler->setFormatter(new LineFormatter("[%datetime%] %level_name%  %message%\n", 'H:i:s'));
$logger->pushHandler($handler);

if (!$config->apiKey || $config->apiKey === 'YOUR_KEY') {
    $logger->critical('API_KEY not set.');
    exit(1);
}

$pdo = new PDO($config->dbDsn, $config->dbUser, $config->dbPassword, [
    PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
]);

$service = new JobImportService($pdo, new InnovibeApiClient($config, $logger), $config, $logger);
$service->run();
