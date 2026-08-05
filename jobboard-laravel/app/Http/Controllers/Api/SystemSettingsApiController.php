<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Http\JsonResponse;

/**
 * Build provenance, ported from WorkBC.Web's SystemSettingsController.
 *
 * THIS IS LOAD-BEARING FOR THE WHOLE DRUPAL INTEGRATION, not a debug endpoint.
 * `workbc_jobboard.module`'s jbTestConnection() GETs
 * `{jobboard_api_url_backend}/api/SystemSettings/BuildInfo` on every render of
 * the Find Jobs and Account pages:
 *
 *     $response = $client->get(... . '/api/SystemSettings/BuildInfo');
 *     return !empty($response);
 *
 * and a falsy result replaces the entire job-board region with "The Job Board is
 * currently unavailable." Guzzle throws on a 404, so simply not having this
 * route takes both pages down — which is exactly what happened on dev2 once the
 * Laravel image replaced WorkBC.Web.
 *
 * Discovered 2026-08-05. It is a FOURTH server-to-server endpoint; contracts.md
 * and ADR-009 both previously stated there were only three.
 */
final class SystemSettingsApiController extends Controller
{
    /**
     * GET /api/SystemSettings/BuildInfo — contracts.md §2.6.
     *
     * Key casing is PascalCase to match the anonymous object WorkBC.Web
     * returned. Drupal only checks the response is non-empty, but the .NET
     * smoke tests (src/scripts/test/cases.txt) hit this too, and matching the
     * original costs nothing.
     */
    public function buildInfo(): JsonResponse
    {
        return response()->json([
            'SHA' => (string) config('build.sha'),
            'RunNumber' => (string) config('build.run_number'),
            'BuildDate' => (string) config('build.build_date'),
        ]);
    }
}
