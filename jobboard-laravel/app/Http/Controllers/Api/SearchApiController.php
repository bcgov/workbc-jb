<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Http\Requests\Api\JobSearchRequest;
use App\Services\Search\JobSearchService;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;

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
     * GET /api/Search/GetTotalJobs(/{language}) — contracts.md §2.2.
     *
     * Returns a BARE INTEGER, not an object. WorkBC.Web's action is
     * `Task<int> GetTotalJobs(...)`, so ASP.NET serialises the body as `42802`
     * — verified against the .NET dev origin on 2026-08-05. Consumers coerce
     * the body straight to a number, so wrapping it as `{"count": n}` yields
     * NaN downstream; that is what produced "Search NaN jobs in B.C." on the
     * WorkBC home page. Do not "improve" this into an object.
     */
    public function totalJobs(Request $request, string $language = ''): JsonResponse
    {
        // .NET bound `language` from the route segment OR the query string —
        // cases.txt exercises `GetTotalJobs/fr` and `GetTotalJobs?language=fr`
        // alike. Without the query-string fallback, `?language=fr` silently
        // returned the ENGLISH count rather than failing visibly.
        if ($language === '') {
            $language = (string) $request->query('language', '');
        }

        app()->setLocale($language === 'fr' ? 'fr' : 'en');

        return response()->json($this->service->activeJobCount());
    }
}
