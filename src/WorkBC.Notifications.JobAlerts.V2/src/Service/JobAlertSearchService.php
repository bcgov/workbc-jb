<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Service;

use WorkBC\JobAlertNotifier\Elastic\ElasticSearchClient;
use WorkBC\JobAlertNotifier\Search\DateField;
use WorkBC\JobAlertNotifier\Search\GeocodingService;
use WorkBC\JobAlertNotifier\Search\JobSearchFilters;
use WorkBC\JobAlertNotifier\Search\JobSearchQuery;

/**
 * Port of WorkBC.Notifications.JobAlerts.Services.JobAlertSearchService —
 * the wrapper around Elasticsearch used by the job alert task.
 */
final class JobAlertSearchService
{
    private string $template;

    public function __construct(
        private readonly ElasticSearchClient $elastic,
        private readonly GeocodingService $geocodingService,
        private readonly string $index,
    ) {
        $path = dirname(__DIR__, 2) . '/resources/jobsearch_main.json.template';
        $template = file_get_contents($path);
        if ($template === false) {
            throw new \RuntimeException("Unable to read resource file: {$path}");
        }
        $this->template = $template;
    }

    /**
     * Searches for jobs matching the alert's saved filter and posted within
     * the alert frequency window; returns the total hit count.
     */
    public function getJobAlertSearchResultCount(string $jobSearchFiltersJson, int $alertFrequency): int
    {
        $filters = JobSearchFilters::fromJson($jobSearchFiltersJson);
        $filters->pageSize = 0;

        $startDate = self::getFilterStartDate($alertFrequency);

        $filters->searchDateSelection = '3'; // custom date range
        $filters->startDate = DateField::fromDateTime($startDate);
        $filters->endDate = DateField::fromDateTime(new \DateTimeImmutable('now'));

        $query = new JobSearchQuery($this->geocodingService, $filters);
        $json = $query->toJson($this->template);

        $results = $this->elastic->search($this->index, $json);

        return (int) ($results['hits']['total']['value'] ?? 0);
    }

    /**
     * Gets the start date for the filter based on frequency:
     * daily −1 day, weekly −7, biweekly −15, monthly (default) −31,
     * truncated to midnight like DateTime.Date.
     */
    private static function getFilterStartDate(int $frequency): \DateTimeImmutable
    {
        $days = match ($frequency) {
            JobAlertEmail::FREQUENCY_DAILY => 1,
            JobAlertEmail::FREQUENCY_WEEKLY => 7,
            JobAlertEmail::FREQUENCY_BIWEEKLY => 15,
            default => 31,
        };

        return (new \DateTimeImmutable("now -{$days} days"))->setTime(0, 0, 0, 0);
    }
}
