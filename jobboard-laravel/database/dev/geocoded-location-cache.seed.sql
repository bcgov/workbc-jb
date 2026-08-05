-- DEV-ONLY seed for GeocodedLocationCache (SRCH-2 radius search).
-- The search geocoder resolves a typed city/postal -> coordinates by reading this DB
-- table first (Name = lookup key), Google Maps fallback (see GeocodingService.cs).
-- Job search itself stays in OpenSearch (geo_distance on LocationGeo).
--
-- IMPORTANT: Name must match the exact key the search builds — city radius searches
-- look up "{City}, BC, CANADA" (JobSearchQuery.cs:546 / JobSearchQuery.php), NOT the bare
-- city name. Coordinates match the fake-data cities from `php artisan dev:index-jobs`.
-- Id is GENERATED ALWAYS AS IDENTITY (omitted). Re-run after `docker compose down -v`:
--   docker exec -i jobboard-laravel-pgsql-1 psql -U sail -d laravel < database/dev/geocoded-location-cache.seed.sql

DELETE FROM public."GeocodedLocationCache" WHERE "Name" LIKE '%, BC, CANADA';

INSERT INTO public."GeocodedLocationCache" ("Name","Latitude","Longitude","DateGeocoded","IsPermanent","City","Province","FrenchCity") VALUES
('Vancouver, BC, CANADA','49.2827','-123.1207',now(),true,'Vancouver','BC',NULL),
('Surrey, BC, CANADA','49.1913','-122.8490',now(),true,'Surrey','BC',NULL),
('Burnaby, BC, CANADA','49.2488','-122.9805',now(),true,'Burnaby','BC',NULL),
('Richmond, BC, CANADA','49.1666','-123.1336',now(),true,'Richmond','BC',NULL),
('Coquitlam, BC, CANADA','49.2838','-122.7932',now(),true,'Coquitlam','BC',NULL),
('Abbotsford, BC, CANADA','49.0504','-122.3045',now(),true,'Abbotsford','BC',NULL),
('Chilliwack, BC, CANADA','49.1579','-121.9514',now(),true,'Chilliwack','BC',NULL),
('Victoria, BC, CANADA','48.4284','-123.3656',now(),true,'Victoria','BC',NULL),
('Nanaimo, BC, CANADA','49.1659','-123.9401',now(),true,'Nanaimo','BC',NULL),
('Campbell River, BC, CANADA','50.0244','-125.2475',now(),true,'Campbell River','BC',NULL),
('Kelowna, BC, CANADA','49.8880','-119.4960',now(),true,'Kelowna','BC',NULL),
('Kamloops, BC, CANADA','50.6745','-120.3273',now(),true,'Kamloops','BC',NULL),
('Vernon, BC, CANADA','50.2670','-119.2720',now(),true,'Vernon','BC',NULL),
('Prince George, BC, CANADA','53.9171','-122.7497',now(),true,'Prince George','BC',NULL),
('Prince Rupert, BC, CANADA','54.3150','-130.3208',now(),true,'Prince Rupert','BC',NULL),
('Fort St. John, BC, CANADA','56.2465','-120.8476',now(),true,'Fort St. John','BC',NULL),
('Cranbrook, BC, CANADA','49.5122','-115.7686',now(),true,'Cranbrook','BC',NULL);
