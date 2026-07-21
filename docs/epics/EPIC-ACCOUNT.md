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
**Description:** The logged-in landing page: summary + navigation to account features.
- [ ] Shows counts: saved jobs, active alerts, saved career/industry profiles (owning-service reads).
- [ ] Navigation to each account area; only the authenticated user's own data (policy-scoped).
- [ ] a11y: landmarks, headings, keyboard nav; Livewire for any live counts.
- [ ] Test: dashboard renders per-user counts; another user's data is never shown.

**Docs:** `data-model.md` (SavedJobs/JobAlerts/Saved*Profiles); copilot-instructions (Accessibility).

---

## ACCT-2 — Saved jobs
**Description:** Save/unsave jobs with an optional note; list them.
- [ ] Save/unsave toggle (Livewire) from search results + job detail; writes `SavedJobs`
      (`AspNetUserId`, `JobId`, `DateSaved`).
- [ ] 800-char `Note` with `NoteUpdatedDate`; add/edit inline.
- [ ] Soft-delete (unsave) sets `IsDeleted`+`DateDeleted` (shared trait); list excludes deleted.
- [ ] List shows current job data (join to `Jobs`); handles jobs that later expired.
- [ ] Tests: save, note add/edit, unsave (soft), list scoping, 800-char validation.

**Docs:** `data-model.md` (SavedJobs), `glossary.md` (soft-delete). **Depends on:** ACCT-1, SRCH-1/7.

---

## ACCT-3 — Job alerts management (create / edit / list / delete)
**Description:** Create and manage saved-search alerts, reusing the search filter UI.
- [ ] Create/edit reuses the **search filter components** in "alert" mode; stores the criteria as
      **`JobSearchFilters` JSON** in `JobAlerts.JobSearchFilters` (+ `JobSearchFiltersVersion`),
      plus `Title`, `AlertFrequency` (daily/weekly/biweekly/monthly), `UrlParameters`.
- [ ] **Live match-count preview** (Livewire) — runs the current filters against OpenSearch and shows
      the total (read-only; `PageSize=0`).
- [ ] List active alerts; **delete** = soft-delete (`IsDeleted`+`DateDeleted`) **and** writes a
      `JobSeekerChangeLog` audit row (preserve this side effect).
- [ ] `JobSearchFilters` version 0/1 handled (contracts.md §1); `UrlParameters` stays alert/search-compatible (SRCH-6).
- [ ] Tests: create→stored JSON round-trips; edit; live count; delete soft + audit row written.

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
- [ ] Career profiles: `SavedCareerProfiles` (`NocCodeId2021`), soft-delete; industry:
      `SavedIndustryProfiles` (`IndustryId`), soft-delete.
- [ ] `POST /api/career-profiles/save/{id}` + `GET …/status/{id}` (+ industry equivalents),
      authenticated per `contracts.md §2.4`.
- [ ] List saved profiles with their NOC/industry titles.
- [ ] Tests: save, status, remove (soft), list scoping; API auth enforced.

**Docs:** `contracts.md §2.4`, `data-model.md` (Saved*Profiles). **Depends on:** ACCT-1.

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
