<?php

namespace App\Console\Commands;

use Illuminate\Console\Command;
use OpenSearch\Client;

/**
 * DEV-ONLY. Generates fake job documents that match the real jobs_en/jobs_fr shape
 * (docs/opensearch/README.md + jobs-index.json) and bulk-indexes them, so the search
 * layer can be built/tested before real data is available. Never run against a cluster
 * holding real data. Read-model only — this stands in for the external indexer locally.
 */
class DevIndexJobs extends Command
{
    protected $signature = 'dev:index-jobs
        {--count=500 : How many jobs to generate}
        {--fresh : Delete & recreate jobs_en/jobs_fr from docs/opensearch/jobs-index.json first}
        {--federal-ratio=0.3 : Fraction that are federal (rest external, mirroring ~10k/25k)}';

    protected $description = '[dev] Generate fake job documents into jobs_en/jobs_fr';

    /** @var array{0:string,1:float,2:float,3:string}[] name, lat, lon, region */
    private array $cities = [
        ['Vancouver', 49.2827, -123.1207, 'Mainland / Southwest'],
        ['Surrey', 49.1913, -122.8490, 'Mainland / Southwest'],
        ['Burnaby', 49.2488, -122.9805, 'Mainland / Southwest'],
        ['Richmond', 49.1666, -123.1336, 'Mainland / Southwest'],
        ['Coquitlam', 49.2838, -122.7932, 'Mainland / Southwest'],
        ['Abbotsford', 49.0504, -122.3045, 'Mainland / Southwest'],
        ['Chilliwack', 49.1579, -121.9514, 'Mainland / Southwest'],
        ['Victoria', 48.4284, -123.3656, 'Vancouver Island / Coast'],
        ['Nanaimo', 49.1659, -123.9401, 'Vancouver Island / Coast'],
        ['Campbell River', 50.0244, -125.2475, 'Vancouver Island / Coast'],
        ['Kelowna', 49.8880, -119.4960, 'Thompson-Okanagan'],
        ['Kamloops', 50.6745, -120.3273, 'Thompson-Okanagan'],
        ['Vernon', 50.2670, -119.2720, 'Thompson-Okanagan'],
        ['Prince George', 53.9171, -122.7497, 'Cariboo'],
        ['Prince Rupert', 54.3150, -130.3208, 'North Coast & Nechako'],
        ['Fort St. John', 56.2465, -120.8476, 'Northeast'],
        ['Cranbrook', 49.5122, -115.7686, 'Kootenay'],
    ];

    /** @var array{0:int,1:int,2:string}[] noc2021, noc2016, title */
    private array $nocs = [
        [44101, 4412, 'home support worker'],
        [62100, 6221, 'Technical sales specialists - wholesale trade'],
        [21231, 2173, 'Software engineers and designers'],
        [31301, 3012, 'Registered nurses and registered psychiatric nurses'],
        [63200, 6322, 'Cooks'],
        [73400, 7521, 'Heavy equipment operators'],
        [64100, 6421, 'Retail salespersons and visual merchandisers'],
        [75110, 7611, 'Construction trades helpers and labourers'],
        [13100, 1221, 'Administrative officers'],
        [41200, 4011, 'University professors and lecturers'],
        [72400, 7371, 'Crane operators'],
        [65100, 6511, 'Food and beverage servers'],
    ];

    /** @var array{0:string,1:int}[] industry, naics sector id */
    private array $industries = [
        ['Financial Services', 52], ['Health Care', 62], ['Construction', 23],
        ['Retail Trade', 46], ['Professional Services', 54], ['Accommodation and Food Services', 72],
        ['Educational Services', 61], ['Manufacturing', 31], ['Transportation and Warehousing', 48],
    ];

    private array $externalHosts = [
        'jobs.ashbyhq.com', 'indeed.com', 'ca.linkedin.com', 'workopolis.com', 'glassdoor.ca',
    ];

    // WorkplaceTypeId enum (Shared/Constants/WorkplaceType.cs): OnSite=0, Hybrid=100000, Travelling=100001, Virtual=15141
    private array $workplaceTypes = [
        ['Id' => 0, 'Description' => 'On-site only'],
        ['Id' => 0, 'Description' => 'On-site only'],
        ['Id' => 100000, 'Description' => 'Hybrid'],
        ['Id' => 100001, 'Description' => 'Travelling'],
        ['Id' => 15141, 'Description' => 'Virtual'],
    ];

    // Real EduLevel values written by the indexer (XmlParsingServiceFederal.cs:675-691) —
    // must match exactly, since the education facet terms on EduLevel.keyword with these strings.
    private array $eduLevels = [
        'No education', 'Secondary school or job-specific training',
        'College or apprenticeship', 'University',
    ];

    private int $lastNoc2016 = 0;

    private \Faker\Generator $faker;

    public function handle(Client $client): int
    {
        if (app()->environment('production')) {
            $this->error('Refusing to run in production. This is a dev-only seeder.');

            return self::FAILURE;
        }

        $this->faker = \Faker\Factory::create('en_CA');
        mt_srand(20260722); // deterministic-ish batches

        $count = max(1, (int) $this->option('count'));
        $federalRatio = (float) $this->option('federal-ratio');

        if ($this->option('fresh')) {
            $this->recreateIndexes($client);
        }

        $this->info("Generating {$count} fake jobs...");
        $bar = $this->output->createProgressBar($count);

        $buffer = [];
        $flush = function () use (&$buffer, $client) {
            if ($buffer === []) {
                return;
            }
            $resp = $client->bulk(['body' => $buffer]);
            if (($resp['errors'] ?? false) === true) {
                foreach ($resp['items'] as $item) {
                    if ($err = ($item['index']['error'] ?? null)) {
                        $this->newLine();
                        $this->warn('bulk index error: '.json_encode($err));
                        break;
                    }
                }
            }
            $buffer = [];
        };

        for ($i = 0; $i < $count; $i++) {
            $isFederal = mt_rand(1, 1000) / 1000 < $federalRatio;
            $doc = $isFederal ? $this->federalJob() : $this->externalJob();

            // Index the same doc into both language indexes (jobs_fr reuses EN content
            // locally; real FR carries translations for federal jobs).
            foreach (['jobs_en' => 'en', 'jobs_fr' => 'fr'] as $index => $lang) {
                $buffer[] = ['index' => ['_index' => $index, '_id' => $doc['JobId']]];
                $buffer[] = ['Lang' => $lang] + $doc;
            }

            if (count($buffer) >= 1000) {
                $flush();
            }
            $bar->advance();
        }
        $flush();

        $client->indices()->refresh(['index' => 'jobs_en,jobs_fr']);
        $bar->finish();
        $this->newLine(2);

        foreach (['jobs_en', 'jobs_fr'] as $index) {
            $c = $client->count(['index' => $index])['count'] ?? 0;
            $this->line("  {$index}: <info>".number_format($c).' docs</info>');
        }

        return self::SUCCESS;
    }

    private function recreateIndexes(Client $client): void
    {
        $body = json_decode(file_get_contents(base_path('docs/opensearch/jobs-index.json')), true);

        foreach (['jobs_en', 'jobs_fr'] as $index) {
            if ($client->indices()->exists(['index' => $index])) {
                $client->indices()->delete(['index' => $index]);
            }
            $client->indices()->create(['index' => $index, 'body' => $body]);
            $this->line("  recreated <info>{$index}</info>");
        }
    }

    /** Fields shared by both job types. */
    private function baseJob(): array
    {
        [$city, $lat, $lon, $region] = $this->cities[array_rand($this->cities)];
        [$noc2021, $noc2016, $title] = $this->nocs[array_rand($this->nocs)];
        $this->lastNoc2016 = $noc2016;

        $posted = $this->faker->dateTimeBetween('-90 days', '-1 day');
        // ~15% expired; rest active (ExpireDate >= now).
        $expired = mt_rand(1, 100) <= 15;
        $expire = (clone $posted)->modify('+'.mt_rand(30, 90).' days');
        if (! $expired && $expire < new \DateTime()) {
            $expire = (new \DateTime())->modify('+'.mt_rand(1, 60).' days');
        }
        if ($expired) {
            $expire = (new \DateTime())->modify('-'.mt_rand(1, 20).' days');
        }
        $updated = $this->faker->dateTimeBetween($posted, 'now');

        $hourly = mt_rand(1750, 6500) / 100;      // $17.50–$65.00/hr
        $salary = (int) round($hourly * 2080);     // annualized (matches real docs)

        return [
            'DatePosted' => $posted->format('Y-m-d\TH:i:sP'),
            'ExpireDate' => $expire->format('Y-m-d\TH:i:sP'),
            'LastUpdated' => $updated->format('Y-m-d\TH:i:sP'),
            'Title' => $title,
            'NocJobTitle' => $title,
            'Noc2021' => (float) $noc2021,
            'NocGroup' => ucfirst($title)." ({$noc2021})",
            'EmployerName' => $this->faker->company(),
            'EmployerTypeId' => mt_rand(1, 100) <= 8 ? 1 : 0, // ~8% placement agencies
            'City' => [$city],
            'Region' => [$region],
            'Location' => [['Lat' => (string) $lat, 'Lon' => (string) $lon]],
            'LocationGeo' => ["{$lat},{$lon}"],
            'PositionsAvailable' => mt_rand(1, 5),
            'IsVariousLocation' => false,
            'WorkHours' => (float) mt_rand(20, 40),
            'HoursOfWork' => ['Description' => [mt_rand(0, 1) ? 'Full-time' : 'Part-time']],
            'Salary' => (float) $salary,
            'SalarySort' => ['Ascending' => (float) $salary, 'Descending' => (float) $salary],
            'SalarySummary' => '$'.number_format($salary).' annually',
        ];
    }

    private function federalJob(): array
    {
        $job = $this->baseJob();

        $flags = [
            'IsAboriginal', 'IsApprentice', 'IsStudent', 'IsNewcomer', 'IsVeteran',
            'IsVismin', 'IsYouth', 'IsMatureWorker', 'IsDisability',
        ];
        $equity = [];
        foreach ($flags as $flag) {
            $equity[$flag] = mt_rand(1, 100) <= 25;
        }

        $wpt = $this->workplaceTypes[array_rand($this->workplaceTypes)];

        return $job + $equity + [
            'JobId' => (string) mt_rand(40000000, 59999999),
            'IsFederalJob' => true,
            'Noc' => $this->lastNoc2016,
            'Lang' => 'en',
            'WorkLangCd' => ['Description' => mt_rand(0, 4) ? ['English'] : ['English', 'French']],
            'WageClass' => ['A', 'C', 'E', 'H'][array_rand([0, 1, 2, 3])],
            'PostalCode' => $this->bcPostalCode(),
            'Province' => 'BC',
            'EduLevel' => $this->eduLevels[array_rand($this->eduLevels)],
            'SalaryDescription' => '$'.number_format($job['Salary'] / 2080, 2).' hourly for '.((int) $job['WorkHours']).' hours per week',
            'PeriodOfEmployment' => ['Description' => [['Permanent', 'Temporary', 'Seasonal', 'Casual'][array_rand([0, 1, 2, 3])]]],
            'EmploymentTerms' => ['Description' => [['Flexible hours', 'Day', 'Evening', 'Weekend'][array_rand([0, 1, 2, 3])]]],
            'SalaryConditions' => ['Description' => []],
            'WorkplaceType' => $wpt,
            'NaicsId' => $this->industries[array_rand($this->industries)][1],
            'ProgramName' => '',
            'ProgramDescription' => '',
            'SkillCategories' => $this->federalSkillCategories($equity),
            // Apply* contact block (mostly empty, like real federal docs)
            'ApplyEmailAddress' => mt_rand(0, 1) ? $this->faker->safeEmail() : '',
            'ApplyPhoneNumber' => '', 'ApplyPhoneNumberExt' => '', 'ApplyFaxNumber' => '',
            'ApplyWebsite' => '', 'ApplyPersonRoom' => '', 'ApplyPersonStreet' => '',
            'ApplyPersonCity' => '', 'ApplyPersonPostalCode' => '', 'ApplyPersonProvince' => '',
            'ApplyMailRoom' => '', 'ApplyMailStreet' => '', 'ApplyMailCity' => '',
            'ApplyMailPostalCode' => '', 'ApplyMailProvince' => '',
        ];
    }

    private function externalJob(): array
    {
        $job = $this->baseJob();
        [$industry, $naics] = $this->industries[array_rand($this->industries)];
        $host = $this->externalHosts[array_rand($this->externalHosts)];
        $url = "https://{$host}/".$this->faker->uuid();

        $noEquity = array_fill_keys([
            'IsAboriginal', 'IsApprentice', 'IsStudent', 'IsNewcomer', 'IsVeteran',
            'IsVismin', 'IsYouth', 'IsMatureWorker', 'IsDisability',
        ], false);

        return $job + $noEquity + [
            'JobId' => $this->cuid(),
            'IsFederalJob' => false,
            'Lang' => 'en',
            'Province' => 'British Columbia', // note: full name for external (matches real data)
            'Industry' => $industry,
            'NaicsId' => $naics,
            'Occupation' => $job['Title'],
            'Function' => '',
            'JobDescription' => $this->jobDescription(),
            'SkillCategories' => [],
            'PeriodOfEmployment' => ['Description' => []],
            'EmploymentTerms' => (object) [],
            'ExternalSource' => ['Source' => [['Url' => $url, 'Source' => $host]]],
            'ApplyWebsite' => $url,
            'ApplyEmailAddress' => '', 'ApplyPhoneNumber' => '',
        ];
    }

    private function federalSkillCategories(array $equity): array
    {
        $cats = [
            ['Category' => ['Id' => 195, 'Name' => 'Education'], 'SkillCount' => 1,
                'Skills' => ['Secondary (high) school graduation certificate']],
            ['Category' => ['Id' => 29, 'Name' => 'Tasks'], 'SkillCount' => 2,
                'Skills' => [$this->faker->sentence(4), $this->faker->sentence(5)]],
            ['Category' => ['Id' => 291, 'Name' => 'Personal suitability'], 'SkillCount' => 3,
                'Skills' => ['Reliability', 'Team player', 'Punctuality']],
            ['Category' => ['Id' => 100001, 'Name' => 'Experience'], 'SkillCount' => 1,
                'Skills' => ['1 to less than 7 months']],
        ];

        // Equity flags mirror the "Support for …" categories in real federal docs.
        $supportMap = [
            'IsDisability' => [104203, 'Support for persons with disabilities'],
            'IsNewcomer' => [104197, 'Support for newcomers and refugees'],
            'IsYouth' => [104198, 'Support for youths'],
            'IsVeteran' => [104199, 'Support for Veterans'],
            'IsAboriginal' => [104200, 'Support for Indigenous people'],
            'IsMatureWorker' => [104201, 'Support for mature workers'],
            'IsVismin' => [104202, 'Supports for visible minorities'],
        ];
        foreach ($supportMap as $flag => [$id, $name]) {
            if ($equity[$flag] ?? false) {
                $cats[] = ['Category' => ['Id' => $id, 'Name' => $name], 'SkillCount' => 1,
                    'Skills' => ["Participates in a program or initiative that supports {$name}"]];
            }
        }

        return $cats;
    }

    private function jobDescription(): string
    {
        return implode("\n", [
            $this->faker->company().' is hiring for a '.strtolower($this->faker->jobTitle()).' role.',
            $this->faker->paragraph(5),
            'Responsibilities:',
            '- '.$this->faker->sentence(6),
            '- '.$this->faker->sentence(6),
            '- '.$this->faker->sentence(6),
            'Requirements:',
            '- '.$this->faker->sentence(8),
            '- '.$this->faker->sentence(7),
            $this->faker->paragraph(3),
        ]);
    }

    private function bcPostalCode(): string
    {
        $letters = 'ABCEGHJKLMNPRSTVXY';
        return 'V'.mt_rand(0, 9).$letters[mt_rand(0, strlen($letters) - 1)]
            .mt_rand(0, 9).$letters[mt_rand(0, strlen($letters) - 1)].mt_rand(0, 9);
    }

    private function cuid(): string
    {
        $chars = 'abcdefghijklmnopqrstuvwxyz0123456789';
        $s = 'c';
        for ($i = 0; $i < 24; $i++) {
            $s .= $chars[mt_rand(0, strlen($chars) - 1)];
        }

        return $s;
    }
}
