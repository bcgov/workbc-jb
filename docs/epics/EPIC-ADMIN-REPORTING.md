# EPIC — Admin & Reporting

**Goal:** The Filament admin panel with **parity to the current WorkBC.Admin** — job-seeker
management, job/posting browsing, admin-account management, and system settings — **plus** the full
reporting suite (core reports ported from Dapper) **and** the new chart/dashboard visualizations.
Charts are additive; the core admin + reporting must be preserved first.

**Applies to every story:** **Filament 3**; Keycloak OIDC auth (FND-6); role-based policies from
`AdminUsers.AdminLevel`; existing DB — map, don't create; reports **call the existing stored
procedures** (scheduled by pg_cron — ADR-004), never reimplement them; WCAG 2.1 AA (**including
charts**); no PII in logs (FOIPPA); DoD + self-check.

**Depends on:** FND-1 (scaffold), FND-2 (models), FND-6 (admin auth/Filament). Reporting also depends
on the Reporting service + the existing stats procs/functions.

---

## ADM-1 — Admin panel shell & role-based access
- [ ] Filament panel behind Keycloak (FND-6); map `AdminUsers.AdminLevel` → **SuperAdmin / Admin /
      Reporting** policies.
- [ ] Navigation to all admin areas; each area gated by role (e.g. Reporting role → reports only;
      SystemSettings + AdminAccounts → SuperAdmin).
- [ ] Tests: role gating per area; unauthenticated → Keycloak.

**Docs:** `ADR-003`, `data-model.md` (AdminUsers).

---

## ADM-2 — Job Seeker management
- [ ] Filament resource over `AspNetUsers` (job seekers): search (name / email / city), view, edit profile
      (`FirstName`, `LastName`, `City`, `LocationId`, `CountryId`, `ProvinceId`).
- [ ] **Lock / unlock** (`DateLocked`, `LockedByAdminUserId`); change `AccountStatus`.
- [ ] Profile edits write a **`JobSeekerChangeLog`** audit row (preserve).
- [ ] Tests: search, edit + audit row, lock/unlock, status change.

**Docs:** `data-model.md` (AspNetUsers, JobSeekerChangeLog), `glossary.md`.

---

## ADM-3 — Job Seeker history & admin notes
- [ ] View **`JobSeekerVersions`** (profile snapshots), **`JobSeekerChangeLog`** (changes),
      **`JobSeekerEventLog`** (activity) for a seeker.
- [ ] **Admin comments**: `JobSeekerAdminComments` — add, pin (`IsPinned`), list.
- [ ] Tests: history views render; comment add/pin.

**Docs:** `data-model.md` (JobSeeker* tables). **Depends on:** ADM-2.

---

## ADM-4 — Impersonation
- [ ] Filament action starts an **impersonated seeker session** and writes an **`ImpersonationLog`**
      row (`Token`, `AspNetUserId`, `AdminUserId`, `DateTokenCreated`).
- [ ] End-impersonation returns to the admin; the audit trail is complete.
- [ ] Tests: impersonation writes audit; session switch; end.

**Docs:** `data-model.md` (ImpersonationLog), builds on FND-6 scaffold. **Depends on:** ADM-2.

---

## ADM-5 — Job / posting management
- [ ] Filament resource over `Jobs`: search (title / employer / `JobId` / source), view job detail.
- [ ] View **`JobVersions`** posting history for a job.
- [ ] **Read-only for job content** — jobs are owned by the ingestion pipeline; admin views, does not
      edit job fields. (Deny-list action via `DeletedJobs` only if that admin capability exists today.)
- [ ] Tests: browse/search; version history renders; job content not editable.

**Docs:** `data-model.md` (Jobs, JobVersions), `architecture.md §1a` (ingestion out of scope).

---

## ADM-6 — Admin account management
- [ ] Filament resource over `AdminUsers`: CRUD, `AdminLevel` (roles), **lock/unlock**
      (`DateLocked`, `LockedByAdminUserId`), `Deleted` flag; audit (`ModifiedByAdminUserId`, `DateUpdated`).
- [ ] **SuperAdmin-only.**
- [ ] Tests: CRUD, role assignment, lock, SuperAdmin gating.

**Docs:** `data-model.md` (AdminUsers), `ADR-003`.

---

## ADM-7 — System settings editor
- [ ] Filament resource over `SystemSettings`: list + edit **by `FieldType`** (Boolean / MultiLineText /
      Number / SingleLineText / Html).
- [ ] **SuperAdmin-only**; audit (`DateUpdated`, `ModifiedByAdminUserId`).
- [ ] **Redis cache invalidation on save** (so the web app picks up changes — mirrors the current
      `InvalidateCachedSettingsInTheApiSite`).
- [ ] **Proper server-side validation** per field type (numeric/range for Number) — improve on the
      current "not empty" check. (e.g. `shared.settings.minimumWage` must be a positive decimal.)
- [ ] Tests: edit each field type; cache invalidation fired; validation rejects bad Number; gating.

**Docs:** `data-model.md` (SystemSettings), `glossary.md` (SystemSetting), `architecture.md §9`.

---

## ADM-8 — Core reports (Dapper ported)
- [ ] Port the report query services to **`DB::select` + readonly DTOs** (verbatim SQL): Jobs by
      Region / City / Industry / Source, Top-20 NOC, NOC Summary, JobSeeker Accounts, JobSeekers by
      City / Location, JobSeeker Detail.
- [ ] Reports read `JobStats` / `JobSeekerStats` (populated by **`usp_GenerateJobStats` /
      `usp_GenerateJobSeekerStats`**, scheduled by **pg_cron**) and the TVFs
      (`tvf_GetJobsForDate` / `tvf_GetJobSeekersForDate`). The app **calls** these; it does not reimplement them.
- [ ] `@Param` → `:Param`; verify repeated named placeholders under `pdo_pgsql` (mechanical fallback if needed).
- [ ] **JobSeeker Detail Excel export** → OpenSpout / Laravel Excel (replace EPPlus).
- [ ] **Numeric parity:** for a frozen closed period, each report matches the current .NET output to the row.
- [ ] Tests: each report runs; parity diff on a frozen period.

**Docs:** `data-model.md §3–4` (stats tables, procs, TVFs), `LARAVEL-MIGRATION-PLAN` §6, `ADR-004`.

---

## ADM-9 — Reporting dashboard & chart widgets
- [ ] Filament dashboard: **stat-overview tiles** (active jobs, new this week, alerts sent, job seekers)
      + **chart widgets** (bar/line) for the tabular reports, with period / region / source filters
      that drive all widgets.
- [ ] Chart data comes from the **Reporting service** (ADM-8 queries) — presentation only in the widget.
- [ ] **Chart accessibility:** not colour-only (pair colour with labels/patterns), sufficient contrast,
      and a text/data-table alternative for each chart.
- [ ] Tests: widgets render from query data; a filter change updates the widgets.

**Docs:** `REPORTING-ENHANCEMENTS` (chart conversions), `architecture.md` (Reporting), copilot-instructions (Accessibility). **Depends on:** ADM-8.

---

## Definition of Done (epic)
- [ ] **Parity:** job-seeker management, job/posting browsing, admin accounts, system settings, and the
      full report set all reproduce current WorkBC.Admin behaviour.
- [ ] Impersonation + all audit trails (`JobSeekerChangeLog`, `ImpersonationLog`) preserved.
- [ ] System settings invalidate the web cache; reports match the current numbers on closed periods.
- [ ] Charts added **on top of** the core reports, accessible (not colour-only, contrast, text alt).
- [ ] `php artisan test` green; a11y checks passing.
