<?php

return [

    /*
    |--------------------------------------------------------------------------
    | OpenSearch connection
    |--------------------------------------------------------------------------
    |
    | OpenSearch is a DERIVED read model (ADR-001). The existing PHP indexer
    | container writes the jobs_en / jobs_fr indexes; this application only
    | READS from them and never creates, writes, or recomputes their contents
    | (Rule B). Locally the cluster runs with the security plugin disabled, so
    | the scheme is http and no credentials are required.
    |
    */

    'host' => env('OPENSEARCH_HOST', 'opensearch'),
    'port' => (int) env('OPENSEARCH_PORT', 9200),
    'scheme' => env('OPENSEARCH_SCHEME', 'http'),
    'username' => env('OPENSEARCH_USERNAME', ''),
    'password' => env('OPENSEARCH_PASSWORD', ''),

    // TLS certificate verification (production HTTPS clusters). False locally.
    'ssl_verify' => (bool) env('OPENSEARCH_SSL_VERIFY', false),

    /*
    | The two derived indexes the app reads. Names come from env so a local
    | cluster can be pointed at differently-named restored indexes if needed.
    */
    'indexes' => [
        'en' => env('OPENSEARCH_INDEX_EN', 'jobs_en'),
        'fr' => env('OPENSEARCH_INDEX_FR', 'jobs_fr'),
    ],

];
