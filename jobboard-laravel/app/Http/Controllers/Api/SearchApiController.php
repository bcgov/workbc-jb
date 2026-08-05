<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Http\Requests\Api\JobSearchRequest;
use App\Services\Search\JobSearchService;
use Illuminate\Http\JsonResponse;

/**
 * SRCH-10 — the Drupal-facing job-search API (docs/contracts.md §2.1, §2.2).
 * Thin controller (copilot-instructions §"Coding standards"): validates via
 * {@see JobSearchRequest} then delegates to {@see JobSearchService}, which
 * reuses the SAME FND-7/SRCH-1..9 `JobSearchQuery` used by the public search
 * page — this is only a new response/controller layer on top of it, not a new
 * query engine. Read-only OpenSearch (Rule B).
 */
final class SearchApiController extends Controller
{
    public function __construct(private JobSearchService $service) {}

    /**
     * POST /api/Search/JobSearch(/{language}) — returns the SearchResultsModel
     * shape (camelCase wrapper, PascalCase job items) exactly per contracts §2.1,
     * including the profile-sidebar federal-first heuristic.
     */
    public function jobSearch(JobSearchRequest $request, string $language = ''): JsonResponse
    {
        app()->setLocale($language === 'fr' ? 'fr' : 'en');

        $result = $this->service->searchForApi($request->filters());

        return response()->json($result->toArray());
    }

    /**
     * GET /api/Search/gettotaljobs(/{language}) — contracts.md §2.2.
     */
    public function totalJobs(string $language = ''): JsonResponse
    {
        app()->setLocale($language === 'fr' ? 'fr' : 'en');

        return response()->json(['count' => $this->service->activeJobCount()]);
    }
}
