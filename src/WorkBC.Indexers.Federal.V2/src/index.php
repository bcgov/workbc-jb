<?php

declare(strict_types=1);

require_once __DIR__ . '/../vendor/autoload.php';

use Monolog\Formatter\LineFormatter;
use Monolog\Handler\StreamHandler;
use Monolog\Level;
use Monolog\Logger;
use WorkBC\FederalJobIndexer\Config\AppConfig;
use WorkBC\FederalJobIndexer\Elastic\ElasticClient;
use WorkBC\FederalJobIndexer\Geocoding\GeocodingService;
use WorkBC\FederalJobIndexer\Service\FederalIndexService;
use WorkBC\FederalJobIndexer\Service\IndexMaintenanceService;
use WorkBC\FederalJobIndexer\Xml\FederalDocumentMapper;

if (file_exists(__DIR__ . '/../.env')) {
    Dotenv\Dotenv::createImmutable(__DIR__ . '/..')->safeLoad();
}

$config = new AppConfig();

// ── CLI flags (mirror WorkBC.ElasticSearch.Indexing.CommandLineOptions) ──
$reIndex = false;     // -r / --reindex : drop + recreate the indexes, flag all rows
$reOpen = false;      // -o / --reopen  : close + reopen to reload synonyms
$skipReIndex = false; // -n / --noreindex : skip the indexing loop
$debug = false;       // -d / --debug   : print ES-vs-Jobs diff and exit

foreach (array_slice($argv ?? [], 1) as $arg) {
    switch ($arg) {
        case '-r':
        case '--reindex':
            $reIndex = true;
            break;
        case '-o':
        case '--reopen':
            $reOpen = true;
            break;
        case '-n':
        case '--noreindex':
            $skipReIndex = true;
            break;
        case '-d':
        case '--debug':
            $debug = true;
            break;
    }
}

$logger = new Logger('federal-indexer');
$handler = new StreamHandler('php://stdout', Level::fromName($config->logLevel));
$handler->setFormatter(new LineFormatter("[%datetime%] %level_name%  %message%\n", 'H:i:s'));
$logger->pushHandler($handler);

try {
    $pdo = new PDO($config->dbDsn, $config->dbUser, $config->dbPassword, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
        PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    ]);
} catch (Throwable $e) {
    $logger->critical('Failed to connect to PostgreSQL: ' . $e->getMessage());
    exit(1);
}

$elastic = new ElasticClient($config, $logger);
$geocoding = new GeocodingService($pdo, $config, $logger);
$mapper = new FederalDocumentMapper($pdo, $geocoding, $logger);
$maintenance = new IndexMaintenanceService($pdo, $elastic, $config, $logger);
$indexer = new FederalIndexService($pdo, $elastic, $mapper, $config, $logger);

try {
    if ($debug) {
        $indexer->debug();
        exit(0);
    }

    $logger->info('INDEXER STARTED');

    // Maintenance first (recreate or close/reopen), mirroring MainTask().
    if ($reIndex) {
        $logger->info('Recreating the job indexes');
        $maintenance->reCreateIndex();
    }
    if ($reOpen) {
        $logger->info('Closing and reopening the job indexes to update synonyms');
        $maintenance->reCloseAndReOpenIndexes();
    }

    // Index loop runs unless --noreindex was passed.
    if (!$skipReIndex) {
        $logger->info('Indexing jobs - start');
        try {
            $indexer->indexJobs();
        } catch (Throwable $e) {
            $logger->error($e->getMessage());
            $logger->error('ERROR OCCURRED RUNNING MAIN TASK... SKIPPING AHEAD TO THE NEXT TASK');
        }
        $logger->info('Indexing jobs - done.');
    }

    // Purge runs on a normal run, but NOT after a fresh --reindex and NOT when
    // the indexing loop was skipped (matches Program.cs control flow).
    if (!$skipReIndex && !$reIndex) {
        $logger->info('Purging expired jobs from Elasticsearch');
        $indexer->purgeJobs();
    }

    $logger->info('INDEXER FINISHED');
    exit(0);
} catch (Throwable $e) {
    $logger->error('INDEXER FAILED: ' . $e->getMessage());
    $logger->error($e->getTraceAsString());
    exit(2);
}
