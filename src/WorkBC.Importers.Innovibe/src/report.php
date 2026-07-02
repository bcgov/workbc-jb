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
// Default 90 days to match the importer and indexer (General.DefaultWantedJobExpiryDays),
// so the report's "still live" cutoff agrees with Jobs.ExpireDate / the ES ExpireDate.
$expiryDays = (int) (getenv('JOB_EXPIRY_DAYS') ?: 90);

$api = new InnovibeApiClient($config, $logger);

$fp = fopen($outputPath, 'w');
if ($fp === false) {
    $logger->critical("Cannot open output file: {$outputPath}");
    exit(1);
}

fputcsv($fp, ['JobID', 'Job Title', 'Job Type', 'Location', 'URL', 'NOC Code', 'Education']);

$count = 0;
$skipped = 0;
$now = time();

foreach ($api->fetchAllJobs($config->pageSize) as $job) {
    $id = (string) ($job['id'] ?? '');
    if ($id === '') {
        continue;
    }

    // Same filters the importer + indexer apply before a job reaches the UI

    if (empty($job['salaryMin']) && empty($job['salaryMax']) && empty($job['salaryValue'])) {
        $skipped++;
        continue;
    }

    $title = trim($job['title'] ?? '');
    if ($title === '') {
        $skipped++;
        continue;
    }

    // Expiry: COALESCE(dateValidThrough, updatedAt + expiryDays)
    $validThrough = !empty($job['dateValidThrough']) ? strtotime((string) $job['dateValidThrough']) : false;
    if ($validThrough !== false) {
        $expireTs = $validThrough;
    } else {
        $updatedAt = strtotime($job['updatedAt'] ?? $job['postedDate'] ?? $job['createdAt'] ?? 'now');
        $expireTs = $updatedAt + ($expiryDays * 86400);
    }
    if ($expireTs < $now) {
        $skipped++;
        continue;
    }

    // Must have at least one NOC match
    $nocMatches = $job['nocMatches'] ?? [];
    if (empty($nocMatches)) {
        $skipped++;
        continue;
    }
    usort($nocMatches, fn($a, $b) => ($b['score'] ?? 0) <=> ($a['score'] ?? 0));
    $nocCode = (string) ($nocMatches[0]['code'] ?? '');

    // Must have a BC location with a city
    $city = '';
    $locations = $job['jobLocations'] ?? [];
    if (!empty($locations)) {
        foreach ($locations as $candidate) {
            if (strcasecmp($candidate['state'] ?? '', 'British Columbia') === 0 && !empty($candidate['city'])) {
                $city = trim($candidate['city'] . ', ' . ($candidate['state'] ?? ''), ', ');
                break;
            }
        }
        if ($city === '') {
            $loc = $locations[0];
            $city = trim(($loc['city'] ?? '') . ', ' . ($loc['state'] ?? ''), ', ');
        }
    }
    if ($city === '') {
        $skipped++;
        continue;
    }

    // Must have an employer name with English characters
    $employerName = '';
    $company = $job['company'] ?? null;
    if (is_array($company) && !empty($company['name'])) {
        $rawName = trim($company['name']);
        if (preg_match('/[a-zA-Z0-9]/', $rawName)) {
            $employerName = $rawName;
        }
    }
    if ($employerName === '') {
        $skipped++;
        continue;
    }

    $jobType = implode(', ', $job['employmentType'] ?? []);
    $url = $job['url'] ?? '';
    $education = resolveEducation($job);

    fputcsv($fp, [$id, $title, $jobType, $city, $url, $nocCode, $education]);
    $count++;
}

fclose($fp);
$logger->info("Report complete: {$count} jobs written, {$skipped} skipped → {$outputPath}");

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
