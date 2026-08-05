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
| `GET/POST/DELETE /api/career-profiles/…` | **Built** (ACCT-6) — session-authenticated, not server-to-server | `SavedProfilesTest` (13 passing, shared with industry) | Drupal NOC/career pages — "save this career profile" button (browser-side) |
| `GET/POST/DELETE /api/industry-profiles/…` | **Built** (ACCT-6) — previously did not exist at all | `SavedProfilesTest` | Drupal industry pages — "save this industry" button (browser-side) |
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

## 2. Total jobs — `GET /api/Search/GetTotalJobs`

Built. Backs Drupal's `hook_cron` → `state('jobboard_total_jobs')`, which is the "Search N jobs"
count shown sitewide (not just on the job board pages). Confirmed it counts active jobs only and
doesn't fetch hit documents (count-only query, no per-request index scan cost).

**Returns a bare integer** (`42802`), not `{"count": n}` — see contracts.md §2.2 for why this was
recorded incorrectly and how it surfaced as "Search NaN jobs in B.C.". Accepts the language as
either a path segment or `?language=`, and answers to both `GetTotalJobs` and `gettotaljobs`.

## 3. City autocomplete — `GET /api/Location/cities/{name}/{includeRegion}`

Built. Path-param style (`.../cities/Surrey/true`), matching the legacy contract exactly — not a
query string. Backs the location combobox's suggestion list on both the Drupal-proxied
`/api/getCities` route and (inside the app itself) the search page's own city input.

## 4 & 5. Career and industry profile save/status — built (ACCT-6)

Both sets are live and behave identically; the industry endpoints previously did not exist at all.

```
GET    /api/{career|industry}-profiles/status/{profileId}   → { saved, csrf }
POST   /api/{career|industry}-profiles/save/{profileId}     → { saved: true }
DELETE /api/{career|industry}-profiles/{profileId}          → { saved: false, removed }
```

**These are not server-to-server.** They are called from the browser by Drupal's profile pages and
authenticate with the seeker's session cookie (ADR-009). Consequences worth knowing:

- **Defined in `web.php`, not `api.php`**, despite the `api/` path. They need the `web` group's
  session/cookie/CSRF; Laravel 12's `api` group is throttle + `SubstituteBindings` only, so a session
  cookie arriving there is ignored. The `api/` prefix is kept because Drupal's
  `WorkbcJobboardSaveProfile` block builds these URLs.
- **`EnsureJobSeekerSession` returns 401**, replacing the retired `EnsureJobSeekerToken`. Deliberately
  not `auth:web`: that only 401s when the request `expectsJson()`, and Drupal's JS sends a wildcard
  `Accept`, so it would 302 to the login page and the caller would read login HTML as success.
- **The CSRF token is in the `status` body.** Drupal's JS is cross-origin and cannot read our
  `XSRF-TOKEN` cookie, so double-submit can't work. Status is already fetched on page load, so the
  save POST gets its token without an extra round-trip. Note `X-XSRF-TOKEN` still needs adding to the
  CloudFront CORS allowlist — an infra change, not code (ADR-009).
- **`profileId` is the `NocCodeId2021` / `IndustryId` itself** — not the row `Id`, and not
  `EDM_CareerProfile_CareerProfileId` (always null in the legacy code).
- Save is **insert-if-absent**; remove is **soft-delete only**. A soft-deleted row is restored rather
  than duplicated (divergence from legacy, matching `SavedJobService`).

Listing lives at `/account/profiles` (`SavedProfilesPage`), joining `NocCodes2021.Title`/`Code` and
`Industries.TitleBC`. The dashboard's counts still come from `JobSeekerDashboardService::summaryFor()`,
a read-only query-builder call rather than this API.

Tests: `tests/Feature/Account/SavedProfilesTest.php` (13), plus a live round-trip against the restored
DB — saved NOC 00010 and industry 1, titles joined, soft-delete confirmed, anonymous calls 401.

## 6. Dead endpoint — `career-profiles/topjobs/{noc}`

Documented as dead in both `contracts.md §2.5` and `glossary.md` — Drupal never calls it (uses
`Search/JobSearch` instead). Not implemented here and doesn't need to be; safe to leave out of the
rewrite entirely.

---

## What's left to reach full parity

**Nothing in this app.** All five endpoints are built and tested; the sixth (`topjobs`) is dead by
design. Two items sit outside the codebase:

1. **Infra:** add `X-XSRF-TOKEN` to the CloudFront `cors-api-jobboard` response-headers policy, or the
   save POST's preflight will be rejected. CORS here is CDN-owned (`OriginOverride: true`), so this is
   an infra change and `config/cors.php` has no effect (ADR-009).
2. **Drupal:** the profile pages' JS must drop its `Authorization` header, add
   `xhrFields: { withCredentials: true }`, branch on **401** instead of token presence, and send the
   CSRF token from the status response — see `drupal-embed.md §3` surface 3.

No other SRCH-10 work is outstanding — job search, total jobs, and city autocomplete are complete
and match the documented contract.
