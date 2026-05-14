# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Tech stack

- **Backend:** .NET 6, C# (one PHP service: `WorkBC.Importers.Innovibe`)
- **Frontend:** Angular 12 (Node 14 — newer Node breaks the build) with ng-bootstrap, ng-select, FontAwesome
- **Data:** SQL Server in production/Visual Studio dev; PostgreSQL when running the Linux Docker Compose stack. Elasticsearch 7 (prod uses AWS OpenSearch — do **not** upgrade to ES 8). Redis for caching/sessions.
- **Tests:** xUnit (`WorkBC.Tests`)

## Common commands

All commands assume `cd src` unless stated otherwise.

### Build / run / test (.NET)
```bash
# Build entire solution
dotnet build WorkBC.JobBoard.sln

# Run unit + integration tests
dotnet test WorkBC.Tests/WorkBC.Tests.csproj -c Release

# Run a single test (xunit filter)
dotnet test WorkBC.Tests/WorkBC.Tests.csproj --filter "FullyQualifiedName~QueryParsingTests.MethodName"
```

### Angular (`src/WorkBC.Web/ClientApp`)
```bash
npm install
npm run watchAll      # watches jb-search, jb-account, jb-lib in parallel — required during dev
npm run buildJbSearch # production build of search SPA
npm run buildJbAccount
npm run buildJbLib
npm run lint
npm test
```
The Angular workspace contains three projects: `jb-search` and `jb-account` are deployable SPAs; `jb-lib` is a shared component library — build `jb-lib` before/with the apps that consume it.

### Admin web (`src/WorkBC.Admin`)
Uses webpack (not Angular). Run `npm install && npm run watch` from `src/WorkBC.Admin` while running the .NET project — without webpack, CSS/JS will be missing.

### EF Core migrations (SQL Server only)
```bash
# From src/
dotnet ef migrations add <Name> --project WorkBC.Data --startup-project WorkBC.Web --context JobBoardContext
dotnet ef database update         --project WorkBC.Data --startup-project WorkBC.Web --context JobBoardContext
```
Migrations were authored for MSSQL — the Linux/Postgres dev stack cannot rebuild the DB from migrations and instead restores from `src/scripts/jobboard-full.sql.gz`.

### Docker stacks
```bash
# Local dev infra only (Elasticsearch, Kibana, Postgres for SSOT) — pair with VS-hosted apps
docker-compose -f docker-compose.local-dev.yml up

# Full Linux stack (everything containerized, Postgres-backed)
alias docker-compose-jb='docker-compose -f docker-compose.yml -f docker-compose.linux-dev.yml'
docker-compose-jb up
# Web → http://localhost:8081  Admin → http://localhost:8080  Kibana → http://localhost:5601  ES → http://localhost:9200

# One-off scheduled-task runs against the running stack
docker-compose-jb exec dotnet-cli bash
# inside container:  cd /app/workbc-indexers-wanted && dotnet WorkBC.Indexers.Wanted.dll -r
```
Most service containers in `docker-compose.yml` use `scale: 0` — they are built but only run on demand via `docker-compose run` or through `dotnet-cli`.

## High-level architecture

The system is a **3-stage pipeline** plus a public/admin web layer. Understanding the pipeline is essential before changing any importer/indexer/search code.

```
External sources  ──►  Importers (SQL/Postgres staging + Jobs table)  ──►  Indexers (Elasticsearch)  ──►  Web/Admin (read-only ES queries)
```

### Stage 1 — Importers (write to SQL)
Console apps that pull jobs from external systems into staging tables (`ImportedJobsFederal`, `ImportedJobsWanted`) and the canonical `Jobs` table.

- `WorkBC.Importers.Federal` — XML feed from `jobbank.gc.ca` (federal IP-whitelisted; large initial run).
- `WorkBC.Importers.Innovibe` — **PHP** importer that fetches jobs from the Innovibe API. Writes to `ImportedJobsWanted` / `Jobs` / `ExpiredJobs` tables. Runs as a Kubernetes CronJob in production. Default mode imports yesterday+today; `--bulk` imports all. Expiry is sourced from Innovibe's `/api/v1/jobs/expired/ids` endpoint, not by date heuristics.

### Stage 2 — Indexers (write to Elasticsearch)
- `WorkBC.Indexers.Federal` / `WorkBC.Indexers.Wanted` — read from staging/Jobs and produce ES documents into `jobs_en` and `jobs_fr` indexes. Both accept `--reindex` / `-r` to recreate the index, and `--migrate` to run EF migrations (handy for debugging).
- `WorkBC.ElasticSearch.Indexing` is a shared library that contains XML parsing and the document-shaping logic used by both indexers — bug fixes to job parsing usually belong here, not in the per-source indexer.

### Stage 3 — Search/Web read path
- `WorkBC.ElasticSearch.Models` defines the strongly-typed ES document/request/response types and filter inputs (e.g. `JobSearchFilters`).
- `WorkBC.ElasticSearch.Search` builds the ES queries, including custom boosts (`Boosts/`) and synonyms (`Resources/`).
- `WorkBC.Web` (Angular SPAs + .NET Core API) is the public job board. The API controllers in `WorkBC.Web/Controllers` (e.g. `SearchController`, `CareerProfilesController`, `JobAlertsController`) consume `WorkBC.ElasticSearch.Search` to query ES and use `WorkBC.Data` for user/account state in SQL.
- `WorkBC.Admin` is the .NET MVC admin site for content management. Database migrations run automatically at admin startup.
- `WorkBC.Notifications.JobAlerts` is a console app sending the daily job-alert emails (uses ES for matching and SendGrid for delivery).

### Cross-cutting projects
- `WorkBC.Data` — EF Core models, `JobBoardContext`, all migrations. Shared by everything that touches SQL.
- `WorkBC.Shared` — config helpers, constants, extensions, used widely.
- `EFMigrationRunner` — standalone runner used by Docker / CI to apply migrations.
- `WorkBC.Tests` — xunit; integration tests use a Docker Compose file (`docker-compose.integration-tests.yml`) for ES.

### SSOT
The SSOT (Single Source Of Truth) Postgres service runs in `docker-compose.local-dev.yml` and is seeded from a dump in the [bcgov/workbc-ssot](https://github.com/bcgov/workbc-ssot) repo. The web app pulls NOC, career, and industry profile data from SSOT APIs at `localhost:3000` / `localhost:8888`.

## Configuration

Secrets are **not** in the repo. Two delivery mechanisms:
- Visual Studio: per-project User Secrets (`Manage User Secrets`). If app settings appear wrong or overridden, check User Secrets first — they take precedence over `appsettings.json`.
- Docker / Linux: `src/.env` file (template at `src/.env.example`). Connection strings use `__` for nested keys (e.g. `ConnectionStrings__DefaultConnection`).

Required values come from the project's secrets spreadsheet — coordinate with the team to obtain them.

## Database schema notes (Postgres, current)

The codebase still contains older models/migrations that assume `JobId` is `bigint` — **this is no longer true on the live Postgres schema**. Verify with `\d "Jobs"` before writing SQL.

- `JobId` is **`character varying(255)`** on every table that holds one: `Jobs`, `SavedJobs`, `ExpiredJobs`, `DeletedJobs`, `ImportedJobsFederal`, `ImportedJobsWanted`, `JobVersions`, `JobViews`. (The parent `JobIds.Id` is still `bigint` in some older dumps — don't trust it for type inference.)
- `JobSources` lookup: `1 = Federal`, `2 = Wanted` (legacy TalentNeuron **and** new Innovibe both live under `JobSourceId = 2`).
- **Distinguishing old TalentNeuron from new Innovibe rows** (both `JobSourceId=2`):
  - Old TN: pure numeric `JobId` strings, e.g. `6752029327`. Most have empty `Title`, `IsActive = false`, no `OriginalSource`.
  - New Innovibe: alphanumeric cuid-like `JobId` strings, e.g. `cmngcwkx000b414hoydcgrydd`. Proper `Title`, `IsActive = true`.
  - Regex predicate that filters TN only: `"JobId" ~ '^[0-9]+$'` (varchar column → operator works directly, no cast).
  - Alternate predicate: `"JobId" NOT IN (SELECT "JobId" FROM "ImportedJobsWanted")` — Innovibe is the only thing writing to that staging table.
- Innovibe importer source of truth: `src/WorkBC.Importers.Innovibe/src/Service/JobImportService.php` — id comes from the API as `(string) $job['id']` (line 101) and is written verbatim.

### Local Postgres Docker

Two databases coexist in the local container `job-importer-postgres-1` (image `postgres:16-alpine`, creds `workbc` / `workbc`, port 5432):

- **`devworkbc`** — current schema (varchar `JobId`). Use this for any cleanup SQL targeting the modern remote DBs.
- **`prodworkbc`** — older snapshot (bigint `JobId`). Useful for reproducing legacy-schema issues only.

```bash
docker exec job-importer-postgres-1 psql -U workbc -d devworkbc
```

## Things to watch out for

- **Don't upgrade Elasticsearch past 7** — production runs AWS OpenSearch, which forked from ES 7.
- **Don't upgrade Node past 14 in `WorkBC.Web/ClientApp`** — the Angular 12 toolchain breaks on newer Node.
- **Linux dev cannot run EF migrations against Postgres** — restore `scripts/jobboard-full.sql.gz` instead. EF migrations only target SQL Server.
- **Federal importer requires an IP-whitelisted egress** to jobbank.gc.ca; failures from a new dev machine usually mean the wrong source IP.
- **The Angular `proxy.conf.json` points at a remote dev environment.** If you start `ng serve` directly (instead of running the .NET host), API calls will hit the cloud dev environment, not your local backend.
- **`Job.cs` / older EF migrations declare `JobId` as bigint** — production Postgres has migrated to varchar. Don't trust the EF model for SQL — `\d` the live table first.
