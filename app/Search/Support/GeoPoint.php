<?php

namespace App\Search\Support;

/**
 * A resolved geographic point (used for geo_distance radius filters and geo
 * distance sorting). Values are decimal degrees.
 */
final class GeoPoint
{
    public function __construct(
        public float $lat,
        public float $lon,
    ) {}
}
