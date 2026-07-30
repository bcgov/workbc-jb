# Copilot Instructions — WorkBC Job Board (Laravel rewrite)

You implement Jira stories for the WorkBC Job Board. Follow this file on **every** task.
For detail, read the referenced `docs/` files. If a story cannot be done without violating
an enforced constraint below, **STOP and add an ADR** under `docs/adr/` instead of working
around it. Treat ambiguity as "stop and ask," not "interpret."

## Stack (no alternatives without an ADR)
- Backend: **Laravel 12, PHP 8.3**
- Frontend: **Blade + Livewire (+ Alpine, bundled)** — server-rendered, no SPA. See ADR-002.
  - **Alpine** for view state (toggles, dropdowns, modals). **Livewire** for data-backed
    reactivity (search filters, alert builder, forms). Alpine for view, Livewire for data.
- Admin: **Filament 4**
- **PostgreSQL 15** · **Redis 7** (cache/queue/session) · **OpenSearch** · **AWS EKS** via GitHub Actions

## Architecture in one line
This is the **app/web layer only**. PostgreSQL is the system of record (shared with the existing
PHP data pipeline); OpenSearch is a derived read model the app **reads**. **Feed ingestion and
indexing are existing PHP standalone containers — NOT in this codebase; do not build or modify
them.** The app is server-rendered (Blade + Livewire); its own background work (alert emails,
sitemap) runs as queued jobs.
Detail: `docs/architecture.md` · Terms: `docs/glossary.md` · Data model: `docs/data-model.md`
· Contracts: `docs/contracts.md` · Decisions: `docs/adr/`

## Enforced constraints (MUST / MUST NOT)
1. **Never recompute derived fields** — `ExpireDate`, annualized `Salary`, min-wage eligibility,
   resolved NOC/location come from the existing ingestion pipeline. The app (search, API, pages)
   **reads** them from `Jobs`/OpenSearch and **MUST NOT** recompute them. (ADR-001)
2. OpenSearch is derived: every document field **MUST** be reproducible from PostgreSQL, the
   index **MUST** be fully rebuildable from `Jobs`, and no data may exist only in OpenSearch. (ADR-001)
3. Write each aggregate only via its owning service (Jobs/staging → Ingestion;
   SavedJobs/JobAlerts → JobSeeker; stats → Reporting). **MUST NOT** write another aggregate's tables directly.
4. **Ingestion & indexing are out of scope** — the existing PHP importers and indexer (standalone
   containers) own them; do **not** reimplement or modify them. Any external call the app *does*
   make (geocoding, email) goes through a service/adapter, never inline in a web request.
5. `/api/*` changes **MUST** be additive; breaking changes need a version bump + ADR. Validate
   every request with a Form Request (reject unknown fields, fail closed). Return API Resources, never raw models.
6. Dependencies flow **Http → Services → (Models | Search | Adapters)**. No business logic in
   controllers, Livewire components, or Filament resources; no service calls from models.
7. All date logic assumes **`America/Vancouver`**. **MUST NOT** mix naive UTC/local.
8. **MUST NOT** log personal data (FOIPPA), commit secrets, or issue non-expiring tokens.
9. **Do NOT invent domain values — copy them from the source of truth, never infer.** Enum
   cases/codes come from `../workbc-jb/src/WorkBC.Data/Enums/*.cs` (values AND names, exactly — e.g.
   `AdminLevel` is ascending privilege `Reporting=1…SuperAdmin=3`; getting it wrong is a
   privilege-escalation bug). Table/column names and every relationship's FK **MUST** be verified
   against the real schema (`docs/data-model.md` + the live DB) — never assume a column exists (e.g.
   `Jobs` has **no** `RegionId`; region is via `Location`). Reference-data ids (NAICS/`Industries`,
   NOC, `Regions`) come from the DB tables, not a guessed scheme. **Assume legacy data is dirty:**
   cast enums **tolerantly** (unknown code → `null`, never a 500), and test fixtures **MUST** mirror
   the real column names and include out-of-range/edge values — a golden-read test that only uses
   clean in-range fixtures does **not** prove the mapping.

## Authentication (ADR-003)
- **Job seekers:** Laravel **session** auth. Verify legacy ASP.NET Identity (v2/v3 PBKDF2)
  hashes and **rehash to bcrypt/argon2** on login. **MUST NOT** port MD5 hashing.
- **Admin:** **Keycloak OIDC** (Socialite) → Filament, mapped to `AdminUsers` roles.
- Preserve **impersonation** and its `Impersonations` audit trail.

## Coding standards
- Controllers thin: Form Request validate → call a Service → return. Business logic only in `app/Services`.
- Services single-purpose, constructor-injected, never `new`ed in controllers.
- **Runs on the EXISTING production database — do NOT create a new schema.** Eloquent models
  **map** to existing PascalCase tables (`$timestamps=false`; string PKs where applicable). Never
  create/alter/drop/rename an existing table or column. **New migrations only for genuinely new
  tables** (`import_logs`, Laravel framework tables). See `docs/data-model.md`.
- Eloquent for DB; raw SQL only when Eloquent can't express it, with a comment saying why.
  Wrap multi-table writes in a transaction.
- Queued jobs (alert emails, sitemap regeneration, view-count flush) extend `BaseJob`, set
  `$tries`/`$backoff` (default 3 + exponential), are idempotent, and log lifecycle via the standard channel.
- Livewire: one component = one responsibility; keep server round-trips purposeful (use Alpine
  for pure view state). Blade partials for shared markup. No business logic in components.
- Build shared UI as reusable **Blade components** (`resources/views/components`) + Livewire
  components; establish a small internal library (buttons, form fields, alerts) early and reuse
  it — do not duplicate markup. All shared components must meet the accessibility rules below.

## Accessibility (mandatory — WCAG 2.1 AA, legal for BC gov)
Keyboard-operable; `<label>` on every input; colour never the sole signal; 4.5:1 contrast
(3:1 large text); meaningful `alt`; logical focus order + visible focus indicators; announce
dynamic updates (Livewire re-renders, filter results) via ARIA live regions. Every UI PR confirms a11y verified.

## Where code goes
```
app/Http/Controllers/{Api,Web} · Requests · Resources
app/Livewire/            (search, alerts, saved jobs)
app/Services/{Search,JobSeeker,Reporting,Integration}
app/Jobs · app/Models · app/Filament
app/Search/Queries · app/Support
resources/views/         (Blade)
# ingestion & indexing are existing PHP standalone containers — NOT in this codebase
```

## Definition of Done
- Acceptance criteria met and verified. Follows this file.
- Unit test per Service method; feature test per API endpoint (happy + ≥1 failure);
  Livewire component tests for interactive components.
- Tests pass; coverage not reduced. Accessibility verified if UI. **Run them in the container** —
  this project runs on Laravel Sail, so a bare `php artisan test` fails with `php: command not found`:
  ```bash
  docker compose exec -T laravel.test php artisan test          # all
  docker compose exec -T laravel.test php artisan test --filter=SomeTest
  ```
  (`docker compose up -d` first if the stack is down.)
- PR links the Jira ticket (`Closes JOBS-123`), explains what/why, conventional-commit title,
  one ticket per PR. Reviewed, approved, merged to **`develop`** (the integration branch — *not*
  `main`), deployed.

## Branching
- **Code** → feature branch off `develop`, merged back with `--no-ff` (e.g. `srch-9-map`,
  `acct-3-alerts`). One story per branch.
- **Docs-only** → commit directly on `develop`.
- Never commit directly to `main`.

## Before opening a PR — self-check (confirm ALL)
- [ ] No derived field computed outside ingestion; indexer copies, doesn't recompute.
- [ ] Only the owning service writes each aggregate.
- [ ] Feed calls only in adapters, inside queued jobs; imports idempotent.
- [ ] API change additive (or ADR linked).
- [ ] Dates assume America/Vancouver.
- [ ] No secrets/PII in code or logs; no non-expiring tokens.
- [ ] No invented domain values: enum cases match `WorkBC.Data/Enums/*.cs`; every column/FK verified
      against the real schema; enums cast tolerantly; fixtures mirror real columns + include edge values.
- [ ] Alpine for view state, Livewire for data (no needless server round-trips).
- [ ] Tests written and passing via `docker compose exec -T laravel.test php artisan test`;
      a11y verified if UI.
- [ ] Story checkboxes in `docs/epics/` ticked and a dated status line added — a story that reads
      "not built" when it is gets rebuilt or skipped next time.
- [ ] PR links Jira ticket; scope is one ticket; branch targets `develop`.
