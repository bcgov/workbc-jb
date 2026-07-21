# EPIC — Public Job Search

**Goal:** The public, SEO-critical job search — server-rendered results, the full faceted filter
set, job detail pages, shareable/alert-compatible URLs, the map view, and the Drupal-facing search
API. Behaviour parity with the current search (see current `JobSearchQuery`/`JobSearchFilters`),
built on the FND-7 search foundation.

**Applies to every story:** follow `.github/copilot-instructions.md`; server-rendered **Blade +
Livewire** (ADR-002); reads only OpenSearch + `Jobs` (never recompute derived fields — Rule B);
`America/Vancouver` for all date logic; WCAG 2.1 AA on all UI; DoD + self-check per copilot-instructions.

**Depends on:** FND-1 (scaffold), FND-4 (layout/components), FND-7 (`JobSearchFilters` + `JobSearchQuery`).

**Order:** SRCH-1 first; facets (2–5) build on SRCH-1; SRCH-6 after facets; SRCH-7/8/9/10 can run in parallel after SRCH-1.

---

## SRCH-1 — Search results page (core + sort & paging)
**Description:** The server-rendered search page: keyword + search-by inputs, results list, base
expiry filter, the 11 sort orders, and pagination — Livewire-driven, crawlable HTML.

**Acceptance criteria**
- [ ] Blade page renders results **server-side** (results present in initial HTML, not injected client-side).
- [ ] Keyword input + **search-by** scope (`all` | `title` | `employer` | `jobId`); keyword parsed
      per the ported keyword parser (BRD rules: spaces=AND, commas/pipes=OR, quoted phrases).
- [ ] Base filter on every query: `ExpireDate >= now/d` (America/Vancouver); `track_total_hits`.
- [ ] All **11 sort orders** + secondary sort (`DatePosted desc, JobId.keyword asc`); default DatePosted↓.
- [ ] Pagination (`Page`/`PageSize`), result count shown; results update via Livewire.
- [ ] Each result links to the **path-based** job detail URL (SRCH-7), not a hash route.
- [ ] a11y: results region is an ARIA live region; keyboard operable; visible focus.
- [ ] Livewire component test: keyword + sort + page produce the expected `JobSearchQuery` body/results.

**Docs:** `contracts.md §1` (SortOrder enum), `architecture.md §5.3, §7`, copilot-instructions (Frontend, Accessibility).

---

## SRCH-2 — Location input & filter (autocomplete, validation, radius)
**Description:** City/postal input with autocomplete + validation, the Location facet, geocoding,
and radius search — including virtual-job inclusion.

**Acceptance criteria**
- [ ] City/postal input with **autocomplete** (city suggestions) and blur **validation**
      (invalid city / invalid postal), errors announced via ARIA live.
- [ ] Location facet: one or many `LocationField` (city / region / postal); `Postal` normalized
      (uppercase, no spaces).
- [ ] Radius: `SearchLocationDistance` (`-1` exact, or km) → geocode via **`GeocodedLocationCache`**
      first, Google Maps fallback, write-back to cache; emit `geo_distance` (+ `_geo_distance` sort).
- [ ] **Virtual jobs force-included** in any location filter (`WorkplaceType.Id 15141`, boost 0).
- [ ] Invalid-location fallback matches current behaviour (1 km circle at lat 0/lon 180 → no results).
- [ ] Tests: exact city, city+radius (cache hit + Google fallback), postal, multi-location, region.

**Docs:** `contracts.md §1` (Location, LocationField), `glossary.md` (WorkplaceType), `architecture.md §7`.

---

## SRCH-3 — Standard filter facets (Job Type, Industry, Education, Date)
**Description:** The straightforward faceted filters as accessible dropdowns.

**Acceptance criteria**
- [ ] **Job Type:** hours (FT/PT/leading-to-FT), period (permanent/temporary/casual/seasonal),
      terms (day…weekend), workplace (on-site/hybrid/travelling/virtual) → correct term filters.
- [ ] **Industry:** `SearchIndustry[]` (NAICS ids) → `NaicsId` terms.
- [ ] **Education:** `SearchJobEducationLevel[]` → `EduLevel.keyword` terms.
- [ ] **Date:** `SearchDateSelection` (any/today/past-3-days/range) with `StartDate`/`EndDate`;
      range uses end-of-day inclusive and `time_zone America/Vancouver`.
- [ ] Facet dropdowns: Alpine for open/close (view state), Livewire applies filters (data).
- [ ] Tests: each facet produces the expected query group; multiple facets AND across groups, OR within.

**Docs:** `contracts.md §1`, `architecture.md §7`. **Depends on:** SRCH-1.

---

## SRCH-4 — Salary filter facet
**Description:** Salary type, brackets, custom range, unknown, and salary-condition benefits.

**Acceptance criteria**
- [ ] `SalaryType` (hourly/weekly/bi-weekly/monthly/annually) with correct annualization
      (hourly ×2080, weekly ×52, bi-weekly ×26, monthly ×12) for custom min/max.
- [ ] Fixed brackets 1–5 + custom (bracket 6) → `Salary` range filters.
- [ ] **Unknown salary** → `SalarySort.Descending <= -99999999` (the sentinel).
- [ ] `SearchSalaryConditions[]` → `SalaryConditions.Description.keyword` terms.
- [ ] Tests: each bracket, custom range with each SalaryType, unknown, conditions.

**Docs:** `contracts.md §1` (Salary), `glossary.md`, `architecture.md §7`. **Depends on:** SRCH-1.

---

## SRCH-5 — "More" filter facet
**Description:** Equity groups, posting language, NOC, job source, and placement-agency exclusion.

**Acceptance criteria**
- [ ] Equity groups (apprentice, veterans, Indigenous, mature, newcomers, disabilities, students,
      visible minority, youth) → the correct `Is*` term filters.
- [ ] Posting language: English / English+French (`IsFederalJob` for E+F).
- [ ] `SearchNocField` → `Noc2021` term; `NocCode` handled per contract.
- [ ] **Job source** (`0`–`5`) → federal flag / non-federal / the three nested `ExternalSource`
      match_phrase groups (federal-gov, municipal, BC-provincial) per current mapping.
- [ ] **Exclude placement agencies** → `must_not { term EmployerTypeId: 1 }`.
- [ ] Tests: each group; job-source 1–5; exclude-agencies.

**Docs:** `contracts.md §1` (SearchJobSource enum), `architecture.md §7`. **Depends on:** SRCH-1.

---

## SRCH-6 — Filter/URL serialization (shareable + alert-compatible)
**Description:** Two-way mapping between the URL query string and `JobSearchFilters`, compatible
with saved alerts.

**Acceptance criteria**
- [ ] Filters serialize to a shareable URL; loading that URL reconstructs the exact `JobSearchFilters`.
- [ ] **Alert compatibility:** the format is compatible with `JobAlerts.UrlParameters` deep-links,
      OR a documented migration + redirect shim is provided for previously-sent alert emails.
- [ ] Round-trip test: filters → URL → filters is lossless for every facet.
- [ ] `JobSearchFilters` version 0/1 handled (per `contracts.md §1`).

**Docs:** `contracts.md §1` (versioning), `LARAVEL-MIGRATION-PLAN` risk #3 (URL contract). **Depends on:** SRCH-2–5.

---

## SRCH-7 — Job detail page (SEO)
**Description:** The server-rendered job detail page — crawlable, structured data, view counting.

**Acceptance criteria**
- [ ] **Path-based, crawlable URL** (e.g. `/jobs/{slug}-{JobId}`); no hash routing.
- [ ] Fetches by id (no `ExpireDate` filter — a linked expired job still renders).
- [ ] Emits **`schema.org/JobPosting`** structured data (JSON-LD): title, employer, location,
      datePosted, validThrough, salary where present; `<title>`/meta description; canonical URL (EN/FR).
- [ ] Increments `JobViews` (via the JobSeeker/Job read path); print + share actions.
- [ ] a11y: semantic headings, landmarks, keyboard operable.
- [ ] Tests: renders in raw HTML (crawlable), structured data valid, view count increments.

**Docs:** `contracts.md §2.1` (job fields), `architecture.md §6`, `glossary.md`. **Depends on:** SRCH-1.

---

## SRCH-7b — External (Innovibe) job detail rendering
**Description:** Render the external job's description (stored in OpenSearch as `JobDescription`) on
**our own** detail page with a button to the original source — instead of redirecting away.
**Innovibe terms permit displaying the description (confirmed).** This turns ~25k external jobs from
redirects into indexable pages (major SEO gain) and unifies external/federal behaviour.

**Acceptance criteria**
- [ ] For external jobs (`IsFederalJob = false` / JobSourceId 2), the detail page renders
      `JobDescription` (from OpenSearch) + job facts (title, employer, city, salary, dates).
- [ ] Prominent **"Apply on {OriginalSource}" / "View original posting"** button → `ExternalSourceUrl`,
      plus a **"via {source}"** attribution line.
- [ ] Detail query `_source` includes `JobDescription`, `ExternalSource`/`ExternalSourceUrl`, `OriginalSource`.
- [ ] Description **HTML-sanitized** before render (XSS); plain text rendered with safe line breaks.
- [ ] Server-rendered + `JobPosting` structured data + **self-canonical** (avoid duplicate-content penalties).
- [ ] **Expired-posting state:** by-id fetch renders even if expired; show "may no longer be available"; external link may be dead.
- [ ] **Linking change:** external jobs in search results **and** the Drupal widget link to our
      detail page (not the raw external URL).
- [ ] Tests: external job renders description + apply button + attribution; sanitization; canonical present; expired state.

**Docs:** `architecture.md §6`, `contracts.md §2.1`, `glossary.md` (JobSource). **Depends on:** SRCH-7.

---

## SRCH-8 — Job sitemap (SEO)
**Description:** `sitemap.xml` listing active job URLs for crawlers.

**Acceptance criteria**
- [ ] `sitemap.xml` (or index + shards) lists active (`ExpireDate >= now`) job detail URLs, EN/FR.
- [ ] Regenerated on a schedule (k8s CronJob → artisan, ADR-004) or served dynamically with caching.
- [ ] Excludes expired/inactive jobs; `lastmod` reflects `LastUpdated`.
- [ ] Test: sitemap contains a known active job and excludes a known expired one.

**Docs:** `architecture.md §6`, `ADR-004`. **Depends on:** SRCH-7.

---

## SRCH-9 — Map view (Google Maps + clustering)
**Description:** The geographic results view.

**Acceptance criteria**
- [ ] Map renders job pins from the **same filters** (via the map query path), clustered.
- [ ] Pin-selection logic matches current behaviour (most-frequent city/region; multi-location handling; 5000 cap).
- [ ] Google Maps JS wrapped in an Alpine component; API key from config/Secrets Manager.
- [ ] Info-window content per job; links to the detail page.
- [ ] a11y: a non-map (list) equivalent remains available (map is not the only way to access results).

**Docs:** `architecture.md §5.3, §7`, current `GetGoogleMapResults`/`GetMapPins`. **Depends on:** SRCH-1.

---

## SRCH-10 — Drupal-facing search API
**Description:** The JSON API consumed by the Drupal WorkBC.ca site.

**Acceptance criteria**
- [ ] `POST /api/Search/JobSearch` (+ `/{language}`): accepts a `JobSearchFilters` body; returns the
      `SearchResultsModel` shape with the **exact keys/casing** per `contracts.md §2.1`.
- [ ] **Strict validation:** unknown request fields → **400** (Form Request, fail closed).
- [ ] **Profile-sidebar heuristic:** NOC filter + `PageSize ≤ 10` + no source pinned ⇒ **federal-first**
      (NJB results first, external fallback).
- [ ] `GET /api/Search/gettotaljobs` → `{ count }`; `GET /api/location/cities/{name}/true` → string[].
- [ ] Responses use API Resources; additive-only contract (ADR/version for breaking).
- [ ] Feature tests: search shape + casing; unknown field → 400; federal-first heuristic; total; cities.

**Docs:** `contracts.md §2`, `architecture.md §8`. **Depends on:** SRCH-1, FND-7.

---

## Definition of Done (epic)
- [ ] Public search reproduces current behaviour across all facets, sort, and the map (diff-tested where possible).
- [ ] Job pages are **server-rendered and crawlable** with `JobPosting` structured data + sitemap —
      **including external jobs** (description rendered on-site, apply button to source).
- [ ] The Drupal API matches `contracts.md` exactly (keys, casing, federal-first, strict validation).
- [ ] Filters round-trip through URLs and remain alert-compatible.
- [ ] `php artisan test` green; a11y checks passing.
