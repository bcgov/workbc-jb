# EPIC — Foundation (Laravel Job Board skeleton)

**Goal:** Stand up the Laravel application on the **existing** database and infrastructure so the
feature epics (Search, Account, Admin/Reporting) build on a coherent, tested
base. No user-facing features here — this epic delivers scaffold, connections, base classes, auth,
core models, the search foundation, and CI/CD.

**Applies to every story:** follow `.github/copilot-instructions.md` and the enforced constraints;
existing DB — **map, don't create** (`docs/data-model.md §0`); one ticket per PR; DoD + self-check
per copilot-instructions. Reference the doc sections named in each story.

**Story order / dependencies:** FND-1 first; FND-2/3/4/8 depend on FND-1; FND-5/6 depend on FND-1+2;
FND-7 depends on FND-1 (+ `contracts.md`). 

---

## FND-1 — Project scaffold & infrastructure connections
**Description:** Create the Laravel 11 / PHP 8.3 app with Blade + Livewire + Filament 3, wired to
the **existing** PostgreSQL, Redis, and OpenSearch — no schema creation.

**Acceptance criteria**
- [ ] Laravel 11, PHP 8.3; Livewire + Filament 3 installed; **no Inertia/React** packages.
- [ ] `config/app.php` timezone = **`America/Vancouver`**; a test fails if it changes (Rule F).
- [ ] PostgreSQL connection points at the existing database; `php artisan migrate:status` runs
      against Laravel's **own** `migrations` table (does not touch `__EFMigrationsHistory` or any
      existing table).
- [ ] Redis configured as **cache, session, and queue** driver (no DB `jobs`/`sessions`/`cache` tables).
- [ ] OpenSearch client (`opensearch-project/opensearch-php` or equivalent) wired; a health check
      command pings the cluster and lists the `jobs_en`/`jobs_fr` indexes.
- [ ] A `README` documents local run (existing-DB pointer, seeded/real data expectations).

**Docs:** copilot-instructions (Stack, Enforced constraints); `data-model.md §0, §5`; `architecture.md §9`.

---

## FND-2 — Core Eloquent models (existing-DB mapping)
**Description:** Map Eloquent models to the existing PascalCase schema for the entities the feature
epics need first.

**Acceptance criteria**
- [ ] Models for: `Job`, `JobSeeker` (`AspNetUsers`), `SavedJob`, `JobAlert`, `JobSource`,
      `Location`, `Region`, `Industry`, `NocCode`, `NocCode2021`, `SystemSetting`, `AdminUser`,
      `JobSeekerFlags`.
- [ ] Each model sets `$table` (PascalCase), `$timestamps = false`, correct `$primaryKey`/`$keyType`/
      `$incrementing`, and casts (bools, `datetime`, `Salary` decimal:2, enums).
- [ ] `SavedJob`/`JobAlert` use a shared **soft-delete trait** on `IsDeleted`+`DateDeleted`
      (not Laravel `SoftDeletes`).
- [ ] Composite-PK tables (`JobStats`, `JobSeekerStats`, `ReportPersistenceControl`) are **not**
      modeled as standard Eloquent (query-builder only) — documented in code.
- [ ] **Golden-read tests:** for each model, assert reads against known real rows (counts, casts,
      one relationship traversal) — proves the mapping before any writes.

**Docs:** `data-model.md §1–3, §6`; `glossary.md`. **Depends on:** FND-1.

---

## FND-3 — Queue foundation: BaseJob & logging
**Description:** The queued-job base class for the app's **own** background work (alert emails,
sitemap regeneration, view-count flush). Feed import/index jobs are **out of scope** — existing containers.

**Acceptance criteria**
- [ ] `BaseJob` abstract class: `$tries`/`$backoff` (default 3 + exponential), `failOnTimeout`,
      idempotency helpers; `ShouldBeUnique` where re-runs must not duplicate.
- [ ] Structured logging to stdout (container-friendly).
- [ ] Dedicated Redis queues (`notifications`, `default`) configured.
- [ ] Unit test: a sample job records start/finish and is idempotent on re-run.

**Docs:** copilot-instructions (Coding standards); `architecture.md §9`. **Depends on:** FND-1.

---

## FND-4 — Base Blade layout & accessible component library
**Description:** The server-rendered layout and a small internal, accessible component set.

**Acceptance criteria**
- [ ] Base Blade layout (header/footer/nav), responsive, no horizontal body scroll.
- [ ] Internal Blade component library: button, form field (with associated `<label>`), alert,
      pagination — each meeting WCAG 2.1 AA (keyboard, contrast, focus, ARIA where needed).
- [ ] Livewire installed and demonstrated with one trivial reactive component; Alpine used for a
      trivial view-state toggle (documents the "Alpine for view / Livewire for data" rule).
- [ ] An automated a11y check (axe or pa11y) runs in CI against the base layout.

**Docs:** copilot-instructions (Frontend, Accessibility); `ADR-002`. **Depends on:** FND-1.

---

## FND-5 — Job-seeker session auth + legacy-hash verifier
**Description:** Session authentication against `AspNetUsers` with an ASP.NET-Identity-compatible
password verifier that rehashes to bcrypt on login. Email-only reset.

**Acceptance criteria**
- [ ] `JobSeeker` implements `Authenticatable` (`getAuthPassword()` → `PasswordHash`).
- [ ] Custom hasher/user-provider verifies Identity **v2/v3 PBKDF2** hashes; on success, rehashes
      to **bcrypt/argon2** and updates `PasswordHash`. Test vectors from the live stack.
- [ ] Anything unrecognized (incl. legacy MD5 marker) → treated as "force reset"; **MD5 hashing is
      not implemented**.
- [ ] Registration + **email verification** (`VerificationGuid`) flow; **email-only** password reset
      (`password_reset_tokens`); **no** security-question flow.
- [ ] `NormalizedEmail`/`NormalizedUserName` maintained on writes.
- [ ] Feature tests: login (v3 hash → success + rehash), unrecognized hash → reset path, register→verify, reset.

**Docs:** `ADR-003`; `data-model.md` (AspNetUsers). **Depends on:** FND-1, FND-2.

---

## FND-6 — Admin auth: Keycloak OIDC → Filament
**Description:** Keycloak OIDC login for the Filament admin panel, mapped to `AdminUsers` roles.

**Acceptance criteria**
- [ ] Socialite (or OIDC middleware) authenticates admins via Keycloak (Authorization-Code flow);
      config from Secrets Manager, no committed secrets.
- [ ] On login, the Keycloak identity maps to an `AdminUsers` row; roles (SuperAdmin/Admin/Reporting)
      drive Filament access policies.
- [ ] Filament panel is reachable only after OIDC auth; a smoke resource (e.g. read-only SystemSettings) loads.
- [ ] Impersonation scaffold: a Filament action starts an impersonated **seeker session** and writes
      an `ImpersonationLog` row (full flow can be a later story; the audit-write + session-switch exists).
- [ ] Feature test: unauthenticated → redirected to Keycloak; role gate denies a non-admin.

**Docs:** `ADR-003`; `data-model.md` (AdminUsers, ImpersonationLog). **Depends on:** FND-1, FND-2.

---

## FND-7 — Search foundation: OpenSearch client, JobSearchQuery, JobSearchFilters
**Description:** The search building blocks the public search and Drupal API will use.

**Acceptance criteria**
- [ ] `JobSearchFilters` **value object** (`Castable`) with **all** fields per `contracts.md §1`,
      exact PascalCase names; deserializes version **0 and 1**; rejects unknown fields.
- [ ] `App\Search\Queries\JobSearchQuery` builds the OpenSearch body as a **structured array**
      (no string concatenation); base filter `ExpireDate >= now/d` (America/Vancouver); the 11 sort
      orders and paging.
- [ ] Result DTO mapping (count, result[], the job fields per `contracts.md §2.1`).
- [ ] **JSON-diff test harness:** for a corpus of recorded filter inputs, the PHP query body matches
      the reference bodies byte-for-byte (or normalized-JSON equal).
- [ ] Keyword parser ported with the documented BRD examples as unit tests.

**Docs:** `contracts.md §1–2`; `architecture.md §7`; `glossary.md`. **Depends on:** FND-1.

---

## FND-8 — CI/CD pipeline & k8s CronJob → artisan wiring
**Description:** GitHub Actions to EKS, and the scheduled-job trigger pattern.

**Acceptance criteria**
- [ ] GitHub Actions: build, `php artisan test`, a11y check, image push, deploy to EKS.
- [ ] Separate workloads: web + queue workers + the app's scheduled CronJobs.
- [ ] **k8s CronJob → `php artisan` → Redis queue** pattern demonstrated with a no-op scheduled
      command (proves the app's scheduled-job path — alert emails, sitemap; ADR-004).
- [ ] Secrets via AWS Secrets Manager; no secrets in the repo or images.
- [ ] The **existing importer/indexer containers and pg_cron are not touched**.

**Docs:** `ADR-004`; `architecture.md §9`; copilot-instructions (Stack, Security). **Depends on:** FND-1.

---

## Definition of Done (epic)
- [ ] All 8 stories merged; `php artisan test` green; a11y check passing.
- [ ] App runs against the existing database with **zero** schema changes to existing tables.
- [ ] A feature epic can start: models, auth, queue base, search foundation, and CI/CD all in place.
