<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Services\Search\LocationService;
use Illuminate\Http\JsonResponse;

/**
 * SRCH-10 — `GET /api/location/cities/{cityName}/{includeRegion}` (contracts.md
 * §2.3): city-name autocomplete for the Drupal-facing API. Delegates to the
 * SAME {@see LocationService} the SRCH-2 search-page combobox uses, so
 * suggestions are identical between the two surfaces.
 */
final class LocationApiController extends Controller
{
    public function __construct(private LocationService $service) {}

    public function cities(string $cityName, string $includeRegion = 'false'): JsonResponse
    {
        return response()->json($this->service->suggestCities($cityName));
    }
}
