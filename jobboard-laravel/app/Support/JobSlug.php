<?php

namespace App\Support;

use Illuminate\Support\Str;

/**
 * Builds and parses the path-based, crawlable job-detail URL segment
 * (`{slug}-{JobId}`; architecture.md §6, ADR-002 — no hash routing).
 *
 * JobIds are alphanumeric with no hyphens, so the id is always the substring
 * after the final hyphen; the leading slug is a human/SEO-friendly decoration
 * that is regenerated from the title for the canonical URL.
 */
final class JobSlug
{
    /**
     * Compose the `{slug}-{JobId}` path segment for a job.
     */
    public static function path(string $jobId, ?string $title): string
    {
        $slug = Str::slug((string) $title);

        return $slug === '' ? $jobId : "{$slug}-{$jobId}";
    }

    /**
     * Extract the JobId from a `{slug}-{JobId}` path segment.
     */
    public static function extractId(string $segment): string
    {
        return Str::afterLast($segment, '-');
    }
}
