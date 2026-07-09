<?php

declare(strict_types=1);

namespace WorkBC\FederalJobIndexer\Geocoding;

use GuzzleHttp\Client;
use GuzzleHttp\Exception\GuzzleException;
use Monolog\Logger;
use PDO;
use WorkBC\FederalJobIndexer\Config\AppConfig;

/**
 * Resolves a location string (typically "{postalCode}, CANADA") to a
 * cached geocode row, calling the Google Maps geocoding API on a cache miss
 * and writing the result back to "GeocodedLocationCache".
 *
 * Faithful port of WorkBC.Shared.Services.GeocodingService:
 *   - GetLocation: return the cache row if "Name" already exists.
 *   - CreateLocation: Google geocode → parse city/province/lat/lng, and
 *     (only when the address contains a digit) a second fr-language lookup
 *     for FrenchCity, then INSERT and return the new row.
 *
 * Used exclusively for VIRTUAL federal jobs. Honours PROXY_* settings for the
 * outbound Google call (the C# GeocodingService routes through the same
 * forward proxy when ProxySettings.UseProxy is true).
 *
 * Returned rows use the same keys as the "GeocodedLocationCache" columns:
 * Name, Latitude, Longitude, City, FrenchCity, Province.
 */
final class GeocodingService
{
    private const GEOCODE_URL = 'https://maps.googleapis.com/maps/api/geocode/xml';

    private Client $http;
    private \PDOStatement $selectStmt;
    private ?\PDOStatement $insertStmt = null;

    public function __construct(
        private readonly PDO $db,
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {
        $options = [
            'timeout' => $config->httpTimeout,
            'http_errors' => false,
        ];
        if ($config->proxyUse && $config->proxyHost !== '') {
            $options['proxy'] = "http://{$config->proxyHost}:{$config->proxyPort}";
            if ($config->proxyIgnoreSslErrors) {
                $options['verify'] = false;
            }
        }
        $this->http = new Client($options);

        $this->selectStmt = $this->db->prepare(
            'SELECT "Name","Latitude","Longitude","City","FrenchCity","Province"
             FROM "GeocodedLocationCache" WHERE "Name" = ? LIMIT 1'
        );
    }

    /**
     * @return array{Name:string,Latitude:?string,Longitude:?string,City:?string,FrenchCity:?string,Province:?string}|null
     */
    public function getLocation(string $location): ?array
    {
        $this->selectStmt->execute([$location]);
        $row = $this->selectStmt->fetch();
        if ($row !== false) {
            return $row;
        }

        return $this->createLocation($location);
    }

    /**
     * @return array{Name:string,Latitude:?string,Longitude:?string,City:?string,FrenchCity:?string,Province:?string}|null
     */
    private function createLocation(string $location): ?array
    {
        // No key configured → behave like a permanent cache miss (the caller
        // then falls back to the postal-code / province label).
        if ($this->config->googleMapsApiKey === '') {
            return null;
        }

        $geo = $this->fetchGoogleMapsLocation($location);
        if ($geo === null || $geo['lat'] === null || $geo['lon'] === null) {
            return null;
        }

        $row = [
            'Name' => $location,
            'Latitude' => $geo['lat'],
            'Longitude' => $geo['lon'],
            'City' => $geo['city'],
            'FrenchCity' => $geo['frenchCity'],
            'Province' => $geo['province'],
        ];

        $this->saveLocation($row);

        return $row;
    }

    private function saveLocation(array $row): void
    {
        if ($this->insertStmt === null) {
            // Insert only if this Name isn't already cached. We use
            // INSERT ... SELECT ... WHERE NOT EXISTS rather than ON CONFLICT
            // because the production "GeocodedLocationCache" table does not
            // carry a unique constraint on "Name" (ON CONFLICT requires one and
            // fails with SQLSTATE 42P10). This form is safe whether or not the
            // constraint exists and still dedupes by Name.
            $this->insertStmt = $this->db->prepare(
                'INSERT INTO "GeocodedLocationCache"
                    ("DateGeocoded","Name","Latitude","Longitude","City","FrenchCity","Province","IsPermanent")
                 SELECT NOW(), ?, ?, ?, ?, ?, ?, FALSE
                 WHERE NOT EXISTS (
                     SELECT 1 FROM "GeocodedLocationCache" WHERE "Name" = ?
                 )'
            );
        }

        try {
            $this->insertStmt->execute([
                $row['Name'],
                $row['Latitude'],
                $row['Longitude'],
                $this->truncate($row['City'], 80),
                $this->truncate($row['FrenchCity'], 80),
                $this->truncate($row['Province'], 2),
                $row['Name'],
            ]);
        } catch (\Throwable $e) {
            $this->log->warning("Failed to cache geocode for '{$row['Name']}': {$e->getMessage()}");
        }
    }

    /**
     * @return array{lat:?string,lon:?string,city:?string,frenchCity:?string,province:?string}|null
     */
    private function fetchGoogleMapsLocation(string $address): ?array
    {
        $xml = $this->getGeocodeXml($address, false);
        if ($xml === null) {
            return null;
        }

        $result = $xml->result ?? null;
        if ($result === null || !isset($result[0])) {
            return [
                'lat' => null, 'lon' => null, 'city' => null, 'frenchCity' => null, 'province' => null,
            ];
        }
        $result = $result[0];

        $city = $this->getCity($result);
        $province = $this->getAddressComponentShortName($result, 'administrative_area_level_1');

        $location = $result->geometry->location ?? null;
        $lat = $location !== null && isset($location->lat) ? (string) $location->lat : null;
        $lon = $location !== null && isset($location->lng) ? (string) $location->lng : null;

        $frenchCity = null;
        // Only look up the French city when the address contains a digit
        // (we only really use it for postal codes). Mirrors the C# guard.
        if (preg_match('/\d/', $address) === 1) {
            $frenchXml = $this->getGeocodeXml($address, true);
            if ($frenchXml !== null && isset($frenchXml->result[0])) {
                $frenchCity = $this->getCity($frenchXml->result[0]);
            }
        }

        return [
            'lat' => $lat,
            'lon' => $lon,
            'city' => $city,
            'frenchCity' => $frenchCity,
            'province' => $province,
        ];
    }

    private function getGeocodeXml(string $address, bool $isFrench): ?\SimpleXMLElement
    {
        $url = self::GEOCODE_URL . '?address=' . rawurlencode($address)
            . '&key=' . rawurlencode($this->config->googleMapsApiKey);
        if ($isFrench) {
            $url .= '&language=fr';
        }

        try {
            $response = $this->http->get($url);
            if ($response->getStatusCode() < 200 || $response->getStatusCode() >= 300) {
                return null;
            }
            $previous = libxml_use_internal_errors(true);
            $xml = simplexml_load_string((string) $response->getBody());
            libxml_clear_errors();
            libxml_use_internal_errors($previous);
            return $xml === false ? null : $xml;
        } catch (GuzzleException $e) {
            $this->log->warning("Google Maps geocode failed for '{$address}': {$e->getMessage()}");
            return null;
        }
    }

    /**
     * Reads the "locality" short_name, falling back to "neighborhood".
     * Mirrors GeocodingService.GetCity (which uses SingleOrDefault — i.e.
     * only resolves when exactly one matching component exists).
     */
    private function getCity(\SimpleXMLElement $result): ?string
    {
        return $this->getAddressComponentShortName($result, 'locality')
            ?? $this->getAddressComponentShortName($result, 'neighborhood');
    }

    private function getAddressComponentShortName(\SimpleXMLElement $result, string $type): ?string
    {
        $matches = [];
        foreach ($result->address_component as $component) {
            foreach ($component->type as $componentType) {
                if ((string) $componentType === $type) {
                    $matches[] = (string) $component->short_name;
                    break;
                }
            }
        }

        // SingleOrDefault semantics: only return when exactly one match.
        return count($matches) === 1 ? $matches[0] : null;
    }

    private function truncate(?string $value, int $max): ?string
    {
        if ($value === null) {
            return null;
        }
        return mb_strlen($value) <= $max ? $value : mb_substr($value, 0, $max);
    }
}
