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
`GET /api/Search/gettotaljobs` → `{ "count": 37831 }` (active-job count).

### 2.3 City autocomplete
`GET /api/location/cities/{cityName}/true` → array of strings: `["Surrey", "Surrey, BC", …]`.

### 2.4 Career / Industry profile — save & status (authenticated)
- `POST /api/career-profiles/save/{profileId}` — save a career (NOC) profile for the user.
- `GET  /api/career-profiles/status/{profileId}` → boolean.
- `POST /api/industry-profiles/save/{profileId}` · `GET /api/industry-profiles/status/{profileId}`.
- **Auth:** Drupal forwards the job seeker's `Authorization` header.

### 2.5 Dead endpoint
`GET /api/career-profiles/topjobs/{noc}` exists but is **not called** by Drupal (it uses §2.1).
Removable in the rewrite.

---

## 3. Contract change rules
- **Additive only.** New optional fields are fine. Removing/renaming/retyping a field, or changing
  the federal-first heuristic or the response key casing, is **breaking** → new API version + ADR.
- Validate every request with a Form Request; **reject unknown fields** (fail closed).
- Return API **Resources**, never raw Eloquent models.
- The `JobSearchFilters` object and these endpoints move together — a change to the filter shape
  that affects `/api/Search/JobSearch` is an API change (§3) **and** a version bump (§1 versioning).
