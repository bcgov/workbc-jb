<?php

declare(strict_types=1);

require_once __DIR__ . '/../vendor/autoload.php';

use Monolog\Logger;
use Monolog\Handler\StreamHandler;
use Monolog\Formatter\LineFormatter;
use Monolog\Level;
use WorkBC\JobImporter\Config\AppConfig;
use WorkBC\JobImporter\Api\InnovibeApiClient;

if (file_exists(__DIR__ . '/../.env')) {
    Dotenv\Dotenv::createImmutable(__DIR__ . '/..')->safeLoad();
}

$config = new AppConfig();
$config->bulkImport = true;

$logger = new Logger('csv-report');
$handler = new StreamHandler('php://stderr', Level::fromName($config->logLevel));
$handler->setFormatter(new LineFormatter("[%datetime%] %level_name%  %message%\n", 'H:i:s'));
$logger->pushHandler($handler);

if (!$config->apiKey || $config->apiKey === 'YOUR_KEY') {
    $logger->critical('API_KEY not set.');
    exit(1);
}

$outputPath = $argv[1] ?? __DIR__ . '/../innovibe-report.csv';

$api = new InnovibeApiClient($config, $logger);

$fp = fopen($outputPath, 'w');
if ($fp === false) {
    $logger->critical("Cannot open output file: {$outputPath}");
    exit(1);
}

fputcsv($fp, ['JobID', 'Job Title', 'Job Type', 'Location', 'URL', 'NOC Code', 'Education']);

$count = 0;

foreach ($api->fetchAllJobs($config->pageSize) as $job) {
    $id = (string) ($job['id'] ?? '');
    if ($id === '') {
        continue;
    }

    $title = $job['title'] ?? '';

    $jobType = implode(', ', $job['employmentType'] ?? []);

    $city = '';
    $locations = $job['jobLocations'] ?? [];
    if (!empty($locations)) {
        $loc = null;
        foreach ($locations as $candidate) {
            if (strcasecmp($candidate['state'] ?? '', 'British Columbia') === 0) {
                $loc = $candidate;
                break;
            }
        }
        $loc ??= $locations[0];
        $city = trim(($loc['city'] ?? '') . ', ' . ($loc['state'] ?? ''), ', ');
    }

    $url = $job['url'] ?? '';

    $nocCode = '';
    $nocMatches = $job['nocMatches'] ?? [];
    if (!empty($nocMatches)) {
        usort($nocMatches, fn($a, $b) => ($b['score'] ?? 0) <=> ($a['score'] ?? 0));
        $nocCode = (string) ($nocMatches[0]['code'] ?? '');
    }

    $education = resolveEducation($job);

    fputcsv($fp, [$id, $title, $jobType, $city, $url, $nocCode, $education]);
    $count++;
}

fclose($fp);
$logger->info("Report complete: {$count} jobs written to {$outputPath}");

function resolveEducation(array $job): string
{
    $level = $job['educationLevel'] ?? '';
    if ($level !== '') {
        return mapEducationLevel($level) ?? $level;
    }

    $detailed = $job['educationRequirementsDetailed'] ?? [];
    if (!empty($detailed) && is_array($detailed[0])) {
        $cat = $detailed[0]['categoryName'] ?? '';
        if ($cat !== '') {
            $mapped = mapEducationCategory($cat);
            if ($mapped !== null) {
                return $mapped;
            }
        }
        $name = $detailed[0]['name'] ?? '';
        if ($name !== '') {
            return mapEducationLevel($name) ?? $name;
        }
    }

    $reqs = $job['educationRequirements'] ?? [];
    if (!empty($reqs) && is_string($reqs[0]) && $reqs[0] !== '') {
        return mapEducationLevel($reqs[0]) ?? $reqs[0];
    }

    return '';
}

function mapEducationLevel(string $text): ?string
{
    return match (strtolower(trim($text))) {
        'less than high school', 'no education' => 'No education',
        'some college', "associate's degree", 'postsecondary', 'college', 'apprenticeship' => 'College or apprenticeship',
        'doctoral', "master's degree", "bachelor's degree", 'university' => 'University',
        'high school', 'high school diploma', 'high school diploma or equivalent' => 'Secondary school or job-specific training',
        default => null,
    };
}

function mapEducationCategory(string $cat): ?string
{
    return match (strtolower(trim($cat))) {
        'no education' => 'No education',
        'college', 'college or apprenticeship', 'apprenticeship' => 'College or apprenticeship',
        'university' => 'University',
        'secondary school or job-specific training', 'secondary school', 'high school' => 'Secondary school or job-specific training',
        default => null,
    };
}
