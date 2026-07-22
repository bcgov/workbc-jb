<?php

namespace App\Search\Contracts;

use App\Search\Support\GeoPoint;

/**
 * Resolves a location string (a postal-code or "City, BC, CANADA" key) into a
 * geographic point for radius (geo_distance) searches.
 *
 * Per copilot-instructions (Enforced constraints #4), any external call the app
 * makes — geocoding included — goes through an adapter, never inline in a web
 * request. JobSearchQuery depends on this contract only; the concrete Google
 * Maps / cache-backed adapter is provided by the Integration layer (later story).
 */
interface Geocoder
{
    /**
     * @return GeoPoint|null null when the location cannot be resolved
     */
    public function resolve(string $locationKey): ?GeoPoint;
}
