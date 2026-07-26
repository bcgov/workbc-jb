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
**Description:** Create the Laravel 12 / PHP 8.3 app with Blade + Livewire + Filament 4, wired to
the **existing** PostgreSQL, Redis, and OpenSearch — no schema creation.

**Acceptance criteria**
- [ ] Laravel 12, PHP 8.3; Livewire + Filament 4 installed; **no Inertia/React** packages.
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

**Build notes (schema verified against the restored real DB, 2026-07-24):**
- `data-model.md §1–3` PKs/types were **cross-checked against the real database and are accurate —
  trust them.** Quick ref: **string** PKs → `Jobs.JobId`, `AspNetUsers.Id`, `SystemSettings.Name`
  (`$incrementing=false; $keyType='string'`); **smallint** PKs → `JobSources.Id`, `Industries.Id`,
  `NocCodes.Id`; **int** PKs → `AdminUsers.Id`, `SavedJobs.Id`, `JobAlerts.Id`, `Locations.LocationId`,
  `Regions.Id`, `NocCodes2021.Id`, `JobSeekerFlags.Id`.
- Soft-delete pair (`IsDeleted`+`DateDeleted`) confirmed on `SavedJobs`, `JobAlerts`,
  `SavedCareerProfiles`, `SavedIndustryProfiles` → shared custom trait (not Laravel `SoftDeletes`).
- Composite PKs confirmed → query-builder only: `JobStats`(WeeklyPeriodId,RegionId,JobSourceId),
  `JobSeekerStats`(WeeklyPeriodId,LabelKey,RegionId), `ReportPersistenceControl`(WeeklyPeriodId,TableName).
- `Regions` holds **special rows with ids ≤ 0** ("Multiple Locations" −5, "Virtual Jobs" −4,
  "Outside BC…" −1, "Location Not Available" 0) alongside the 7 economic regions (ids 2–8) — do not
  assume region ids are all positive.
- `Industries.Id` is the real NAICS scheme (`1, 21–46`), **not** the legacy 1–19 (see the
  industry-taxonomy fix in the search epic).

**Golden-read tests — keep them portable:** the suite runs on **SQLite in-memory**, not the prod dump,
so do **not** assert exact production row counts (brittle + non-portable). Prove each mapping with a
small controlled fixture — create the table + a couple of known rows (pattern:
`tests/Concerns/InteractsWithLocationsTable.php`), then assert the `$table`/PK resolve, the casts
convert (bool / datetime / `decimal:2`), and one relationship traverses. Reserve full-DB spot-checks
for manual `tinker` against the local `jobboard` DB.

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
- [ ] **Chrome-less "embed" layout mode** (per `ADR-006`): a layout variant with **no WorkBC
      header/footer** for serving the search inside the Drupal iframe, selectable per-request (e.g.
      `?embed=1` or a route/middleware). The full layout is retained for local dev / direct access.
      Embed mode also emits the `postMessage` content-height bridge so the parent frame can auto-size.
- [ ] Internal Blade component library: button, form field (with associated `<label>`), alert,
      pagination — each meeting WCAG 2.1 AA (keyboard, contrast, focus, ARIA where needed).
- [ ] Livewire installed and demonstrated with one trivial reactive component; Alpine used for a
      trivial view-state toggle (documents the "Alpine for view / Livewire for data" rule).
- [ ] An automated a11y check (axe or pa11y) runs in CI against the base layout.

**Docs:** copilot-instructions (Frontend, Accessibility); `ADR-002`, `ADR-006` (embed mode).
**Depends on:** FND-1.

---

## FND-5 — Job-seeker session auth + legacy-hash verifier
**Description:** Session authentication against `AspNetUsers` with an ASP.NET-Identity-compatible
password verifier that rehashes to bcrypt on login. Email-only reset.

**Acceptance criteria**
- [ ] `JobSeeker` implements `Authenticatable` (`getAuthPassword()` → `PasswordHash`) — **done in
      FND-2**; here wire the session guard + a custom user-provider/hasher.
- [ ] The verifier accepts the **three** stored formats and, on **any** success, rehashes to
      **bcrypt/argon2** and overwrites `PasswordHash` (+ regenerate `SecurityStamp`):
      - **v3** (`0x01`, base64 `AQAAAA…`) → PBKDF2-HMAC-SHA256, params read from the blob;
      - **v2** (`0x00`) → PBKDF2-HMAC-SHA1, 1000 iters, 16-byte salt, 32-byte subkey;
      - **MD5-marker** (`0xF0`, base64 `8AAAAA…`) → **verify per ADR-007** (flip byte 0 to `0x01`, then
        verify the v3 blob against `md5_hex(password)`). MD5 is **verification-only**, never used to
        create a hash.
- [ ] Truly unrecognized format (not `0x00`/`0x01`/`0xF0`, or an undecodable blob) → **force-reset** path.
- [ ] Registration + **email verification** (`VerificationGuid`) flow; **email-only** password reset
      (`password_reset_tokens`); **no** security-question flow.
- [ ] `NormalizedEmail`/`NormalizedUserName` maintained on writes (Identity uses `ToUpperInvariant()`).
- [ ] Feature tests: **v3** login → success + bcrypt rehash; **`0xF0`** login → success + rehash;
      unrecognized hash → reset path; register→verify; reset.

**Build brief (verified against the real DB + .NET source, 2026-07-24):**
- Auth columns confirmed present on `AspNetUsers`: `PasswordHash`(text), `SecurityStamp`,
  `NormalizedEmail`, `NormalizedUserName`, `EmailConfirmed`, `LockoutEnabled`, `LockoutEnd`,
  `AccessFailedCount`, `VerificationGuid`(uuid), `UserName`, `Email`.
- Real hash split (345,985 rows, **no nulls**): v3 `AQAAAA…` = 131,947 (38%); MD5-marker `8AAAAA…`
  = 214,038 (**62%**). The `0xF0` path is the **majority** — get it right and tested (this is why
  ADR-007 verifies rather than force-resets it).
- **Port** `../workbc-jb/src/WorkBC.Web/Helpers/Md5PasswordHasher.cs` for the `0xF0` case:
  base64-decode; if `bytes[0]==0xF0`, set it to `0x01`, re-encode, and verify the standard v3 blob
  against `md5_hex(password)` where `md5_hex = strtolower(bin2hex(hash('md5', $password, true)))`.
- Identity blob layout (big-endian): v3 = `[0x01][prf u32][iter u32][saltLen u32][salt][subkey]`
  (prf 0=SHA1,1=SHA256,2=SHA512); v2 = `[0x00][16-byte salt][32-byte subkey]`. Use `hash_pbkdf2()`
  + constant-time `hash_equals()`.
- **Test vectors:** the scrambled dump has **no plaintexts** — do NOT try to reverse real hashes.
  Generate `{password → hash}` pairs (the reference `Md5PasswordHasher` can emit a `0xF0` and a v3
  hash for a known password; dotnet 6 is installed), commit them as fixtures, and assert the PHP
  verifier accepts the right password and rejects a wrong one for each format.

**Docs:** `ADR-003`, **`ADR-007`** (verify MD5-wrapped hashes); `data-model.md` (AspNetUsers).
**Depends on:** FND-1, FND-2.

---

## FND-6 — Admin auth: Keycloak OIDC → Filament
**Description:** Keycloak OIDC login for the Filament admin panel, mapped to `AdminUsers` roles.

**Split per ADR-008** (no Keycloak realm/client available yet; `AdminUsers` has no credential column
— verified against the real schema, confirming ADR-003's own note that it's "not used for login"):
this story (FND-6) delivers everything **except** the OIDC handshake. **FND-6b** (below) is the
handshake itself, once real Keycloak credentials exist.

**Acceptance criteria**
- [ ] A dedicated `admin` guard (session-based); provider bound to `App\Models\AdminUser` (FND-2).
      `AdminUser` implements `Authenticatable` but the provider has **no password check** — there is
      no credential to check (`AdminUsers` has none). It supports direct session login only
      (`Auth::guard('admin')->login($adminUser)`) — the shape FND-6b's OIDC callback will call.
- [ ] Filament's admin panel authenticates via the `admin` guard (not the default `web`/job-seeker guard).
- [ ] `AdminUsers.AdminLevel` (`Disabled`/`Reporting`/`Admin`/`SuperAdmin`) drives Filament navigation
      + resource access policies (ADM-1 detail).
- [ ] Impersonation scaffold: a Filament action starts an impersonated **seeker session** and writes
      an `ImpersonationLog` row (`Token`, `AspNetUserId`, `AdminUserId`, `DateTokenCreated`); ending
      impersonation returns to the admin session (full flow ADM-4; the audit-write + session-switch
      exists here).
- [ ] Local/demo access: the same gitignored, `local`-env-guarded pattern as the job-seeker portal
      (`routes/dev-preview.php`) — logs in a chosen `AdminUser` directly. No schema change.
- [ ] Feature tests: guard resolves an `AdminUser`; role gate denies a lower `AdminLevel`; Filament
      panel unreachable when logged out (of the `admin` guard specifically); impersonation writes the
      audit row + switches the session; ending impersonation returns cleanly.

**Docs:** `ADR-003`, **`ADR-008`**; `data-model.md` (AdminUsers, ImpersonationLog). **Depends on:** FND-1, FND-2.

---

## FND-6b — Keycloak OIDC handshake (follow-up; needs real IdP credentials)
**Description:** Replace FND-6's local/demo login route with the real Socialite ↔ Keycloak
Authorization-Code exchange. **Not started — blocked on Keycloak realm/client/issuer credentials.**

**Acceptance criteria**
- [ ] Socialite (or OIDC middleware) authenticates admins via Keycloak (Authorization-Code flow);
      config from Secrets Manager, no committed secrets.
- [ ] On successful callback, resolve the Keycloak identity to an `AdminUsers` row and call
      `Auth::guard('admin')->login($adminUser)` — the same call FND-6's guard already supports.
- [ ] Unauthenticated access to the admin panel redirects to Keycloak (not a local login form).
- [ ] Delete/disable `routes/dev-preview.php`'s admin shortcut once this ships (it's gitignored, so
      nothing to remove from the repo — just stop relying on it locally).
- [ ] Feature test: unauthenticated → redirected to Keycloak.

**Docs:** `ADR-003`, `ADR-008`. **Depends on:** FND-6; real Keycloak dev/test realm credentials.

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
