<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

/**
 * Maps the EXISTING `JobViews` table (shared with the legacy C# app —
 * WorkBC.Data.Model.JobBoard.JobView). One row per job with a running view
 * counter and the last-viewed timestamp; the PK is the string `JobId`.
 *
 * Map, don't create (copilot-instructions): existing PascalCase schema, no
 * timestamps, non-incrementing string key. SRCH-7 writes to this table on the
 * job-detail read path (federal jobs only) — the ONLY write this read-focused
 * app performs, and always fire-and-forget so a DB hiccup never blocks a render.
 */
final class JobView extends Model
{
    protected $table = 'JobViews';

    protected $primaryKey = 'JobId';

    protected $keyType = 'string';

    public $incrementing = false;

    public $timestamps = false;

    /** @var list<string> */
    protected $fillable = ['JobId', 'Views', 'DateLastViewed'];

    protected $casts = [
        'Views' => 'integer',
        'DateLastViewed' => 'datetime',
    ];
}
