# Job Board — System Architecture

> **Purpose:** Defines system structure, module boundaries, data flows, and architectural
> rules. Companion to `.github/copilot-instructions.md`. Place code per the module boundaries
> and follow the Architectural Rules. If a story requires violating a rule, stop and request an ADR.

## 1. System Context

The Job Board is one application in a larger ecosystem. It is **not** the public marketing
site — WorkBC.ca is a **Drupal** application that consumes the Job Board's JSON API.

| Actor / System | Relationship |
|---|---|
| **Job seekers** | Use the Job Board's own search/detail/account pages (server-rendered) |
| **Drupal (WorkBC.ca)** | Server-to-server calls to the Job Board **JSON API**; renders results itself |
| **Admins** | Use the Filament admin panel (Keycloak OIDC) |
| **Federal Job Bank** | External XML (Solr) feed → Federal importer |
| **Innovibe** | External JSON feed → Innovibe importer |
| **OpenSearch** | Derived search index (Rule A) |
| **PostgreSQL** | System of record |
| **Redis** | Cache, session, queue |
| **Keycloak** | Admin OIDC IdP (BC Gov standard) |
| **Email (SES/SendGrid/SMTP)** | Job-alert delivery |

## 1a. Scope — reused vs. built

The Laravel application **builds**: the public site (search + detail), the job-seeker account, the
Filament admin, reporting, the Drupal-facing API, and orchestration. It **reads** the existing
PostgreSQL and OpenSearch.

**Reused as-is — NOT reimplemented or modified by the rewrite:**
- the **OpenSearch indexer** (existing PHP) — projects `Jobs`/staging → OpenSearch on its own k8s
  CronJob schedule. **Do not touch this code.** The rewrite consumes its output (OpenSearch); it
  does not own it.
- the **feed importers** — also existing PHP standalone containers, left untouched.

The rewrite does **not** contain ingestion or indexing code. It reads `Jobs`/OpenSearch and owns
only the app/web layer.

Rules A/B remain the **system's** invariants (OpenSearch is derived; derived fields computed once).
Enforcing them inside the existing indexer is that component's concern, not the rewrite's — but any
`ExpireDate`/salary correctness fix belongs there, in the PHP indexer.

## 2. Architectural Rules (Enforced)

- **Rule A — OpenSearch is a derived read model, never a source of truth.** Fully rebuildable
  from PostgreSQL at any time. No data exists only in OpenSearch. (ADR-001)
- **Rule B — Never recompute derived fields.** `ExpireDate`, annualized `Salary`, min-wage
  eligibility, resolved NOC/location are produced by the existing ingestion pipeline and written to
  `Jobs`. The app (search, API, pages) **reads** them from `Jobs`/OpenSearch and **MUST NOT**
  recompute them. *(Production defects came from recomputing `ExpireDate`/salary and drifting.)* (ADR-001)
- **Rule C — Ingestion & indexing are out of scope.** The existing PHP importers and indexer
  (standalone containers) own feed pulling, normalization, and projection to OpenSearch. The rewrite
  does not reimplement, modify, or branch on that logic.
- **Rule D — Dependencies flow one way:** `Http → Services → (Models | Search | Adapters)`.
- **Rule E — External calls only in adapters, only from queued jobs.** No synchronous feed
  call during a web request.
- **Rule F — Timezone is `America/Vancouver` application-wide.** No naive UTC/local mixing.

## 3. Application Layers & Placement

```
app/
  Http/Controllers/{Api,Web} · Requests · Resources
  Livewire/                # interactive components (search, alerts, saved jobs)
  Services/{Search,JobSeeker,Reporting,Integration}
  Jobs/                    # queued jobs (alert emails, sitemap) — extend BaseJob
  Models/                  # Eloquent (maps the existing schema)
  Filament/                # admin resources, pages, widgets
  Search/Queries           # search query building (reads OpenSearch)
  Support/                 # cross-cutting helpers, value objects
resources/views/           # Blade
```
Ingestion and indexing are **not** in this codebase — they are existing PHP standalone containers.

## 4. Core Domain Model (write ownership is exclusive)

| Aggregate | Tables | Written by |
|---|---|---|
| **Job** | `Jobs`, `JobVersions`, `JobViews` | Ingestion only |
| **Staging** | `ImportedJobsFederal`, `ImportedJobsWanted` | Ingestion (adapters) |
| **Job Seeker** | `AspNetUsers`, `SavedJobs`, `JobAlerts`, `Saved*Profiles` | JobSeeker services |
| **Reference** | `NocCodes(2021)`, `Locations`, `Regions`, `Industries` | Read-only (seed/migrate) |
| **Reporting** | `WeeklyPeriods`, `JobStats`, `JobSeekerStats` | Reporting |
| **Admin/System** | `AdminUsers`, `SystemSettings`, `Impersonations` | Admin/Filament |

Cross-aggregate **reads** allowed; cross-aggregate **writes** go through the owning service.
Full field mapping: `docs/data-model.md`. Terms: `docs/glossary.md`.

> **Existing database — hard constraint.** The app runs on the existing PostgreSQL database with
> existing data. Eloquent **maps** to the existing PascalCase schema; we do **not** create a new
> database or alter existing tables. New migrations only for genuinely new tables. EF migrations
> (`__EFMigrationsHistory`) are frozen; Laravel uses its own `migrations` table. Database-resident
> logic (stats stored procedures, `tvf_*` functions, and the `pg_cron` schedule) stays in the DB —
> the app calls it, it does not reimplement it. Details and gotchas: `docs/data-model.md`.

## 5. Data Flows

**5.1 Ingest (existing PHP importers — standalone containers, reused as-is; NOT built by the
rewrite):** pull feeds → staging → normalize (derived fields computed here) → `Jobs` (+
`JobVersions`) → mark `ReIndexNeeded`. Shown for context only.

**5.2 Index (existing PHP indexer — reused as-is, NOT built by the rewrite):** `Jobs WHERE
ReIndexNeeded` → OpenSearch document → clear flag; expired/removed → delete. Runs on its own k8s
CronJob. The rewrite does not modify this; it only reads the resulting index. (Rules A/B still
describe the intended behaviour of that indexer.)

**5.3 Search:** Web/API/Livewire → `Search\Queries\JobSearchQuery` (array query body; base
filter `ExpireDate >= now`, America/Vancouver) → OpenSearch → result DTOs → Blade/JSON.

**5.4 Alerts (scheduled + queued):** evaluate `JobAlerts.JobSearchFilters` on cadence
(daily; weekly Mon; biweekly 15th + EOM; monthly 1st) → render → email.

**5.5 Reporting (scheduled):** populate `JobStats`/`JobSeekerStats`; report queries read those.

## 6. Rendering & integration (ADR-002, ADR-006)

Everything is **server-rendered** — there is no SPA.
- **Public search + job detail:** **Blade + Livewire**, delivered **chrome-less** (no WorkBC
  header/footer) to be **embedded in the Drupal page via an `<iframe>`** — Drupal owns the page and
  chrome (ADR-006). Pages are path-based (no hash routing). **SEO is not a driver** (the current site
  isn't crawlable and indexing isn't required); the `schema.org/JobPosting` structured data and
  `sitemap.xml` are **retained but not primary** — they keep a future "flip to crawlable direct
  access" option open. Shareable-search and email-alert links open the app **directly** (the browser
  URL inside a frame doesn't reflect search state).
- **Authenticated job-seeker area** (dashboard, saved jobs, alerts, recommended, profiles,
  settings): **Blade + Livewire** for interactivity (behind login).
- **Admin:** **Filament**.
- **Alpine** for view state, **Livewire** for data-backed reactivity.

## 7. Search Subsystem

- Queries built in `app/Search/Queries/*` as **structured arrays**; user input is data, never
  string-concatenated into a query body.
- The index schema and the `Jobs → document` projection are owned by the **existing PHP indexer**
  (out of scope). The app **reads** the index; it does not build, map, or reindex it.
- The app **reads** `ExpireDate`/`Salary` from the index/`Jobs`; it never recomputes them (Rule B).

## 8. Integration API (Drupal-facing)

- Drupal consumes JSON under `/api/*` — a **published contract**; changes are additive or
  versioned (+ ADR). Contract shapes: `docs/contracts.md`.
- Validate with Form Requests (reject unknown fields). Return API Resources, never raw models.

## 9. Cross-Cutting

- **Auth:** job seekers → Laravel session (legacy-hash verify + rehash to bcrypt; no MD5).
  Admin → Keycloak OIDC → Filament. Impersonation audited. (ADR-003)
- **Queues (Redis):** dedicated queues for ingestion / indexing / notifications; import jobs
  `ShouldBeUnique` + idempotent.
- **Scheduling (ADR-004):** DB-resident work (stats procedures) runs on **pg_cron**; app-level
  jobs (feed imports, indexing, job-alert notifications) run as **Kubernetes CronJobs** invoking
  `php artisan` commands that dispatch to the Redis queue. One scheduler per job — never both.
- **Config/Secrets:** AWS Secrets Manager + env. Feed endpoints, index names, expiry windows are config.
- **Caching:** `SystemSettings` + reference data cached with explicit invalidation on write.
- **Logging:** structured; import lifecycle → `import_logs`; never log personal data (FOIPPA).

## 10. Change Control

Any deviation from Rules A–F, a new external dependency, a search-mapping change, or an API
contract change requires an **ADR** (`docs/adr/`) linked from the Jira story. The agent must
not silently introduce a second computation of a derived field, a second normalization path,
or a direct-to-OpenSearch write of non-derived data.
