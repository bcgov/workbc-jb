<?php

return [

    /*
    |--------------------------------------------------------------------------
    | Build provenance
    |--------------------------------------------------------------------------
    |
    | Baked into the image by the Dockerfile (ARG COMMIT_SHA / RUN_NUMBER /
    | BUILD_DATE -> ENV APP_*), so a running container can report which build it
    | came from. Surfaced by GET /api/SystemSettings/BuildInfo.
    |
    | These MUST be read here rather than via env() at the call site: the
    | entrypoint runs `config:cache`, after which env() returns null everywhere
    | outside config files.
    |
    */

    'sha' => env('APP_COMMIT_SHA', 'unknown'),

    'run_number' => env('APP_RUN_NUMBER', 'unknown'),

    'build_date' => env('APP_BUILD_DATE', 'unknown'),

];
