<?php

declare(strict_types=1);

require_once __DIR__ . '/../vendor/autoload.php';

use Monolog\Formatter\LineFormatter;
use Monolog\Handler\StreamHandler;
use Monolog\Level;
use Monolog\Logger;
use WorkBC\JobAlertNotifier\Config\AppConfig;
use WorkBC\JobAlertNotifier\Elastic\ElasticSearchClient;
use WorkBC\JobAlertNotifier\Email\SendGridEmailSender;
use WorkBC\JobAlertNotifier\Email\SesEmailSender;
use WorkBC\JobAlertNotifier\Email\SmtpEmailSender;
use WorkBC\JobAlertNotifier\Search\GeocodingService;
use WorkBC\JobAlertNotifier\Service\JobAlertSearchService;
use WorkBC\JobAlertNotifier\Service\JobAlertSenderService;

if (file_exists(__DIR__ . '/../.env')) {
    Dotenv\Dotenv::createImmutable(__DIR__ . '/..')->safeLoad();
}

$config = new AppConfig();

$logger = new Logger('jobalert-notifier');
$handler = new StreamHandler('php://stdout', Level::fromName($config->logLevel));
$handler->setFormatter(new LineFormatter("[%datetime%] %level_name%  %message%\n", 'H:i:s'));
$logger->pushHandler($handler);

$logger->info('JOB ALERT TASK STARTED');

// Mirrors Program.cs: only run when IsProduction is true or a testing
// override address is configured — otherwise sending is skipped entirely.
if ($config->isProduction || trim($config->sendEmailTestingTo) !== '') {
    try {
        $pdo = new PDO($config->dbDsn, $config->dbUser, $config->dbPassword, [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        ]);

        if ($config->emailUseSes) {
            $emailSender = new SesEmailSender($config, $logger);
        } elseif ($config->emailUseSmtp) {
            $emailSender = new SmtpEmailSender($config, $logger);
        } else {
            $emailSender = new SendGridEmailSender($config, $logger);
        }

        $elastic = new ElasticSearchClient($config);
        $geocoding = new GeocodingService($pdo, $config, $logger);
        $searchService = new JobAlertSearchService($elastic, $geocoding, $config->indexEn);

        $service = new JobAlertSenderService($pdo, $config, $logger, $emailSender, $searchService);
        $service->runJobAlertSender();
    } catch (Throwable $e) {
        $logger->error("Failed sending notifications:\n{$e}");
    }
} else {
    $logger->info('No emails were sent. SEND_EMAIL_TESTING_TO is blank and IS_PRODUCTION is false.');
}

$logger->info('JOB ALERT TASK COMPLETED');
