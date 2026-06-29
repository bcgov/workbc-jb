# WorkBC.Indexers.Federal.V2

PHP 8.3 indexer that reads federal job postings from the `"ImportedJobsFederal"`
staging table and writes the corresponding documents into Elasticsearch
(`jobs_en` / `jobs_fr`). Packaged as a Docker image and scheduled via a
Kubernetes CronJob.

This is the **stage 2** of the federal pipeline (the **indexer**). Stage 1 (the
**importer** that fetches the jobbank.gc.ca XML feed into `"ImportedJobsFederal"`)
is `WorkBC.Importers.Federal.V2`. This project is a PHP rewrite of the legacy
.NET `WorkBC.Indexers.Federal` + the federal half of the shared
`WorkBC.ElasticSearch.Indexing` library — it removes the last .NET dependency in
the federal indexing path.

The Elasticsearch document shape is **functionally equivalent** to the .NET
indexer: PascalCase field names exactly as defined by `resources/jobs_index.json`,
with null-valued fields omitted (mirrors Newtonsoft `NullValueHandling.Ignore`).

## Where it runs in production

| Resource | Identifier |
|---|---|
| Image | `${ECR}/jb-indexers-federal:<sha>` (now built from this PHP project) |
| Built by | `.github/workflows/build*.yml` (compose service `indexers-federal-v2`) |
| Tagged by | `.github/workflows/tag*.yml` (test → prod promotion) |
| Wired by | `.github/workflows/deploy-*.{yml,yaml}` (`IMAGE8`) |
| Cron | `cronjob/jb-importer-indexer`, container `federal-indexer` |

It is also baked into the shared `php-cli` shell image (`cli2` /
`jb-cli-innovibe`) at `/app/workbc-indexers-federal-v2`, alongside the two PHP
importers, for manual runs via `kubectl exec`.

## Project Structure

```
WorkBC.Indexers.Federal.V2/
├── src/
│   ├── Config/AppConfig.php               # Strongly-typed env-var configuration.
│   ├── Elastic/ElasticClient.php          # Guzzle ES wrapper: PUT/DELETE docs, create/delete
│   │                                      #   index, close/open, scroll federal JobIds.
│   ├── Geocoding/GeocodingService.php     # GeocodedLocationCache lookup + Google Maps fallback
│   │                                      #   (+ cache write-back). Virtual jobs only.
│   ├── Xml/FederalDocumentMapper.php      # XML → full ES document (PascalCase, nulls omitted).
│   ├── Service/FederalIndexService.php    # Orchestrator: index loop, clear-flag, purge, debug.
│   ├── Service/IndexMaintenanceService.php# --reindex (drop+create+flag), --reopen.
│   └── index.php                          # CLI entry point.
├── resources/
│   ├── jobs_index.json                    # ES index mapping (the output contract). ##SYNONYM## placeholder.
│   ├── synonym_file.json                  # Production synonyms.
│   └── synonym_predefined.json            # Inline synonyms (test).
├── Dockerfile                             # Multi-stage Alpine build (PHP 8.3 + pdo_pgsql + dom + simplexml).
│                                          # Runs ENTRYPOINT ["php", "src/index.php"].
├── .env.example                           # Template — copy to .env.
├── composer.json                          # monolog, guzzle, vlucas/phpdotenv.
├── php.ini                                # 1024M memory_limit (full reindex).
└── README.md
```

## Index Flow

A normal run does two things, mirroring the legacy `Program.cs`:

1. **Index** — `SELECT` every `"ImportedJobsFederal"` row where
   `"ReIndexNeeded" = TRUE`; for each, build an **English** document from
   `JobPostEnglish` and a **French** document from `JobPostFrench`, then
   `PUT {ELASTIC_URL}/{index}/_doc/{JobId}` into `jobs_en` and `jobs_fr`.
   A missing/empty French translation is tolerated (the English document still
   indexes). On a transport error a single retry runs after `RETRY_DELAY_MS`.
2. **Purge** — remove documents that should no longer be in the index:
   - drain `"ExpiredJobs"` rows not yet `RemovedFromElasticsearch` (DELETE from
     both indexes, mark removed);
   - sweep orphans — any federal document in ES whose `JobId` is no longer in
     `"ImportedJobsFederal"` is deleted from both indexes.

### Document construction

`FederalDocumentMapper` ports the legacy `XmlParsingServiceFederal` +
`XmlParsingServiceBase` + `FederalXmlLocations` + `XmlManualOverRides`:
NOC validation and NOC-387 consolidation (00011–00015 → 00018); salary
calculation with the `shared.settings.minimumWage` filter; BC-only city
dedupe + alphabetical sort; duplicate-city disambiguation labels
(e.g. *Mill Bay - Vancouver Island / Coast*); region lookup; `Location`/
`LocationGeo` (BC bounding-box filtered geo_points); skill categories +
benefit normalization (`SalaryConditions`); workplace-type EN/FR strings;
hours/period/employment-term labels; all Apply* blocks; education band;
`NocGroup` 2021 title lookup; salary description/summary; `SalarySort`; and
the virtual-job city label resolved via `GeocodingService`.

> **Known parity quirk (intentional):** after indexing, the `ReIndexNeeded`
> flag is cleared with `UPDATE "ImportedJobsWanted"` — **not**
> `"ImportedJobsFederal"`. This reproduces a bug in the .NET
> `FederalIndexService.UpdateIds()` byte-for-byte; in practice the federal flag
> is reset only by a full `--reindex`. Do not "fix" this without an explicit
> decision, since the observable behaviour must stay identical to the .NET
> implementation it replaces.

## Database (existing schema — no migrations owned here)

| Table | Purpose |
|---|---|
| `"ImportedJobsFederal"` | Staging — raw EN/FR XML; `ReIndexNeeded` drives the index loop. |
| `"ExpiredJobs"` | Archive — drained to remove ES documents. |
| `"Jobs"` | Read-only here (used by `--debug` to diff ES vs SQL). |
| `"NocCodes"` / `"NocCodes2021"` | NOC validation + 2021 group titles. |
| `"Locations"` | City → region / duplicate-city disambiguation. |
| `"GeocodedLocationCache"` | Postal-code → City/Province for virtual jobs (read + write-back). |
| `"SystemSettings"` | `shared.settings.minimumWage` salary filter. |

This indexer does **not** create or alter tables.

## Configuration

| Var | Default | Notes |
|---|---|---|
| `DB_HOST`,`DB_PORT`,`DB_NAME`,`DB_USER`,`DB_PASSWORD` | postgres/5432/jobboard | Postgres connection. |
| `ELASTIC_URL` | `http://elasticsearch:9200` | Elasticsearch / OpenSearch root. |
| `ELASTIC_USER`,`ELASTIC_PASSWORD` | — | Basic auth (leave empty for none). |
| `INDEX_EN`,`INDEX_FR` | `jobs_en` / `jobs_fr` | Index names. |
| `GOOGLE_MAPS_API_KEY` | — | Geocoding for virtual jobs; empty = cache-only fallback. |
| `PROXY_USE`,`PROXY_HOST`,`PROXY_PORT`,`PROXY_IGNORE_SSL_ERRORS` | false | Forward proxy for the Google call (ES is always direct). |
| `HTTP_TIMEOUT` | `30` | Per-request timeout (s). |
| `RETRY_DELAY_MS` | `15000` | Pause before the single retry on an ES transport error. |
| `LOG_LEVEL` | `INFO` | DEBUG/INFO/WARNING/ERROR/CRITICAL. |

## CLI Flags

| Flag | Purpose |
|---|---|
| *(none)* | Index pending jobs, then purge. The daily cron path. |
| `-r` / `--reindex` | Drop + recreate `jobs_en`/`jobs_fr` from `jobs_index.json`, flag all staging rows, then index (no purge). |
| `-o` / `--reopen` | Close + reopen both indexes to reload the synonym file. |
| `-n` / `--noreindex` | Run maintenance only; skip the indexing loop (and purge). |
| `-d` / `--debug` | Print the active-in-ES vs active-in-`Jobs` JobId diff and exit. |

## Build & Run

### Docker Compose (integrated with the WorkBC dev stack)

From the `src/` directory:

```bash
docker compose build indexers-federal-v2
docker compose run --rm indexers-federal-v2 -r   # recreate + reindex all
docker compose run --rm indexers-federal-v2      # index pending + purge
```

The compose service `indexers-federal-v2` has `scale: 0` — built but not started
by `docker compose up`. Use `docker compose run` for one-off invocations, exactly
like the cron does in production.

### Production shell — kubectl exec into the `php-cli` pod

```bash
kubectl exec -it <php-cli-pod-name> -- bash
cd /app/workbc-indexers-federal-v2
php src/index.php            # index pending + purge
php src/index.php -r         # recreate + reindex all
php src/index.php -d         # debug diff
```

### Local PHP (development only)

```bash
composer install
cp .env.example .env
# Fill in DB_USER, DB_PASSWORD, ELASTIC_URL
php src/index.php -d
```

## Verifying a run

```sql
-- pending re-index work
SELECT COUNT(*) FILTER (WHERE "ReIndexNeeded") AS pending,
       COUNT(*) AS staging
FROM "ImportedJobsFederal";
```

```bash
# active federal doc count in Elasticsearch
curl -s "$ELASTIC_URL/jobs_en/_count" -H 'Content-Type: application/json' \
  -d '{"query":{"term":{"IsFederalJob":true}}}'
```
