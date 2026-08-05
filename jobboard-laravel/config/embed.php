<?php

return [

    /*
    |--------------------------------------------------------------------------
    | Drupal embed (ADR-006 / ADR-009)
    |--------------------------------------------------------------------------
    |
    | The WorkBC.ca parent page(s) permitted to frame this app, and to receive
    | the content-height messages the embed layout posts. Per-environment, from
    | the origin map in ADR-009:
    |
    |   prod   https://www.workbc.ca
    |   test   https://test.workbc.ca
    |   dev    https://dev2.workbc.ca
    |
    | These drive BOTH the `frame-ancestors` CSP (who may frame us) and the
    | `postMessage` target origins (who may receive our height). Never use `*`
    | for either: `frame-ancestors *` invites clickjacking, and a wildcard
    | postMessage target leaks page dimensions to any framing site.
    |
    | An empty list means "not embeddable" — frame-ancestors falls back to
    | 'none', which is the safe default for local dev and direct access.
    |
    */

    'parent_origins' => array_values(array_filter(
        array_map('trim', explode(',', (string) env('EMBED_PARENT_ORIGINS', ''))),
        static fn (string $origin): bool => $origin !== '',
    )),

];
