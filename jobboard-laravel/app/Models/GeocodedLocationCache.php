<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

/**
 * Maps the EXISTING `GeocodedLocationCache` table (data-model.md §6; unique
 * `Name`). It is a cache of geocoded locations written by the existing pipeline
 * and the legacy C# GeocodingService.
 *
 * Map, don't create (copilot-instructions): existing PascalCase schema, no
 * timestamps, string PK-less identity `Id`. This app only READS the cache for
 * radius search (Rule B); it never creates/alters the table. The Google Maps
 * write-back path (which WOULD insert rows) is deferred — see {@see
 * \App\Services\Integration\CachedGeocoder}.
 */
final class GeocodedLocationCache extends Model
{
    protected $table = 'GeocodedLocationCache';

    protected $primaryKey = 'Id';

    public $timestamps = false;

    protected $casts = [
        'DateGeocoded' => 'datetime',
        'IsPermanent' => 'bool',
    ];
}
