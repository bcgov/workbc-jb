<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Search;

use GuzzleHttp\Client;
use Monolog\Logger;
use WorkBC\JobAlertNotifier\Config\AppConfig;

/**
 * Port of WorkBC.Shared.Services.GeocodingService.
 *
 * Looks a location key (e.g. "V6B4N7, CANADA" or "Surrey, BC, CANADA") up in
 * the GeocodedLocationCache table; on a miss it geocodes via the Google Maps
 * XML API and stores the result. Latitude/Longitude are varchar columns and
 * are concatenated verbatim into the ES query, exactly like the C#.
 */
final class GeocodingService implements GeocodingInterface
{
    private Client $http;

    public function __construct(
        private readonly \PDO $pdo,
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {
        $options = [
            'timeout' => $config->httpTimeout,
            'http_errors' => false,
        ];
        if (($proxy = $config->outboundProxy()) !== null) {
            $options['proxy'] = $proxy;
        }
        if ($config->proxyIgnoreSslErrors) {
            $options['verify'] = false;
        }
        $this->http = new Client($options);
    }

    /**
     * @return array{Latitude: string, Longitude: string}|null
     */
    public function getLocation(string $location): ?array
    {
        $stmt = $this->pdo->prepare(
            'SELECT "Latitude", "Longitude" FROM "GeocodedLocationCache" WHERE "Name" = :name LIMIT 1'
        );
        $stmt->execute([':name' => $location]);
        $row = $stmt->fetch();

        if ($row !== false) {
            return ['Latitude' => (string) $row['Latitude'], 'Longitude' => (string) $row['Longitude']];
        }

        return $this->createLocation($location);
    }

    /**
     * @return array{Latitude: string, Longitude: string}|null
     */
    private function createLocation(string $location): ?array
    {
        $geo = $this->getGoogleMapsLocation($location);

        if ($geo['lat'] !== null && $geo['lon'] !== null) {
            $stmt = $this->pdo->prepare(
                'INSERT INTO "GeocodedLocationCache"
                    ("Name", "Latitude", "Longitude", "City", "FrenchCity", "Province", "DateGeocoded", "IsPermanent")
                 VALUES (:name, :lat, :lon, :city, :frenchCity, :province, NOW(), FALSE)'
            );
            $stmt->execute([
                ':name' => $location,
                ':lat' => $geo['lat'],
                ':lon' => $geo['lon'],
                ':city' => $geo['city'],
                ':frenchCity' => $geo['frenchCity'],
                ':province' => $geo['province'],
            ]);

            return ['Latitude' => $geo['lat'], 'Longitude' => $geo['lon']];
        }

        return null;
    }

    /**
     * @return array{lat: ?string, lon: ?string, city: ?string, frenchCity: ?string, province: ?string}
     */
    private function getGoogleMapsLocation(string $address): array
    {
        $none = ['lat' => null, 'lon' => null, 'city' => null, 'frenchCity' => null, 'province' => null];

        try {
            $requestUri = 'https://maps.googleapis.com/maps/api/geocode/xml?address='
                . rawurlencode($address) . '&key=' . $this->config->googleMapsApiKey;

            $xdoc = $this->getWebResponse($requestUri);
            $result = $xdoc?->result;

            if ($result === null || count($result) === 0) {
                return $none;
            }
            $result = $result[0];

            $city = $this->getCity($result);
            $province = $this->getAddressComponent($result, 'administrative_area_level_1');

            $lat = isset($result->geometry->location->lat) ? (string) $result->geometry->location->lat : null;
            $lon = isset($result->geometry->location->lng) ? (string) $result->geometry->location->lng : null;

            $frenchCity = null;

            // only look up the French city if the address contains a number
            // (because we only really use it for postal codes)
            if (preg_match('/\d/', $address) === 1) {
                $frenchDoc = $this->getWebResponse($requestUri, true);
                $frenchResult = ($frenchDoc?->result ?? null);
                if ($frenchResult !== null && count($frenchResult) > 0) {
                    $frenchCity = $this->getCity($frenchResult[0]);
                }
            }

            return ['lat' => $lat, 'lon' => $lon, 'city' => $city, 'frenchCity' => $frenchCity, 'province' => $province];
        } catch (\Throwable $e) {
            $this->log->error('getGoogleMapsLocation(' . $address . ') : ' . $e->getMessage());
        }

        return $none;
    }

    private function getCity(\SimpleXMLElement $result): ?string
    {
        return $this->getAddressComponent($result, 'locality')
            ?? $this->getAddressComponent($result, 'neighborhood');
    }

    private function getAddressComponent(\SimpleXMLElement $result, string $type): ?string
    {
        foreach ($result->address_component as $component) {
            foreach ($component->type as $componentType) {
                if ((string) $componentType === $type) {
                    return isset($component->short_name) ? (string) $component->short_name : null;
                }
            }
        }
        return null;
    }

    private function getWebResponse(string $url, bool $isFrench = false): ?\SimpleXMLElement
    {
        try {
            if ($isFrench) {
                $url .= '&language=fr';
            }

            $response = $this->http->get($url);
            $body = (string) $response->getBody();

            $previous = libxml_use_internal_errors(true);
            $xml = simplexml_load_string($body);
            libxml_use_internal_errors($previous);

            return $xml === false ? null : $xml;
        } catch (\Throwable) {
            return null;
        }
    }
}
