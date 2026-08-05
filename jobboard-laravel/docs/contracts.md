# Job Board — Contracts

> **Purpose:** The published, shared contracts. **`JobSearchFilters`** is shared across the search
> form, saved alerts, and the alert email sender — change it and old alert rows stop deserializing.
> The **Drupal-facing API** is consumed server-to-server by WorkBC.ca. Treat both as breaking-change-
> controlled: additive changes only; anything breaking needs a **version bump + ADR**.

---

## 1. `JobSearchFilters` value object

The single search-criteria object used by **three** consumers:
1. the **search UI** (Livewire) → builds it from form input,
2. **saved alerts** → stored as JSON in `JobAlerts.JobSearchFilters`,
3. the **alert email sender** → re-runs the stored filters.

Implement as **one PHP value object** (a `Castable`) used by all three. **Field names and shape
must match the existing serialization exactly** — existing `JobAlerts` rows must keep deserializing.

### Fields (grouped; names are exact)
**Paging / sort**
- `Page` (int), `PageSize` (int, default 20), `SortOrder` (int — see enum below)

**Keyword**
- `Keyword` (string), `SearchInField` (string: `all` | `title` | `employer` | `jobId`; default `all`)

**Date**
- `SearchDateSelection` (string: `0` any | `1` today | `2` past-3-days | `3` range)
- `StartDate`, `EndDate` — `DateField` objects (see below), used when `SearchDateSelection = 3`

**Job type — hours**
- `SearchJobTypeFullTime`, `SearchJobTypePartTime`, `SearchJobTypeLeadingToFullTime` (bool)

**Job type — period**
- `SearchJobTypePermanent`, `SearchJobTypeTemporary`, `SearchJobTypeCasual`, `SearchJobTypeSeasonal` (bool)

**Job type — terms**
- `SearchJobTypeDay`, `SearchJobTypeEarly`, `SearchJobTypeEvening`, `SearchJobTypeFlexible`,
  `SearchJobTypeMorning`, `SearchJobTypeNight`, `SearchJobTypeOnCall`, `SearchJobTypeOvertime`,
  `SearchJobTypeShift`, `SearchJobTypeTbd`, `SearchJobTypeWeekend` (bool)

**Workplace type**
- `SearchJobTypeOnSite`, `SearchJobTypeHybrid`, `SearchJobTypeTravelling`, `SearchJobTypeVirtual` (bool)
  *(map to WorkplaceType ids 0 / 100000 / 100001 / 15141)*

**Education**
- `SearchJobEducationLevel` (string[]) — e.g. `["University","College or apprenticeship","Secondary school or job-specific training","No education"]`

**Salary**
- `SalaryType` (int: `0` hourly | `1` weekly | `2` bi-weekly | `3` monthly | `4` annually)
- `SalaryBracket1`…`SalaryBracket6` (bool) — brackets 1–5 fixed, 6 = custom
- `SearchSalaryUnknown` (bool), `SalaryMin` (string), `SalaryMax` (string)
- `SearchSalaryConditions` (string[]) — benefits, e.g. `"Dental benefits"`, `"Pension plan benefits"`, `"RRSP benefits"`, …

**Location**
- `SearchLocationDistance` (int: `-1` exact | `0` none | `10|15|25|50|75|100` km radius)
- `SearchLocations` (`LocationField[]`)

**Industry**
- `SearchIndustry` (int[]) — NAICS ids

**Equity groups (the "More" filters)**
- `SearchIsApprentice`, `SearchIsVeterans`, `SearchIsIndigenous`, `SearchIsMatureWorkers`,
  `SearchIsNewcomers`, `SearchIsPeopleWithDisabilities`, `SearchIsStudents`,
  `SearchIsVisibleMinority`, `SearchIsYouth` (bool)

**Posting language**
- `SearchIsPostingsInEnglish` (bool, default **true**), `SearchIsPostingsInEnglishAndFrench` (bool)

**NOC / source**
- `SearchNocField` (string), `NocCode` (string)
- `SearchJobSource` (string — see enum), `SearchExcludePlacementAgencyJobs` (bool),
  `SearchNjbJobsFirst` (bool — federal-first)

### Sub-objects
**`DateField`** — `{ Year, Month, Day, Hour, Minute, Second, Millisecond }` (ints).
Serializes to `YYYY-MM-DDThh:mm:ss.fff`.

**`LocationField`** — `{ City, Region, Postal }`.
`Postal` is normalized: **uppercased, spaces removed**.

### Enums (part of the contract)
- **`SortOrder`:** 1 = DatePosted↓, 2 = DatePosted↑, 3 = Title↑, 4 = Title↓, 5 = City↑, 6 = City↓,
  7 = Employer↑, 8 = Employer↓, 9 = Salary↑, 10 = Salary↓, 11 = Relevance.
- **`SearchJobSource`:** `0`/empty = any, `1` = NJB/Federal, `2` = External, `3` = Federal gov,
  `4` = Municipal gov, `5` = BC provincial gov.

### Versioning — `JobSearchFiltersVersion`
- Stored per alert in `JobAlerts.JobSearchFiltersVersion`. **DB default `0`; the old C# model used
  `1`.** Existing rows may be **0 or 1** — both are the **same shape** today.
- **Rule:** the deserializer **must accept 0 and 1** as the current shape. Only **bump the version**
  if the filter shape changes, and **keep a deserializer for every prior version** — the alert email
  sender must handle old rows forever.

### Serialization rules
- Preserve the exact **PascalCase** field names above.
- **Strict on input:** unknown fields are rejected (mirrors the current `MissingMemberHandling.Error`
  → HTTP **400**). Do not silently drop unexpected fields.
- Booleans default `false` unless noted; `SearchIsPostingsInEnglish` defaults `true`.

---

## 2. Drupal-facing API

Consumed server-to-server by the Drupal `workbc_jobboard` module. Base URL = Drupal config
`jobboard_api_url_backend`. **Reproduce the exact keys below** — Drupal reads them literally.

### 2.1 Job search
`POST /api/Search/JobSearch` (optional `/{language}`, e.g. `fr`)
- **Request body:** a `JobSearchFilters` JSON object (§1). Drupal typically sends a subset:
  `Page`, `SortOrder`, `PageSize`, `SearchNocField` (career pages) / `SearchLocations`
  (region/city pages) / `SearchIndustry` (industry pages).
- **Response** (`SearchResultsModel`): wrapper is camelCased (`count` / `result` / `pageNumber` /
  `pageSize`, plus a `textHeaders` UI-label object Drupal ignores). Each `result[]` item is the
  **API response projection**, not the raw index doc — it mirrors
  `workbc-jb/.../WorkBC.ElasticSearch.Models/JobAttributes/Source.cs`. (The raw index fields/analyzers
  used to *build the query* live in `docs/opensearch/` — that's a different layer.)
  ```json
  {
    "count": 1234,
    "result": [
      {
        "JobId": "cmnze80pj264pr8t2pr0mfujk",
        "Title": "Senior Analyst, Sales & Trading",
        "EmployerName": "Hiive",
        "DatePosted": "2026-06-09T13:58:07",
        "ExpireDate": "2026-09-07T13:58:07",
        "City": "Vancouver",
        "Province": "British Columbia",
        "Region": ["Mainland / Southwest"],
        "Location": [ { "Lat": "49.28", "Lon": "-123.12" } ],
        "Noc2021": "62100",
        "NocGroup": "Technical sales specialists - wholesale trade (62100)",
        "Salary": 110000.0,
        "SalarySummary": "$110,000 - $190,000 annually",
        "IsFederalJob": false,
        "HoursOfWork": { "Description": ["Full-time"] },
        "WorkplaceType": { "Id": 0, "Description": "On-site only" },
        "Views": 42,
        "IsNew": false,
        "ApplyWebsite": "https://jobs.ashbyhq.com/…",
        "ExternalSource": { "Source": [ { "Url": "https://…", "Source": "jobs.ashbyhq.com" } ] }
      }
    ],
    "pageNumber": 1,
    "pageSize": 3
  }
  ```
  **Response-layer rules (differ from the raw index — cross-check `docs/opensearch/README.md`):**
  - **`NullValueHandling.Ignore`** — empty/null fields are **omitted** from each item, so federal and
    external items carry *different key sets* (federal → `WorkLangCd`/`WageClass`/`SkillCategories`/
    `Apply*`; external → `JobDescription` is fetched separately, `ExternalSource`/`ApplyWebsite`).
  - **`City` is a CSV *string*** (`ListToCsvConverter` joins the index's `City` **array**) — not an array.
  - **`Region` and `Location` stay arrays**; `Location[]` = `{ Lat, Lon }` (strings).
  - **`Noc2021` is a zero-padded 5-char *string*** (e.g. `"00010"`), not the index's float; the
    response has **no `Noc`** (2016) field.
  - `*.Description` objects (`WorkLangCd`, `HoursOfWork`, `PeriodOfEmployment`, `EmploymentTerms`,
    `SalaryConditions`) = `{ "Description": string[] }`; `WorkplaceType` = `{ Id, Description }`.
  - **Decoration fields not in the index:** `Views` (from DB `JobViews`), `IsNew`; `Score`/`Reason`
    only for Recommended Jobs.
  - Result items are a **subset** of index fields — filter-only fields (`EduLevel`, `Industry`,
    `Occupation`, `NocJobTitle`, `NaicsId`, `LocationGeo`, `AllSkills`) are **queried but not
    returned** here.
  > **Casing is mixed and must be preserved:** top-level `count`/`result`/`pageNumber`/`pageSize`
  > (camelCase); **job-item keys PascalCase** (`JobId`, `Title`, `City`, `ExternalSource.Source[].Url`).
- **Behavior contract:** unknown request fields → **400**; **profile-sidebar heuristic** — NOC
  filter + `PageSize ≤ 10` + no source pinned ⇒ **federal-first** (NJB results first, external
  fallback). Drupal's "Recent Jobs" widgets depend on both.

### 2.2 Total jobs
`GET /api/Search/GetTotalJobs` → **`42802`** — a **bare JSON integer**, not an object.

> **Corrected 2026-08-05.** This section previously read `{ "count": 37831 }`. That was wrong.
> WorkBC.Web's action is `Task<int> GetTotalJobs(...)`, which serialises as a naked number;
> verified with `curl https://workbc-jb.a55eb5-dev.stratus.cloud.gov.bc.ca/api/Search/GetTotalJobs`
> → `42802`. Callers coerce the body straight to a number, so the object form rendered
> **"Search NaN jobs in B.C."** on the WorkBC home page. `TotalJobsApiTest` now pins the bare
> integer.

Language selection accepts **both** forms, as `.NET` model binding did:
`GetTotalJobs/fr` (path segment) and `GetTotalJobs?language=fr` (query string).

### 2.3 City autocomplete
`GET /api/Location/cities/{cityName}/true` → array of strings: `["Surrey", "Surrey, BC", …]`.

### 2.3.1 Path casing — more than one casing is live
ASP.NET Core matches routes **case-insensitively**; Laravel matches **case-sensitively**. Callers
therefore standardised on whatever casing they happened to write, and the app answered. Registering
a single casing in Laravel 404s every other one.

| Endpoint | Casing in real callers (`src/scripts/test/cases.txt`) | Casing this doc used to publish |
|---|---|---|
| Total jobs | `/api/Search/GetTotalJobs` | `/api/Search/gettotaljobs` |
| City autocomplete | `/api/Location/cities/…` | `/api/location/cities/…` |
| Job search | `/api/Search/JobSearch` | *(same — already matched)* |

`routes/api.php` registers **both** casings for each. When adding an endpoint, check `cases.txt`
for the casing real callers use rather than assuming this doc's. The Drupal module itself has not
been read end to end (separate repo — see `integration/drupal-embed.md`), so treat any documented
casing as potentially having a caller.

### 2.4 Career / Industry profile — save & status (authenticated)

> **RETIRED as a server-to-server contract** (2026-07-29, [ADR-009](adr/ADR-009-same-site-session-auth-for-embed.md)).
> The previous text here claimed *"Drupal forwards the job seeker's `Authorization` header."* That was
> **never how this worked.** Inspection of the live Drupal module showed the save/status calls are made
> **from the browser** by `js/workbc_jobboard.js` (URLs pushed to `drupalSettings` by
> `WorkbcJobboardSaveProfile.php`), authenticated with a JWT the inline-embedded Angular app stores
> client-side. The `Authorization`-forwarding branches in Drupal's `WorkBcJobboardController.php` exist
> but are **dead code** — nothing calls them, and they are the likely source of the wrong claim.
>
> These are therefore **not** part of the Drupal-facing server-to-server API. Under ADR-009 they become
> ordinary **session-authenticated** routes (same-site cookie, **401** when anonymous), called from the
> browser by Drupal's JS with `withCredentials`. Owned by **ACCT-6**.
>
> - `POST /career-profiles/save/{profileId}` · `GET /career-profiles/status/{profileId}` → `{ saved, csrf }`
> - `POST /industry-profiles/save/{profileId}` · `GET /industry-profiles/status/{profileId}`
> - **Auth:** Laravel session (ADR-003). No bearer token. CSRF token is returned in the *status*
>   response body, because the `XSRF-TOKEN` cookie is unreadable cross-origin — see ADR-009.
>
> Only §2.1–2.3 remain genuine server-to-server contracts. See
> `integration/drupal-embed.md §6` and `integration/api-status.md`.

### 2.5 Dead endpoint
`GET /api/career-profiles/topjobs/{noc}` exists but is **not called** by Drupal (it uses §2.1).
Removable in the rewrite.

### 2.6 Build info — the availability probe (**load-bearing**)
`GET /api/SystemSettings/BuildInfo` → `{ "SHA": "...", "RunNumber": "...", "BuildDate": "..." }`

> **Added 2026-08-05.** This endpoint was missed in the original contract survey, and §2 previously
> stated there were only three server-to-server calls. **There are four.**

This is **not** a diagnostics endpoint — it gates the entire Drupal job-board region.
`workbc_jobboard.module` calls it before rendering **both** the Find Jobs and the Account page:

```php
function jbTestConnection() {
  try {
    $client = new Client();
    $response = $client->get(config('jobboard_api_url_backend') . '/api/SystemSettings/BuildInfo');
    return !empty($response);
  } catch (...) { return FALSE; }
}
// template: {% if jobboard_connection %} "The Job Board is currently unavailable." {% else %} …app… {% endif %}
```

Guzzle throws on a 404, so **a missing route takes both Drupal pages down** — observed on dev2 the
moment the Laravel image replaced WorkBC.Web. Drupal only checks the response is non-empty, so the
body barely matters, but the keys mirror WorkBC.Web's anonymous object.

Values come from `config/build.php`, populated by the Dockerfile's `COMMIT_SHA` / `RUN_NUMBER` /
`BUILD_DATE` build args. They are `"unknown"` in local builds — deliberately still a 200, because a
500 here would blank the Drupal pages.

**Do not delete this route** because it looks unused from inside this codebase. `BuildInfoApiTest`
exists to make that failure loud.

---

## 3. Contract change rules
- **Additive only.** New optional fields are fine. Removing/renaming/retyping a field, or changing
  the federal-first heuristic or the response key casing, is **breaking** → new API version + ADR.
- Validate every request with a Form Request; **reject unknown fields** (fail closed).
- Return API **Resources**, never raw Eloquent models.
- The `JobSearchFilters` object and these endpoints move together — a change to the filter shape
  that affects `/api/Search/JobSearch` is an API change (§3) **and** a version bump (§1 versioning).
