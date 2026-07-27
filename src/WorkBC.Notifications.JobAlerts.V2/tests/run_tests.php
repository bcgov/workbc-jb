<?php

/**
 * Parity test runner for the Job Alert Notifier PHP port.
 *
 * The files in golden/ were produced by running the fixtures through the
 * REAL C# implementation (WorkBC.ElasticSearch.Search JobSearchQuery,
 * KeywordParsing and SalaryRangeHelper, .NET 6, en-CA culture, with a stubbed
 * IGeocodingService). This runner feeds the same fixtures through the PHP
 * port and requires byte-for-byte identical output.
 *
 * No composer install needed:  php tests/run_tests.php
 */

declare(strict_types=1);

require_once __DIR__ . '/../src/Search/KeywordParsing.php';
require_once __DIR__ . '/../src/Search/SalaryRangeHelper.php';
require_once __DIR__ . '/../src/Search/JobSearchFilters.php';
require_once __DIR__ . '/../src/Search/GeocodingInterface.php';
require_once __DIR__ . '/../src/Search/JobSearchQuery.php';
require_once __DIR__ . '/../src/Service/JobAlertEmail.php';

use WorkBC\JobAlertNotifier\Search\GeocodingInterface;
use WorkBC\JobAlertNotifier\Search\JobSearchFilters;
use WorkBC\JobAlertNotifier\Search\JobSearchQuery;
use WorkBC\JobAlertNotifier\Search\KeywordParsing;
use WorkBC\JobAlertNotifier\Search\SalaryRangeHelper;
use WorkBC\JobAlertNotifier\Service\JobAlertEmail;

/** Same fixed coordinates as the C# StubGeocoder that produced the goldens. */
final class StubGeocoder implements GeocodingInterface
{
    private const KNOWN = [
        'V6B4N7, CANADA' => ['Latitude' => '49.2790', 'Longitude' => '-123.1120'],
        'Surrey, BC, CANADA' => ['Latitude' => '49.104431', 'Longitude' => '-122.801094'],
    ];

    public function getLocation(string $location): ?array
    {
        return self::KNOWN[$location] ?? null;
    }
}

$failures = 0;
$passes = 0;

function check(string $name, string $expected, string $actual): void
{
    global $failures, $passes;
    if ($expected === $actual) {
        $passes++;
        return;
    }
    $failures++;
    echo "FAIL: {$name}\n";
    $len = min(strlen($expected), strlen($actual));
    for ($i = 0; $i < $len; $i++) {
        if ($expected[$i] !== $actual[$i]) {
            $from = max(0, $i - 60);
            echo '  first diff at byte ' . $i . "\n";
            echo '  expected: …' . substr($expected, $from, 120) . "…\n";
            echo '  actual:   …' . substr($actual, $from, 120) . "…\n";
            return;
        }
    }
    echo '  lengths differ: expected ' . strlen($expected) . ', actual ' . strlen($actual) . "\n";
}

$base = __DIR__;
$template = file_get_contents($base . '/../resources/jobsearch_main.json.template');
$geocoder = new StubGeocoder();

// ── 1. Full query JSON parity ───────────────────────────────────────
foreach (glob($base . '/fixtures/*.filters.json') as $fixtureFile) {
    $name = basename($fixtureFile, '.filters.json');
    $goldenFile = $base . '/golden/' . $name . '.golden.json';
    if (!file_exists($goldenFile)) {
        echo "FAIL: missing golden file for {$name}\n";
        $failures++;
        continue;
    }

    $filters = JobSearchFilters::fromJson(file_get_contents($fixtureFile));
    $query = new JobSearchQuery($geocoder, $filters);
    $actual = $query->toJson($template);
    $expected = file_get_contents($goldenFile);

    check("query {$name}", $expected, $actual);

    // every generated body must also be valid JSON
    if (json_decode($actual) === null) {
        echo "FAIL: query {$name} did not produce valid JSON\n";
        $failures++;
    }
}

// ── 2. Keyword parsing parity ───────────────────────────────────────
$keywordGolden = json_decode(file_get_contents($base . '/golden/keywords.golden.json'), true);
foreach ($keywordGolden as $input => $expected) {
    check("keyword '{$input}'", $expected, KeywordParsing::buildSimpleQueryString((string) $input));
}

// ── 3. Salary range parity ──────────────────────────────────────────
$salaryGolden = json_decode(file_get_contents($base . '/golden/salary_ranges.golden.json'), true);
foreach ($salaryGolden as $key => $expected) {
    [$type, $bracket] = array_map('intval', explode('_', (string) $key));
    $actual = SalaryRangeHelper::getAnnualRange($type, $bracket);
    check("salary {$key} min", $expected[0], $actual[0]);
    check("salary {$key} max", $expected[1], $actual[1]);
}

// ── 4. Email formatting (string.Format semantics) ───────────────────
$email = new JobAlertEmail(
    42,
    'Baker Jobs',
    JobAlertEmail::FREQUENCY_WEEKLY,
    'user-guid-1',
    'Pat',
    'pat@example.com',
    'https://www.workbc.ca/jobs',
    'WorkBC Job Alert - {0}',
    "Hi {0},\nYour {1} job alert \"{2}\" has new jobs: {3}",
    '<html>{{css}}<body>Hi {0}, your {1} alert <b>{2}</b>: <a href="{3}">{4}</a></body></html>'
);

check('email subject', 'WorkBC Job Alert - Baker Jobs', $email->emailSubject());
check(
    'email text',
    "Hi Pat,\nYour weekly job alert \"Baker Jobs\" has new jobs: "
        . 'https://www.workbc.ca/jobs#/job-search/r;nid=42;jsid=user-guid-1',
    $email->textMessage()
);
check(
    'email html',
    '<html>{css}<body>Hi Pat, your weekly alert <b>Baker Jobs</b>: '
        . '<a href="https://www.workbc.ca/jobs#/job-search/r;nid=42;jsid=user-guid-1">'
        . 'WorkBC Job Alert - Baker Jobs</a></body></html>',
    $email->htmlMessage()
);
check('email domain', 'example.com', $email->emailDomain());

// ── Summary ─────────────────────────────────────────────────────────
echo "\n{$passes} passed, {$failures} failed\n";
exit($failures === 0 ? 0 : 1);
