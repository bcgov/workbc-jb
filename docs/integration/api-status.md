# Drupal-facing API — build status

**Purpose:** a single, accurate answer to "what's actually built vs. documented/planned" for the
API in `routes/api.php`. `contracts.md §2` is the contract shape; `drupal-embed.md §6` is the
parity checklist Drupal needs; this doc is the status against both, verified against code and a
live test run (not just doc checkboxes, which had drifted out of date).

> **Updated 2026-07-29 ([ADR-009](../adr/ADR-009-same-site-session-auth-for-embed.md)):** the
> career/industry profile endpoints are **no longer a server-to-server contract**. Only §1–§3 below
> are genuinely Drupal-PHP-to-backend. §4/§5 become session-authenticated browser-called routes, which
> changes what "built" means for them — see those sections.

---

## Summary

| Endpoint | Status | Tests | Used by |
|---|---|---|---|
| `POST /api/Search/JobSearch(/{lang})` | **Built** | `JobSearchApiTest` (7 passing) | Drupal: career/NOC pages, industry pages, region/city pages, "Recent Jobs" widgets |
| `GET /api/Search/gettotaljobs(/{lang})` | **Built** | `TotalJobsApiTest` (2 passing) | Drupal `hook_cron` → sitewide "Search N jobs" count |
| `GET /api/location/cities/{name}/{includeRegion}` | **Built** | `LocationCitiesApiTest` (2 passing) | Drupal `/api/getCities` proxy → city autocomplete widget |
| `POST/GET /api/career-profiles/{save,status}/{id}` | **Stub, on a wrong premise** — needs rework, not extension | `CareerProfileApiTest` (5 passing — assert *bearer* behaviour, so they go too) | Drupal NOC/career pages — "save this career profile" button (browser-side, not server-to-server) |
| `POST/GET /api/industry-profiles/{save,status}/{id}` | **Not built** — no route, no controller | none | Drupal industry pages — "save this industry" button (currently has nowhere to call) |
| `GET /api/career-profiles/topjobs/{noc}` | **Dead** — not called by Drupal (superseded by `Search/JobSearch`) | n/a | Removable, not a gap |

Verified 2026-07-27 by reading `routes/api.php` + each controller, then running the full suite in
the Sail container: `php artisan test --filter="JobSearchApiTest|TotalJobsApiTest|LocationCitiesApiTest|CareerProfileApiTest"`
→ **16 passed, 47 assertions**.

---

## 1. Job search — `POST /api/Search/JobSearch`

Fully built. `JobSearchRequest::passedValidation()` runs the request body through the same
`JobSearchFilters::fromArray()` used by the search UI and saved alerts (Rule: one shared value
object, §1 of `contracts.md`) — any field it doesn't recognise is rejected with **400**, not
silently dropped. `SearchApiController::jobSearch()` delegates to `JobSearchService::searchForApi()`,
which reuses the **same** `JobSearchQuery` the public `/jobs` page uses — this is a response-shape
layer on top of the existing query engine, not a second one.

Confirmed working, including the two behavioral contracts Drupal's widgets depend on:
- **Federal-first heuristic** — a NOC filter + `PageSize ≤ 10` + no source pinned returns federal
  (NJB) jobs first, falling back to external when there aren't enough. This is what makes Drupal's
  "Recent Jobs" sidebar on a career-profile page show real federal listings first.
- **Response casing** — camelCase wrapper (`count`/`result`/`pageNumber`/`pageSize`) around
  PascalCase job items, exactly matching the legacy shape (`contracts.md §2.1`), so Drupal's
  existing JS/Twig that reads `result[].JobId` etc. doesn't need to change.

**Used by (Drupal side, per `WorkBcJobboardController.php`):** career/NOC pages (send
`SearchNocField`), industry pages (send `SearchIndustry`), region/city pages (send
`SearchLocations`) — all via the same endpoint, just a different filter subset in the request body.

## 2. Total jobs — `GET /api/Search/gettotaljobs`

Built. Backs Drupal's `hook_cron` → `state('jobboard_total_jobs')`, which is the "Search N jobs"
count shown sitewide (not just on the job board pages). Confirmed it counts active jobs only and
doesn't fetch hit documents (count-only query, no per-request index scan cost).

## 3. City autocomplete — `GET /api/location/cities/{name}/{includeRegion}`

Built. Path-param style (`.../cities/Surrey/true`), matching the legacy contract exactly — not a
query string. Backs the location combobox's suggestion list on both the Drupal-proxied
`/api/getCities` route and (inside the app itself) the search page's own city input.

## 4. Career profile save/status — stub built on a premise that turned out to be false

`CareerProfileApiController` is a deliberate stub (`save()` returns `true` without writing;
`status()` returns `false`, avoiding a false "saved" state). Persistence was scoped to **ACCT-6**.

What changed: it sits behind `EnsureJobSeekerToken`, which requires a **bearer token** because
`contracts.md §2.4` described Drupal forwarding an `Authorization` header. Verified against the live
Drupal module, that never happened — the calls come **from the browser**, and the forwarding code in
Drupal is dead. Under [ADR-009](../adr/ADR-009-same-site-session-auth-for-embed.md) these become
**session-authenticated** routes returning **401** when anonymous.

So ACCT-6 is partly **rework**, not extension:

- `EnsureJobSeekerToken` is replaced by session auth (its 5 tests assert bearer behaviour and go with it)
- The routes need **session middleware** — `routes/api.php` uses the `api` group, which in Laravel 12 is
  throttle + `SubstituteBindings` only, **no session/cookies**. A session cookie arriving here today
  would be ignored entirely. Either add session middleware to this group or move these routes to `web.php`
- `status` must also return a **CSRF token** in its body (`{ saved, csrf }`) — the `XSRF-TOKEN` cookie is
  unreadable cross-origin, so the usual double-submit pattern can't work (ADR-009)

The app's own dashboard already reads `SavedCareerProfiles` directly for its **count**
(`JobSeekerDashboardService::summaryFor()`) — a read-only query-builder call, not via this API.

**Used by:** the "Save this career profile" button on Drupal's NOC/career pages — browser-side.

## 5. Industry profile save/status — not built

No route, no controller — `routes/api.php` only has the `career-profiles/*` group. A real gap, not an
unfinished stub. Already tracked inside **ACCT-6** ("+ industry equivalents"), so no new story is
needed; but until ACCT-6 lands, a Drupal industry page has nothing to call. Same session-auth and CSRF
shape as §4.

**Would be used by:** the "Save this industry" button on Drupal's industry pages (parallel to #4).

## 6. Dead endpoint — `career-profiles/topjobs/{noc}`

Documented as dead in both `contracts.md §2.5` and `glossary.md` — Drupal never calls it (uses
`Search/JobSearch` instead). Not implemented here and doesn't need to be; safe to leave out of the
rewrite entirely.

---

## What's left to reach full parity

Everything left lives in **ACCT-6** (depends on ACCT-1, which is done):

1. `SavedCareerProfiles` / `SavedIndustryProfiles` Eloquent models + `SoftDeletesByFlag`.
2. **Swap the auth model** (ADR-009): retire `EnsureJobSeekerToken`, put the routes on session
   middleware, return 401 when anonymous, and return a CSRF token from `status`.
3. Real save/remove/status logic replacing the stub + a parallel `IndustryProfileApiController`
   and its two routes. Port the legacy semantics: `profileId` is the **`NocCodeId2021` / `IndustryId`
   directly** (not `EDM_CareerProfile_CareerProfileId`, which the .NET code always wrote as `null`);
   save is **insert-if-absent** (no duplicate active rows, no toggle); remove is **soft-delete only**.
4. A "list saved profiles with titles" view for the account dashboard (currently only a count).
   Note the legacy list joins industry titles on **`Industries.TitleBC`**, not `Title`.
5. Tests: save, status, remove (soft), list scoping, session-auth enforcement, CSRF, for both types.

No other SRCH-10 work is outstanding — job search, total jobs, and city autocomplete are complete
and match the documented contract.
