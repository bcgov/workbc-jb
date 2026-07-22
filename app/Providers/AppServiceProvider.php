<?php

namespace App\Providers;

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
    }

    /**
     * Bootstrap any application services.
     */
    public function boot(): void
    {
        //
    }
}
