<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Search;

/** Port of WorkBC.Shared.Services.IGeocodingService. */
interface GeocodingInterface
{
    /**
     * Resolve a location key (e.g. "V6B4N7, CANADA") to coordinates.
     * Latitude/Longitude are returned as strings and concatenated verbatim
     * into the ES query, like the C# GeocodedLocationCache varchar columns.
     *
     * @return array{Latitude: string, Longitude: string}|null
     */
    public function getLocation(string $location): ?array;
}
