# EPIC — Job Seeker Account

**Goal:** The authenticated job-seeker experience — dashboard, saved jobs, job alerts (management
**and** the PHP email sender that replaces the .NET notifications container), recommended jobs,
saved career/industry profiles, and personal settings. Behind login → **Blade + Livewire** (no SEO
requirement); rich interactivity via Livewire, view state via Alpine.

**Applies to every story:** follow `.github/copilot-instructions.md`; the app **reads** derived
fields (never recomputes — Rule B); write each aggregate only via its owning **JobSeeker** service;
soft-delete uses the `IsDeleted`+`DateDeleted` trait (not Laravel `SoftDeletes`); WCAG 2.1 AA;
no PII in logs (FOIPPA); DoD + self-check.

**Depends on:** FND-1 (scaffold), FND-2 (models), FND-4 (layout/components), FND-5 (session auth),
FND-7 (`JobSearchFilters` + `JobSearchQuery`). Alerts/recommended also read OpenSearch.

**Order:** ACCT-1 first; ACCT-2/3/5/6/7 build on it; ACCT-4 (sender) after ACCT-3.

---

## ACCT-1 — Account dashboard
**Description:** The logged-in landing page and the account area's shared layout: summary,
persistent navigation, and the admin-managed copy that surrounds them.
- [x] Shows counts: saved jobs, active alerts, saved career/industry profiles (owning-service reads).
- [x] Navigation to each account area; only the authenticated user's own data (policy-scoped).
- [x] a11y: landmarks, headings, keyboard nav; Livewire for any live counts.
- [x] Test: dashboard renders per-user counts; another user's data is never shown.

**Layout parity (added 2026-07-30 — see Status below).** Compared against the live .NET account page:

- [ ] **No dead navigation.** The dashboard currently links to `/account/profiles` and
      `/account/settings`, which both **404** today (they arrive with ACCT-6 / ACCT-7). Either ship
      the target or hide the link — never link to a 404 from our own navigation.
- [ ] **Persistent account navigation** on *every* account page, not just the dashboard. Legacy has a
      standing bar — `Account Profile ｜ Jobs ▾ ｜ Careers & Industries ▾ ｜ Manage Account ▾` — plus a
      greeting ("Hello, {FirstName}"). Today you cannot get from Saved Jobs to Alerts without
      returning to the dashboard. Extract it as a shared account layout the other ACCT pages use.
- [ ] **Group the counts, and make the count the link.** Legacy uses three thematic cards, each with a
      heading, a description and count rows that *are* the links:
      - **Jobs** → Saved Jobs, Recommended Jobs *(count arrives with ACCT-5)*, Job Alerts
      - **Careers & Industries** → Saved Career Profiles, Saved Industry Profiles
      - **Manage Account** → Personal Settings

      Ours renders four flat stat tiles *plus* a separate "Go to" link row — the same concept twice.
- [ ] **`SystemSettings` read-through service** (cached, explicit invalidation per `architecture.md §9`).
      **Nothing in the app reads `SystemSettings` today** — only the model, its enum and an `AdminUser`
      relation exist — so every piece of admin-managed copy below is currently missing. This service is
      the first consumer and is reusable; **ADM-7** owns the editor that writes these values.
- [ ] **Admin-managed dashboard copy**, from the 21 `jbAccount.dashboard.*` keys (verified in the real
      DB):
      - `introText`, `jobsDescription`, `careersDescription`, `accountDescription`
      - `newAccountMessageTitle`/`Body` — the dismissible welcome banner
      - `notification1*`/`notification2*` — two banner slots, each with its own `Enabled` flag, so
        these are a **feature** (admin-toggleable announcements), not fixed copy
      - `resource1..3Title`/`Body`/`Url` — the "Recommended Resources" block
- [ ] Tests: persistent nav renders on every account page; no account link resolves to 404; grouped
      cards show the right counts and link targets; a disabled notification slot renders nothing;
      `SystemSettings` copy is read (not hardcoded) and cached.

> **Status (2026-07-30): reopened.** The counts, policy scoping and a11y shipped and are passing. The
> criteria above were added after comparing with the live .NET page: the delivered dashboard satisfies
> "navigation to each account area" literally, but it is a single page with a link list rather than an
> account *area* with persistent wayfinding — and two of those links 404. Nothing deploys until every
> Foundation/epic story is complete, so this is tracked here rather than as a separate parity pass.
> Note ADR-010: French comes from page translation, so this copy needs **no** `lang/fr` variants.

**Docs:** `data-model.md` (SavedJobs/JobAlerts/Saved*Profiles/SystemSettings); `architecture.md §9`
(caching); `ADR-010` (no French variants needed); copilot-instructions (Accessibility).
**Related:** ACCT-5 (Recommended Jobs count), ACCT-6/7 (the currently-dead link targets), ADM-7 (settings editor).

---

## ACCT-2 — Saved jobs
**Description:** Save/unsave jobs with an optional note; list them.
- [x] Save/unsave toggle (Livewire) from search results + job detail; writes `SavedJobs`
      (`AspNetUserId`, `JobId`, `DateSaved`).
- [x] 800-char `Note` with `NoteUpdatedDate`; add/edit inline.
- [x] Soft-delete (unsave) sets `IsDeleted`+`DateDeleted` (shared trait); list excludes deleted.
- [x] List shows current job data (join to `Jobs`); handles jobs that later expired.
- [x] Tests: save, note add/edit, unsave (soft), list scoping, 800-char validation.

**Docs:** `data-model.md` (SavedJobs), `glossary.md` (soft-delete). **Depends on:** ACCT-1, SRCH-1/7.

---

## ACCT-3 — Job alerts management (create / edit / list / delete)
**Description:** Create and manage saved-search alerts, reusing the search filter UI.
- [x] Create/edit reuses the **search filter components** in "alert" mode; stores the criteria as
      **`JobSearchFilters` JSON** in `JobAlerts.JobSearchFilters` (+ `JobSearchFiltersVersion`),
      plus `Title`, `AlertFrequency` (daily/weekly/biweekly/monthly), `UrlParameters`.
- [x] **Live match-count preview** (Livewire) — runs the current filters against OpenSearch and shows
      the total (read-only; `PageSize=0`).
- [x] List active alerts; **delete** = soft-delete (`IsDeleted`+`DateDeleted`) **and** writes a
      `JobSeekerChangeLog` audit row (preserve this side effect).
- [x] `JobSearchFilters` version 0/1 handled (contracts.md §1); `UrlParameters` stays alert/search-compatible (SRCH-6).
- [x] Tests: create→stored JSON round-trips; edit; live count; delete soft + audit row written.

**Docs:** `contracts.md §1`, `data-model.md` (JobAlerts), `glossary.md`. **Depends on:** ACCT-1, FND-7, SRCH-1..6.

---

## ACCT-4 — Job alert email sender (PHP — replaces the .NET container)
**Description:** Replace `WorkBC.Notifications.JobAlerts` with a PHP artisan command that sends alert
emails; **cadence is configured by Kubernetes CronJob(s)**.
- [ ] `php artisan alerts:send --frequency={daily|weekly|biweekly|monthly}` — a **k8s CronJob per
      frequency** sets how often each runs (cadence lives in the CronJob schedule). Preserve the
      current semantics (daily always; weekly Mon; biweekly 15th + last day; monthly 1st) — whether
      via per-frequency CronJobs or a daily run with internal fan-out (document the choice).
- [ ] Selects eligible alerts: not deleted, owner `AccountStatus = Active`, matching frequency.
- [ ] Runs each alert's stored `JobSearchFilters` against OpenSearch; builds the email of matches.
- [ ] Sends via **SES / SendGrid / SMTP** (transport chosen by config); email settings from
      `SystemSettings` `email.%` keys.
- [ ] Runs as a **queued** job (`BaseJob`); **idempotent** — a given alert is not double-sent for the
      same period; failures logged, not fatal to the batch.
- [ ] Tests: frequency selection (each cadence), filter→matches, email rendered, idempotency, transport selection.

**Docs:** `ADR-004` (scheduling), `contracts.md §1`, `data-model.md` (JobAlerts, SystemSettings),
`architecture.md §5.4, §9`. **Depends on:** ACCT-3, FND-3, FND-7.

---

## ACCT-5 — Recommended jobs
**Description:** Personalized recommendations from the seeker's saved-job signals.
- [ ] Builds a boost-weighted OpenSearch query from the user's saved-job **NOC 2021 / employers /
      titles / city** (with the per-item boost increments), `minimum_should_match`, and
      `IgnoreJobIdList` (exclude already-saved).
- [ ] Reads OpenSearch only; respects the base `ExpireDate >= now` filter.
- [ ] Empty-state when the user has no saved jobs / no matches.
- [ ] Tests: query composition from saved signals; excludes saved jobs; empty state.

**Docs:** `architecture.md §7`, current `RecommendedJobsQuery`. **Depends on:** ACCT-2, FND-7.

---

## ACCT-6 — Saved career & industry profiles
**Description:** Save/remove career (NOC) and industry profiles; expose save/status for Drupal.
- [x] Career profiles: `SavedCareerProfiles` (`NocCodeId2021`), soft-delete; industry:
      `SavedIndustryProfiles` (`IndustryId`), soft-delete.
- [x] `POST /api/career-profiles/save/{id}` + `GET …/status/{id}` (+ industry equivalents),
      **session-authenticated per `ADR-009`** — `contracts.md §2.4`'s server-to-server framing is
      retired; these are called from the browser by Drupal's profile pages.
- [x] List saved profiles with their NOC/industry titles.
- [x] Tests: save, status, remove (soft), list scoping; API auth enforced.

**Status (2026-07-30): done.** 13 tests (`tests/Feature/Account/SavedProfilesTest.php`), verified live
against the restored DB. Notes for whoever touches this next:

- **`profileId` is the `NocCodeId2021` / `IndustryId` value itself** — not the row's `Id`, and not
  `EDM_CareerProfile_CareerProfileId` (which the .NET controller always wrote as null and never read;
  mapped but never populated here either).
- Save is **insert-if-absent**, never a toggle; remove is **soft-delete only** (the legacy
  hard-delete line is commented out deliberately). One divergence, matching `SavedJobService`: a
  soft-deleted row is **restored** rather than duplicated, so the table stops growing per save/unsave
  cycle. Observably identical.
- Industry titles join **`Industries.TitleBC`**, not `Title` (falls back to `Title` when blank).
- Routes live in **`web.php`, not `api.php`**, despite the `api/` path — they need the `web` group's
  session/cookie/CSRF, and Laravel 12's `api` group is throttle + `SubstituteBindings` only, so a
  session cookie arriving there is ignored. The `api/` prefix is kept because Drupal's
  `WorkbcJobboardSaveProfile` block builds these URLs.
- `EnsureJobSeekerToken` is retired in favour of `EnsureJobSeekerSession`, which returns **401**
  rather than using `auth:web` — that would 302 to the login page for Drupal's wildcard `Accept`
  header, and the caller would read login HTML as success.
- `status` returns `{ saved, csrf }`; the token is in the body because Drupal's cross-origin JS
  cannot read our `XSRF-TOKEN` cookie (ADR-009).
- Closes the `/account/profiles` 404 from ACCT-1's "no dead navigation" criterion.
  **`/account/settings` is still dead** until ACCT-7.

**Docs:** **`ADR-009`** (session auth; supersedes `contracts.md §2.4`), `data-model.md`
(Saved*Profiles), `integration/api-status.md`. **Depends on:** ACCT-1.

---

## ACCT-7 — Personal settings
**Description:** Profile management and password change.
- [ ] Edit profile: `FirstName`, `LastName`, `City`, `LocationId`, `CountryId`, `ProvinceId`;
      writes maintain `NormalizedEmail`/`NormalizedUserName` where email changes.
- [ ] Change password → **rehash to bcrypt/argon2** (FND-5 hasher); no MD5.
- [ ] Profile changes write a `JobSeekerChangeLog` audit row (preserve).
- [ ] a11y: labelled inputs, error messaging via ARIA live.
- [ ] Tests: profile update + audit row; password change rehashes; validation.

**Docs:** `ADR-003`, `data-model.md` (AspNetUsers, JobSeekerChangeLog). **Depends on:** ACCT-1, FND-5.

---

## Definition of Done (epic)
- [ ] All account features reproduce current behaviour; per-user data isolation enforced everywhere.
- [ ] Alerts store the `JobSearchFilters` contract (v0/1) and the **PHP sender replaces the .NET
      container**, scheduled by k8s CronJob, with current frequency semantics preserved.
- [ ] Audit side effects (`JobSeekerChangeLog`) preserved; soft-deletes correct.
- [ ] `php artisan test` green; a11y checks passing.
