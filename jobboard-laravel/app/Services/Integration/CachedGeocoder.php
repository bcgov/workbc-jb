<?php

namespace App\Services\Integration;

use App\Models\GeocodedLocationCache;
use App\Search\Contracts\Geocoder;
use App\Search\Support\GeoPoint;

/**
 * DB-backed {@see Geocoder} adapter — mirrors the legacy
 * `WorkBC.Shared.Services.GeocodingService.GetLocation`: resolve a location
 * key CACHE-FIRST from `GeocodedLocationCache` (lookup by `Name`, coordinates
 * in `Latitude`/`Longitude`).
 *
 * Read-only for now (Rule B): the JobSearchQuery passes the same key format the
 * cache is populated with ("{postal}, CANADA" / "{city}, BC, CANADA").
 *
 * External call isolated in an adapter (constraint #4): no geocoding happens
 * inline in a web request — JobSearchQuery depends only on the Geocoder
 * contract, and this adapter is the seam where the deferred Google Maps call
 * will live.
 */
final class CachedGeocoder implements Geocoder
{
    public function resolve(string $locationKey): ?GeoPoint
    {
        $row = GeocodedLocationCache::query()
            ->where('Name', $locationKey)
            ->first();

        if ($row === null) {
            // TODO(SRCH-2 follow-up): on a cache miss, geocode via Google Maps
            // (AppSettings:GoogleMapsIPApi) and WRITE BACK a GeocodedLocationCache
            // row, mirroring GeocodingService.CreateLocation. Deferred here (no API
            // key). Until then a miss returns null, so JobSearchQuery applies its
            // invalid-location fallback (1 km at lat 0/lon 180 → no results).
            return null;
        }

        if (! $this->isNumeric($row->Latitude) || ! $this->isNumeric($row->Longitude)) {
            return null;
        }

        return new GeoPoint((float) $row->Latitude, (float) $row->Longitude);
    }

    private function isNumeric(mixed $value): bool
    {
        return $value !== null && $value !== '' && is_numeric($value);
    }
}
