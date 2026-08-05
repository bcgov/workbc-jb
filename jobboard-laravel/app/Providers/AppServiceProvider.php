<?php

namespace App\Providers;

use App\Auth\JobSeekerUserProvider;
use App\Auth\LegacyPasswordHasher;
use Illuminate\Cache\RateLimiting\Limit;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\RateLimiter;
use Illuminate\Support\ServiceProvider;
use OpenSearch\Client;
use OpenSearch\ClientBuilder;

class AppServiceProvider extends ServiceProvider
{
    /**
     * Register any application services.
     */
    public function register(): void
    {
        // OpenSearch client — the app READS the derived jobs_en/jobs_fr indexes only
        // (ADR-001, Rule B). One shared, lazily-built connection.
        $this->app->singleton(Client::class, function () {
            $config = config('opensearch');

            $builder = ClientBuilder::create()
                ->setHosts([sprintf('%s://%s:%d', $config['scheme'], $config['host'], $config['port'])])
                ->setSSLVerification($config['ssl_verify']);

            if ($config['username'] !== '') {
                $builder->setBasicAuthentication($config['username'], $config['password']);
            }

            return $builder->build();
        });

        // Geocoding goes through the Geocoder adapter (constraint #4), never inline.
        // The concrete adapter resolves cache-first from GeocodedLocationCache.
        $this->app->bind(\App\Search\Contracts\Geocoder::class, \App\Services\Integration\CachedGeocoder::class);
    }

    /**
     * Bootstrap any application services.
     */
    public function boot(): void
    {
        Auth::provider('jobseeker', function ($app, array $config) {
            return new JobSeekerUserProvider(
                $app['hash'],
                $config['model'],
                $app->make(LegacyPasswordHasher::class),
            );
        });

        $emailKey = static function (Request $request): string {
            $email = mb_strtoupper(trim((string) $request->input('email', '')), 'UTF-8');

            return $email !== '' ? $email : 'anonymous-email';
        };

        $tokenKey = static function (Request $request): string {
            $token = trim((string) $request->input('token', ''));

            return $token !== '' ? hash('sha256', $token) : 'anonymous-token';
        };

        RateLimiter::for('job-seeker-login', static function (Request $request) use ($emailKey): array {
            $limit = (int) config('auth.job_seeker_rate_limits.login.per_minute', 5);

            return [
                Limit::perMinute($limit)->by('job-seeker-login:ip:'.$request->ip()),
                Limit::perMinute($limit)->by('job-seeker-login:email:'.$emailKey($request)),
            ];
        });

        RateLimiter::for('job-seeker-register', static function (Request $request) use ($emailKey): array {
            $limit = (int) config('auth.job_seeker_rate_limits.register.per_minute', 5);

            return [
                Limit::perMinute($limit)->by('job-seeker-register:ip:'.$request->ip()),
                Limit::perMinute($limit)->by('job-seeker-register:email:'.$emailKey($request)),
            ];
        });

        RateLimiter::for('job-seeker-forgot-password', static function (Request $request) use ($emailKey): array {
            $limit = (int) config('auth.job_seeker_rate_limits.forgot_password.per_minute', 5);

            return [
                Limit::perMinute($limit)->by('job-seeker-forgot-password:ip:'.$request->ip()),
                Limit::perMinute($limit)->by('job-seeker-forgot-password:email:'.$emailKey($request)),
            ];
        });

        RateLimiter::for('job-seeker-reset-password', static function (Request $request) use ($emailKey, $tokenKey): array {
            $limit = (int) config('auth.job_seeker_rate_limits.reset_password.per_minute', 5);

            return [
                Limit::perMinute($limit)->by('job-seeker-reset-password:ip:'.$request->ip()),
                Limit::perMinute($limit)->by('job-seeker-reset-password:email:'.$emailKey($request)),
                Limit::perMinute($limit)->by('job-seeker-reset-password:token:'.$tokenKey($request)),
            ];
        });
    }
}
