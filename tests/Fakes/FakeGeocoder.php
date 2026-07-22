<?php

namespace Tests\Fakes;

use App\Search\Contracts\Geocoder;
use App\Search\Support\GeoPoint;

/**
 * Test double for the Geocoder contract (mirrors the inline fake used by the
 * FND-7 JobSearchQuery tests): returns fixed coordinates without touching the
 * DB. Pass null to simulate a cache miss / unresolvable location.
 */
final class FakeGeocoder implements Geocoder
{
    public function __construct(private ?GeoPoint $point = null) {}

    public function resolve(string $locationKey): ?GeoPoint
    {
        return $this->point;
    }
}
