# WorkBC.Indexers.Innovibe.V2

PHP 8.3 indexer that reads external (Innovibe) job postings from the
`"ImportedJobsWanted"` staging table and writes the corresponding documents into
Elasticsearch (`jobs_en`). Packaged as a Docker image and scheduled via a
Kubernetes CronJob.

This is **stage 2** of the external-jobs pipeline (the **indexer**). Stage 1 (the
**importer** that fetches the Innovibe API into `"ImportedJobsWanted"`) is
`WorkBC.Importers.Innovibe`. This project is a PHP rewrite of the legacy .NET
`WorkBC.Indexers.Wanted` + the JSON half of the shared
`WorkBC.ElasticSearch.Indexing` library — it removes the last .NET dependency in
the external-jobs indexing path.

Innovibe is the only source of external jobs and its staging payloads are always
JSON, so only the JSON mapping is ported (the legacy TalentNeuron XML branch is
intentionally dropped — there is no live XML data).

The Elasticsearch document shape is **functionally equivalent** to the .NET
indexer: PascalCase field names exactly as defined by
`resources/jobs_index.json.template`, with null-valued fields omitted (mirrors
Newtonsoft `NullValueHandling.Ignore`).

## Where it runs in production

| Resource | Identifier |
|---|---|
| Image | `${ECR}/jb-indexers-wanted:<sha>` (now built from this PHP project) |
| Built by | `.github/workflows/build*.yml` (compose service `indexers-innovibe-v2`) |
| Tagged by | `.github/workflows/tag*.yml` (test → prod promotion) |
| Wired by | `.github/workflows/deploy-*.{yml,yaml}` (`IMAGE7`) |
| Cron | `cronjob/jb-importer-indexer`, container `wanted-indexer` |

> The ECR repo / CronJob container keep the legacy **"wanted"** name to avoid
> touching the Kubernetes objects (which live outside this repo). Only the source
> project is renamed to "Innovibe" for clarity.

It is also baked into the shared `php-cli` shell image (`cli2` / `jb-cli-innovibe`)
at `/app/workbc-indexers-innovibe-v2`, alongside the federal indexer and the two
PHP importers, for manual runs via `kubectl exec`.

## Project Structure

```
WorkBC.Indexers.Innovibe.V2/
├── src/
│   ├── Config/AppConfig.php                # Env-var configuration (no geocoding/proxy).
│   ├── Elastic/ElasticClient.php           # Guzzle ES wrapper: PUT/DELETE docs, create/delete
│   │                                       #   index, close/open, scroll JobIds by source.
│   ├── Json/InnovibeDocumentMapper.php     # JSON → ES document (PascalCase, nulls omitted).
│   ├── Service/InnovibeIndexService.php    # Orchestrator: index loop, publishable gate,
│   │                                       #   FlushRejected, clear-flag, purge, debug.
│   ├── Service/IndexMaintenanceService.php # --reindex (drop+create+flag), --reopen.
│   └── index.php                           # CLI entry point.
├── resources/                              # ES index mapping + synonym templates (##SYNONYM##).
├── Dockerfile                              # Multi-stage Alpine build (PHP 8.3 + pdo_pgsql).
├── .env.example
├── composer.json                           # monolog, guzzle, vlucas/phpdotenv.
├── php.ini
└── README.md
```

## Index Flow

A normal run, mirroring the legacy `WorkBC.Indexers.Wanted/Program.cs`:

1. **Index** — `SELECT` every `"ImportedJobsWanted"` row where
   `"ReIndexNeeded" = TRUE AND "IsFederalOrWorkBc" = FALSE`; for each, build the
   document from the Innovibe JSON and `PUT {ELASTIC_URL}/jobs_en/_doc/{JobId}`.
   - **Publishable gate** — a job missing a Title, EmployerName, valid Noc2021, or
     City is *not* indexed. It is parked: upserted into `"ExpiredJobs"` and flipped
     `"IsActive" = FALSE` in `"Jobs"` so admin counts stop tracking it.
   - On a transport error a single retry runs after `RETRY_DELAY_MS`; a persistent
     ES failure stops the loop (it is almost certainly down).
2. **Purge** — runs on every non-`--reindex` run:
   - drain `"ExpiredJobs"` rows not yet `RemovedFromElasticsearch` (DELETE, mark);
   - sweep orphans — any non-federal doc in ES whose `JobId` is no longer in
     `"ImportedJobsWanted"` is deleted.

### Document construction

`InnovibeDocumentMapper` ports the JSON branch of the legacy
`XmlParsingServiceWanted`: BC-preferred location + city/region disambiguation;
salary annual conversion (HOUR×2080 / WEEK×52 / MONTH×12) with min–max range
summary; `employmentType` → HoursOfWork / PeriodOfEmployment; 3-strategy education
mapping; NOC 2021 from the highest-scored `nocMatches` (validated against
`"NocCodes2021"`); title cleanup (strip U+200B, all-caps → lowercase,
`\bpt\b`→PT / `\bft\b`→FT); `ExternalSource`; and `SalarySort`.

## Database (existing schema — no migrations owned here)

| Table | Purpose |
|---|---|
| `"ImportedJobsWanted"` | Staging — raw Innovibe JSON; `ReIndexNeeded` drives the loop; `IsFederalOrWorkBc=FALSE` selects Innovibe rows. |
| `"ExpiredJobs"` | Archive — drained to remove ES documents; also receives publishable-gate rejects. |
| `"Jobs"` | Reject path flips `IsActive=FALSE`; `--debug` diffs ES vs SQL. |
| `"NocCodes2021"` | NOC validation + 2021 group titles. |
| `"Locations"` / `"Regions"` | City → region / duplicate-city disambiguation. |

This indexer does **not** create or alter tables.

## Configuration

| Var | Default | Notes |
|---|---|---|
| `DB_HOST`,`DB_PORT`,`DB_NAME`,`DB_USER`,`DB_PASSWORD` | postgres/5432/jobboard | Postgres connection. |
| `ELASTIC_URL` | `http://elasticsearch:9200` | Elasticsearch / OpenSearch root. |
| `ELASTIC_USER`,`ELASTIC_PASSWORD` | — | Basic auth (leave empty for none). |
| `INDEX_EN`,`INDEX_FR` | `jobs_en` / `jobs_fr` | Documents go to `INDEX_EN`; `INDEX_FR` is only recreated by `--reindex`. |
| `WANTED_JOB_EXPIRY_DAYS` | `90` | Days added to the refreshed date to compute `ExpireDate`. |
| `HTTP_TIMEOUT` | `30` | Per-request timeout (s). |
| `RETRY_DELAY_MS` | `15000` | Pause before the single retry on an ES transport error. |
| `LOG_LEVEL` | `INFO` | DEBUG/INFO/WARNING/ERROR/CRITICAL. |

## CLI Flags

| Flag | Purpose |
|---|---|
| *(none)* | Index pending jobs, then purge. The daily cron path. |
| `-r` / `--reindex` | Drop + recreate `jobs_en`/`jobs_fr`, flag all staging rows, then index (no purge). |
| `-o` / `--reopen` | Close + reopen both indexes to reload the synonym file. |
| `-n` / `--noreindex` | Skip the indexing loop (purge still runs). |
| `-d` / `--debug` | Print the active-in-ES vs active-in-`Jobs` JobId diff and exit. |

## Build & Run

```bash
# From src/
docker compose build indexers-innovibe-v2
docker compose run --rm indexers-innovibe-v2 -r   # recreate + reindex all
docker compose run --rm indexers-innovibe-v2      # index pending + purge

# Manual run from the cli2 / php-cli pod
kubectl exec -it <php-cli-pod> -- bash
cd /app/workbc-indexers-innovibe-v2
php src/index.php            # index pending + purge
php src/index.php -r         # recreate + reindex all
php src/index.php -d         # debug diff
```
