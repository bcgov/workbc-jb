<?xml version="1.0" encoding="UTF-8"?>
<sitemapindex xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
@foreach ($shards as $shard)
    <sitemap>
        <loc>{{ route('sitemap.shard', ['language' => $shard]) }}</loc>
    </sitemap>
@endforeach
</sitemapindex>
