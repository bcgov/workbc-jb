<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
@foreach ($jobs as $job)
    <url>
        <loc>{{ route('jobs.show', ['job' => \App\Support\JobSlug::path($job['jobId'], $job['title'])]) }}</loc>
@if ($job['lastUpdated'] !== null)
        <lastmod>{{ \Illuminate\Support\Carbon::parse($job['lastUpdated'])->toDateString() }}</lastmod>
@endif
    </url>
@endforeach
</urlset>
