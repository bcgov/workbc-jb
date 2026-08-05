# Job Board — Domain Glossary

> **Purpose:** Canonical definitions of Job Board domain terms. When a Jira story or code uses
> a term below, it means exactly this. Do not invent alternative meanings. Where a term is
> ambiguous in everyday speech (e.g. "active"), use the disambiguated term. Physical schema
> retains legacy PascalCase identifiers (`IsActive`, `ExpireDate`); Eloquent models map to them.

## Core Entities

| Term | Definition |
|---|---|
| **Job** | A live, normalized posting in `Jobs`. System of record for what a job *is*. Written **only** by ingestion. |
| **ImportedJob (staging)** | A raw feed payload in `ImportedJobsFederal` / `ImportedJobsWanted`, keyed by `JobId`. Input to normalization; not the public job. |
| **JobVersion** | Immutable historical snapshot of a Job's key fields. Appended on change. Used by admin history/reporting, never search. |
| **JobSeeker** | An end-user account (`AspNetUsers`; FK `AspNetUserId`). "User" and "JobSeeker" are the same entity. |
| **SavedJob** | A bookmarked job with optional 800-char note. Soft-deleted via `IsDeleted`+`DateDeleted`. |
| **JobAlert** | A saved search (serialized `JobSearchFilters` JSON) emailed on a cadence. Soft-deleted like SavedJob. |
| **ExpiredJob** | Row marking a JobId removed from a feed, pending deletion from OpenSearch (`RemovedFromElasticsearch`). |
| **DeletedJob** | A JobId on a permanent deny-list; never re-imported. |

## Job Status — critical disambiguation

"Active" is ambiguous. **Always use the precise term.** These are independent and can disagree:

| Term | Exact meaning | Governs |
|---|---|---|
| **IsActive** (bool) | The feed still lists this job. Set `false` when the source expires it. | **Presence in OpenSearch** — `IsActive=true` → indexed; `false` → purged. |
| **ExpireDate** (timestamp) | Date the posting stops appearing in search. Computed by ingestion. | **Search visibility** — search filters `ExpireDate >= now`. |
| **date-active** | `IsActive = true AND ExpireDate > now`. | The set that *should* be search-visible. |
| **search-visible** | An OpenSearch doc whose `ExpireDate >= now`. | What the front end returns. |

> **Invariant:** a Job can be `IsActive=true` but past `ExpireDate` (hidden from search) — that
> is normal. `search-visible` must equal `date-active`; when it doesn't, the document's
> `ExpireDate` has drifted from `Jobs.ExpireDate` — a defect (Rule B).

## Date Fields — all six

| Field | Meaning |
|---|---|
| **DatePosted** | Effective post date used for sort/display; may track the source's refresh date. |
| **ActualDatePosted** | Original source post date; does not move on refresh. |
| **ExpireDate** | When the job leaves search (derived; see below). |
| **DateFirstImported** | First time our system ever saw this JobId. |
| **DateLastImported** | Last time ingestion wrote/updated this job — i.e. when its source `updatedAt` last changed. Stale = source unchanged recently (may still be live). |
| **DateLastSeen** | Last time the job appeared in the feed at all (Innovibe staging). |
| **LastUpdated** | Last time any write touched the `Jobs` row (bookkeeping). |

> `DateLastImported` ≠ `LastUpdated`. First tracks source changes; second tracks any DB write.

## Job Sources & Feeds

| Term | Meaning |
|---|---|
| **JobSourceId = 1 / Federal / Job Bank / NJB** | Government of Canada Job Bank XML (Solr) feed. NJB = National Job Bank. |
| **JobSourceId = 2 / External / Wanted** | Third-party aggregator; **currently Innovibe** (JSON). |
| **Innovibe** | The current external feed provider (JSON). |
| **"Wanted"** | Legacy label for the external pipeline (`ImportedJobsWanted`, Wanted indexer). Historically **TalentNeuron (TN)**, now Innovibe. Treat "Wanted" = "the external / JobSourceId-2 pipeline," not a live provider. |
| **IsFederalOrWorkBc** (bool on `ImportedJobsWanted`) | Marks a staging row as federal/WorkBC so the Wanted indexer skips it. |

## Derived Fields (computed once, in ingestion — Rule B)

| Field | Definition | Never recompute in |
|---|---|---|
| **ExpireDate** | Computed by the importer from source dates, extended on refresh. Authoritative in `Jobs.ExpireDate`. | indexer, API, search |
| **Salary** | Annualized numeric salary; used for range filters/sort. | indexer, API |
| **SalarySummary** | Human-readable salary string for display. Distinct from `Salary`. | — |
| **Minimum-wage eligibility** | A **data-quality filter** on federal salaries (`shared.settings.minimumWage`) — drops implausible feed salaries. **Not** employer-pay enforcement. | indexer |
| **Resolved NOC / Location** | Source codes mapped to validated `NocCodes2021` / `Locations` ids. | indexer, API |

## Flags & Reference Data

| Term | Meaning |
|---|---|
| **ReIndexNeeded** | Staging flag: (re)project this job into OpenSearch. Set on write, cleared after index. |
| **NOC 2016 vs NOC 2021** | Two active occupation-classification versions. Federal carries both; Innovibe 2021 only. **Search uses `Noc2021`.** |
| **WorkplaceType** | By id: On-site `0`, Hybrid `100000`, Travelling `100001`, **Virtual `15141`** (force-included in location searches). |
| **SystemSetting** | Key-value config in `SystemSettings` (e.g. `shared.settings.minimumWage`). SuperAdmin-editable; cached with invalidation. |
| **JobSearchFilters** | The serialized search-criteria object shared by the search form, saved alerts, and the alert email sender. See `docs/contracts.md`. |

## Reporting Terms

| Term | Meaning |
|---|---|
| **WeeklyPeriod** | A reporting week bucket with fiscal/calendar attributes. |
| **JobStat** | Weekly aggregate of postings/vacancies by region and source (composite key). |
| **JobSeekerStat** | Weekly job-seeker demographic aggregate (composite key). |

## Legacy / Deprecated (you will see these in old .NET code)

| Term | Status |
|---|---|
| **"Wanted" as a provider** | Dead — TalentNeuron/now-Innovibe; the code name persists. |
| **"FakeKentico"** | Dead — old CMS wrapper. Parent CMS is now **Drupal**. Do not port. |
| **NEST** | Old .NET ES client. Rewrite uses the OpenSearch client with array query bodies. |
| **`Md5PasswordHasher`** | Do not port. Legacy accounts are force-reset (ADR-003). |
| **`topjobs/{noc}` endpoint** | Dead — Drupal calls the general search endpoint, not this. |
