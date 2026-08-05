<?php

namespace App\Services\Search;

use App\Models\JobView;
use App\Search\Results\JobDetail;
use App\Search\Results\JobResult;
use OpenSearch\Client;
use Throwable;

/**
 * Fetches a single job by id for the SEO-critical detail page — the Laravel
 * mirror of the legacy WorkBC.Web JobDetailService.
 *
 * The fetch READS the derived OpenSearch read model only (ADR-001, Rule B):
 * a `terms` query on `_id` with size 1 and, deliberately, NO ExpireDate
 * filter, so a linked/expired job still renders. The one write this app makes
 * is the federal-job view counter (see {@see recordView()}), which is always
 * fire-and-forget so it can never block or fail a render.
 */
final class JobDetailService
{
    public function __construct(
        private Client $client,
    ) {}

    /**
     * Fetch and project a job by its OpenSearch `_id` (== JobId). Returns null
     * when no document matches. Does NOT record a view — the caller decides
     * whether the request is the canonical one before counting (see the
     * controller) to avoid double-counting on redirects.
     */
    public function find(string $jobId): ?JobDetail
    {
        $response = $this->client->search([
            'index' => $this->index(),
            'body' => [
                'size' => 1,
                'query' => ['terms' => ['_id' => [$jobId]]],
            ],
        ]);

        $hit = $response['hits']['hits'][0] ?? null;
        if ($hit === null) {
            return null;
        }

        $data = JobResult::fromSource($hit['_source'] ?? [])->toArray();

        return new JobDetail($data);
    }

    /**
     * Increment the running view count for a federal job (mirrors the C#
     * JobDetailService rule: `!isToggle && isFederalJob`). Creating the first
     * row is the intended behaviour — map the existing table, but adding a
     * counter row is a legitimate write of the derived counter, not a schema
     * change. Fire-and-forget: any failure is reported and swallowed so the
     * page still renders.
     *
     * @return int|null  The new view count, or null when nothing was recorded.
     */
    public function recordView(string $jobId, bool $isFederalJob, bool $isToggle = false): ?int
    {
        if ($isToggle || ! $isFederalJob) {
            return null;
        }

        try {
            $row = JobView::find($jobId);

            if ($row === null) {
                JobView::create([
                    'JobId' => $jobId,
                    'Views' => 1,
                    'DateLastViewed' => now(),
                ]);

                return 1;
            }

            $row->Views = (int) ($row->Views ?? 0) + 1;
            $row->DateLastViewed = now();
            $row->save();

            return $row->Views;
        } catch (Throwable $e) {
            report($e);

            return null;
        }
    }

    /**
     * The index to query for the active locale (contracts: en/fr are the two
     * derived indexes). Defaults to the English index.
     */
    private function index(): string
    {
        $key = app()->getLocale() === 'fr' ? 'fr' : 'en';

        return (string) config("opensearch.indexes.{$key}", config('opensearch.indexes.en'));
    }
}
