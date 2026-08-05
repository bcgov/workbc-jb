#!/bin/sh
set -e

# Runs before supervisord starts nginx/php-fpm.
#
# The important thing here is WHEN config is cached. `php artisan config:cache`
# freezes whatever env is visible at the moment it runs — do it during the image
# build and the container permanently ignores its ConfigMap/Secret, which is the
# classic Laravel-in-Docker bug. So it happens here, after Kubernetes has
# injected the environment.

cd /var/www/html

# Fail loudly and immediately rather than serving 500s: without APP_KEY Laravel
# cannot decrypt sessions or the CSRF cookie, and the failure it produces at
# request time is far less obvious than this message.
if [ -z "${APP_KEY:-}" ]; then
    echo "FATAL: APP_KEY is not set. Generate one with 'php artisan key:generate --show'" >&2
    echo "       and add it to the container's Secret." >&2
    exit 1
fi

# Writable paths. Sessions, cache and queue are all Redis, so compiled Blade
# views and the framework cache are the only local state.
mkdir -p storage/framework/cache/data storage/framework/sessions \
         storage/framework/views storage/logs bootstrap/cache
chmod -R 775 storage bootstrap/cache

# Discard anything cached at build time, then rebuild against the live
# environment. Ordering matters: clear before cache.
php artisan config:clear >/dev/null 2>&1 || true
php artisan config:cache
php artisan route:cache
php artisan view:cache

# NOTE: migrations are deliberately NOT run here.
#
# The stock Laravel migrations would create eight tables in the existing WorkBC
# database (users, sessions, cache, jobs, job_batches, ...) — most redundant
# because this app uses Redis, and `users` would sit confusingly beside
# AspNetUsers. Search is anonymous and needs none of them. If a future story
# needs password_reset_tokens or failed_jobs, create those deliberately rather
# than turning on `migrate --force` here.

echo "Job board web container ready (commit ${APP_COMMIT_SHA:-unknown}, built ${APP_BUILD_DATE:-unknown})"

exec "$@"
