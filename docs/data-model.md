# Job Board — Data Model

> **Purpose:** The authoritative map of the existing PostgreSQL schema to Eloquent, and the
> rules for working with it. Source of truth is the live database (see `schema.csv`). Terms:
> `docs/glossary.md`.

## 0. HARD CONSTRAINT — existing database, existing data

**The application runs on the existing production database. We do NOT create a new schema or a
new database, and we do NOT alter existing tables to suit Laravel conventions.**

- **Map, don't create.** Eloquent models bind to existing tables (PascalCase names, PascalCase
  columns). There are **no `create` migrations for existing tables.**
- **New Laravel migrations only for genuinely new tables** (e.g. `import_logs`, and Laravel's own
  framework tables listed in §5). Never `create`/`drop`/`rename` an existing table or column.
- **EF migrations are frozen.** The DB carries `__EFMigrationsHistory` (EF) — Laravel uses its
  own separate `migrations` table; they don't collide. During any transition where both stacks
  touch the DB, schema changes are coordinated and applied once.
- **The .NET apps may still read this DB during transition** — do not remove columns they use
  (e.g. `DataProtectionKeys`, `NormalizedEmail/UserName`).

## 1. Eloquent conventions (apply to every model)

The schema predates Laravel, so the framework defaults are wrong. Base pattern:

```php
class Job extends Model
{
    protected $table = 'Jobs';        // PascalCase — pgsql grammar quotes identifiers automatically
    protected $primaryKey = 'JobId';
    public $incrementing = false;     // string / externally-assigned PK
    protected $keyType = 'string';
    public $timestamps = false;       // NO created_at / updated_at columns exist

    protected $casts = [
        'IsActive'   => 'boolean',
        'ExpireDate' => 'datetime',
        'Salary'     => 'decimal:2',
        // enums: 'AccountStatus' => AccountStatus::class, etc.
    ];
}
```

- **Keep column names PascalCase in code** (`$job->ExpireDate`). Do **not** alias to snake_case —
  a renaming layer over a shared DB is a standing source of bugs.
- **`$timestamps = false` everywhere** — no table has `created_at`/`updated_at`.
- **String PKs** (`AspNetUsers.Id`, `Jobs.JobId`, `JobIds.Id`, `SystemSettings.Name`,
  `JobSeekerStatLabels.Key`, `NocCodes.Id`?): set `$incrementing=false`, `$keyType='string'`.
- **`timestamp without time zone`** columns + `app.timezone = America/Vancouver` → cast as
  `datetime`; the app TZ is load-bearing (Rule F).

## 2. Core domain tables

### Jobs & ingestion
| Table | PK | Eloquent notes |
|---|---|---|
| `Jobs` | `JobId` (string) | Central job. Casts: 7 type bools (`FullTime`,`PartTime`,`LeadingToFullTime`,`Permanent`,`Temporary`,`Casual`,`Seasonal`), `IsActive` → bool; `Salary` decimal:2; all `Date*`/`ExpireDate`/`LastUpdated` datetime. `NocCodeId` (smallint, 2016) **and** `NocCodeId2021` (int) both live. `belongsTo` Location/NocCode/NocCode2021/Industry/JobSource. |
| `JobVersions` | `Id` (bigint) | Historical snapshots; `IsCurrentVersion`, `VersionNumber`. |
| `JobViews` | `JobId` | Per-job view counter (`Views`, `DateLastViewed`). |
| `JobIds` | `Id` (string) | Job-id registry (`DateFirstImported`, `JobSourceId`). |
| `JobSources` | `Id` (smallint) | Lookup: 1=Federal, 2=External/Wanted. |
| `ImportedJobsFederal` | `JobId` | **Staging** (raw XML in `JobPostEnglish/French`, `DisplayUntil`, `ApiDate`, `ReIndexNeeded`). Ingestion-owned; no second write path. |
| `ImportedJobsWanted` | `JobId` | **Staging** (raw JSON, `IsFederalOrWorkBc`, `HashId` bigint, `DateLastSeen`, `ReIndexNeeded`). |
| `ExpiredJobs` | `JobId` | `RemovedFromElasticsearch`, `DateRemoved`. |
| `DeletedJobs` | `JobId` | Permanent deny-list (`DeletedByAdminUserId`, `DateDeleted`). |

### Job seeker (Identity + profile)
| Table | PK | Eloquent notes |
|---|---|---|
| `AspNetUsers` | `Id` (string GUID) | The **JobSeeker**. Identity columns + profile (`FirstName`,`LastName`,`City`,`LocationId`,`CountryId`,`ProvinceId`,`LegacyWebUserId`), `AccountStatus` (smallint enum; `99`=deleted), `DateRegistered`/`LastLogon`/`LastModified` datetime, `VerificationGuid` (uuid), `SecurityAnswer`/`SecurityQuestionId`, `DateLocked`/`LockedByAdminUserId`, `PasswordHash`. `$incrementing=false; $keyType='string'`. Implement `Authenticatable` with `getAuthPassword()→PasswordHash`. |
| `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims` | Identity | Identity infrastructure. Mostly untouched; auth uses sessions (ADR-003). |
| `JobSeekerFlags` | `Id` | **Equity flags live here**, one row per user: `IsApprentice`,`IsIndigenousPerson`,`IsMatureWorker`,`IsNewImmigrant`,`IsPersonWithDisability`,`IsStudent`,`IsVeteran`,`IsVisibleMinority`,`IsYouth`. `hasOne` from JobSeeker. |
| `JobSeekerVersions` | `Id` (bigint) | Versioned profile snapshots (`IsCurrentVersion`, `VersionNumber`). |
| `JobSeekerChangeLog` | `Id` | Profile change audit (`Field`,`OldValue`,`NewValue`,`ModifiedByAdminUserId`). *(Note: table is `JobSeekerChangeLog`, not "ChangeEvents".)* |
| `JobSeekerEventLog` | `Id` | Activity audit (`EventTypeId` enum, `DateLogged`). |
| `JobSeekerAdminComments` | `Id` | Internal admin notes (`Comment`, `IsPinned`). |
| `SecurityQuestions` | `Id` | Lookup for password reset (see ADR-003 open item). |

### User actions
| Table | PK | Eloquent notes |
|---|---|---|
| `SavedJobs` | `Id` (int) | `JobId`, `AspNetUserId`, `DateSaved`, 800-char `Note`, `NoteUpdatedDate`. **Soft-delete pair** `IsDeleted`+`DateDeleted` (custom trait, not `SoftDeletes`). |
| `JobAlerts` | `Id` (int) | `Title`, `AlertFrequency` (smallint enum), `UrlParameters`, **`JobSearchFilters` (text JSON)**, `JobSearchFiltersVersion` (int, **DB default `0`**), `DateCreated/Modified/Deleted`, `IsDeleted`. Cast `JobSearchFilters` → the shared value object (`docs/contracts.md`). |
| `SavedCareerProfiles` | `Id` | `AspNetUserId`, `NocCodeId2021`, `EDM_CareerProfile_CareerProfileId`, soft-delete pair. |
| `SavedIndustryProfiles` | `Id` | `AspNetUserId`, `IndustryId`, soft-delete pair. |

### Reference data (read-only; seeded/maintained)
`Countries`, `Provinces`, `Regions`, `Locations` (PK `LocationId`; `IsHidden`,`IsDuplicate`,`FederalCityId`,`Label`,`Lat/Lng`), `Industries`, `NocCodes` (2016, PK `Id` smallint), `NocCodes2021` (PK `Id` int, `Code2016`), `NocCategories`(2021), `GeocodedLocationCache` (unique `Name`).

### Reporting (mostly written by stored procedures — see §4)
| Table | PK | Notes |
|---|---|---|
| `WeeklyPeriods` | `Id` | Normal Eloquent model (report dropdowns read it). |
| `JobStats` | **composite** (`WeeklyPeriodId`,`JobSourceId`,`RegionId`) | **No standard Eloquent** — query-builder only. |
| `JobSeekerStats` | **composite** (`WeeklyPeriodId`,`RegionId`,`LabelKey`) | Same. |
| `JobSeekerStatLabels` | `Key` (string) | Simple read-only lookup. |
| `ReportPersistenceControl` | **composite** (`WeeklyPeriodId`,`TableName`) | Idempotency guard for stats generation. |

### Admin / system
| Table | PK | Notes |
|---|---|---|
| `AdminUsers` | `Id` (int) | `AdminLevel` (enum), self-FK `LockedByAdminUserId`. Login via Keycloak (ADR-003); table is for roles/management. |
| `SystemSettings` | `Name` (string) | `Value`(text), `FieldType` (int enum), `DefaultValue`, `ModifiedByAdminUserId`, `DateUpdated`. Cache with invalidation. |
| `ImpersonationLog` | `Token` (string) | `AspNetUserId`, `AdminUserId`, `DateTokenCreated`. *(Table is `ImpersonationLog`, not "Impersonations".)* |

## 3. Composite-key tables — do not model as standard Eloquent

`JobStats`, `JobSeekerStats`, `ReportPersistenceControl` have composite primary keys. Eloquent
has no composite-PK support. Access them via the **query builder** (they're written by stored
procedures and read by report queries). Do not add a standard model with a single `$primaryKey`.

## 4. Database-resident logic stays in the DB

- **Stored procedures** `usp_GenerateJobStats` / `usp_GenerateJobSeekerStats` populate the stats
  tables + `ReportPersistenceControl`. The Reporting service **calls** them (`DB::select`/`CALL`);
  it does not reimplement them.
- **Table-valued functions** `tvf_GetJobsForDate` / `tvf_GetJobSeekersForDate` back point-in-time
  reports — call from PDO as-is.
- **`cron` schema (pg_cron)** exists (`cron.job`, `cron.job_run_details`). Scheduling of the
  stats procs may currently run via **pg_cron inside PostgreSQL**. Decide (ADR) whether scheduled
  work is driven by **pg_cron** (DB-resident procs) or the **Laravel scheduler** — do not run both
  for the same job. Recommendation: Laravel scheduler for app jobs (import/index/alerts); leave
  pg_cron only if it's currently invoking the stats procs and you want that to stay in-DB.

## 5. Framework tables & genuinely new tables

Since cache/queue/session use **Redis**, most Laravel framework tables are unnecessary. What we
add (new migrations, new tables only):
- `migrations` — Laravel's own migration ledger (separate from `__EFMigrationsHistory`).
- `import_logs` — job start/finish/failure logging (per coding standards).
- `password_reset_tokens` — only if using Laravel's password-reset (see ADR-003 decision).
- `failed_jobs` — only if not using Horizon's Redis-based failure tracking.
- **Queue:** set the queue connection to **redis** so Laravel does **not** create a `jobs` table.
  (A lowercase `jobs` table would be distinct from the existing `"Jobs"` in Postgres, but avoid
  the confusion entirely by using Redis.)

## 6. Gotchas / notes

- **`ImpersonationLog`, `JobSeekerChangeLog`, `JobSeekerEventLog`** are the real table names
  (earlier drafts said Impersonations / ChangeEvents / Events — corrected here).
- **Equity flags are in `JobSeekerFlags`**, not on `AspNetUsers`.
- **`JobAlerts.JobSearchFiltersVersion` DB default is `0`** (C# model used `1`). Existing rows may
  be `0` or `1`; the deserializer must handle both (`docs/contracts.md`).
- **`DataProtectionKeys`** is ASP.NET Data Protection — the .NET apps use it; Laravel does not.
  Leave it during transition.
- **`__EFMigrationsHistory`** — EF's ledger; do not touch. Laravel migrations are independent.
- **Two NOC versions** (`NocCodeId` 2016 smallint + `NocCodeId2021` int) are both live; search
  uses 2021. Model both.
- **`AccountStatus = 99`** means deleted (report SQL filters `<> 99`).
