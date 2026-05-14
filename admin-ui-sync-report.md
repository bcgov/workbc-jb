# Admin / UI Job-Count Sync — Issue Report

**Date:** 2026-05-13
**Branch:** `relevance-logic`
**Scope:** Innovibe (Wanted) jobs only — `JobSourceId = 2`. Federal jobs were unaffected.

---

## TL;DR

- **Symptom:** Admin "Manage Job Postings" reported **~28,665** active External jobs while the public Search UI reported **~22,415**.
- **Root cause:** Three independent drifts compounded:
  1. **Expiry-window mismatch** — PHP importer hardcoded `Jobs.ExpireDate = updatedAt + 90 days`; the C# indexer wrote ES with `updatedAt + WantedJobExpiryDays` (= **30** in deployed config). `Jobs.ExpireDate` overshot ES.ExpireDate by 60 days, so jobs whose `updatedAt` was 31–90 days old were "active" in admin but expired in ES.
  2. **Data-quality holes** — admin counted any `IsActive=TRUE AND ExpireDate>NOW()` row, even when the indexer/UI would silently drop it (`LocationId=0`, `NocCodeId2021 IS NULL`, empty `City`/`Title`/`EmployerName`, etc.).
  3. **PurgeJobs limitation** — the indexer's purge step only removed ES docs whose `ImportedJobsWanted` row was gone. Rows that expired by date (or were deactivated without the staging row being deleted) lingered in ES forever.
- **Fix:**
  - PHP and C# now both compute `ExpireDate` as `COALESCE(dateValidThrough, updatedAt + N days)` where `N` is shared config.
  - Admin's `JobService.cs` now applies the same predicate ES uses (`VisibleWantedJobs()`).
  - `PurgeJobs()` got a third pass that removes ES docs whose `Jobs` row is inactive or expired.
  - One-time SQL cleanup deactivated 5,476 stale rows and backfilled 25,575 `ExpireDate`s on the dev DB.
- **Result:** Admin count dropped from **27,085 → 23,185** in dev, ≈ ES/UI count of **22,306**.

---

## Background

The pipeline:

```
Innovibe API ──► PHP importer ──► Postgres
                                  ├── "ImportedJobsWanted" (raw JSON staging)
                                  ├── "Jobs"               (canonical row used by admin)
                                  └── "ExpiredJobs"        (expiry signal for PurgeJobs)
                                              │
                                              ▼
                                    C# Wanted indexer ──► Elasticsearch (jobs_en) ──► Public UI
```

Two writers produce an `ExpireDate`:

| Writer | Reads | Writes |
|---|---|---|
| `src/WorkBC.Importers.Innovibe/src/Service/JobImportService.php:493` | Innovibe JSON | `Jobs.ExpireDate` (used by **admin**) |
| `src/WorkBC.ElasticSearch.Indexing/Services/XmlParsingServiceWanted.cs:143` | Same JSON | ES `ExpireDate` (used by **public UI**) |

If these disagree, admin and UI counts diverge.

---

## Root-cause analysis (verified against prod)

### 1. The 90-vs-30 day mismatch

Old PHP:
```php
$datePosted = date('Y-m-d H:i:s', strtotime($j['updatedAt'] ?? $j['postedDate'] ?? $j['createdAt'] ?? 'now'));
$expireDate = date('Y-m-d H:i:s', strtotime($datePosted . ' +90 days'));   // hardcoded
```

Old C#:
```csharp
ExpireDate = refreshedDate.AddDays(WantedJobExpiryDays),   // config-driven; prod = 30
```

Sample from the live API:
```
DatePosted: 2026-05-13T17:36:16
ExpireDate: 2026-06-12T17:36:16     ← exactly +30 days
```

So `Jobs.ExpireDate = updatedAt + 90d` but `ES.ExpireDate = updatedAt + 30d`. For any job last updated 31–90 days ago, admin counted it active while ES already considered it expired. The diagnostic showed this was the single largest contributor — **5,476 rows** on the dev DB had `Jobs.ExpireDate` past the equivalent ES expiry.

Innovibe also publishes `dateValidThrough` on many postings — the authoritative expiry — and **neither writer was using it**.

### 2. Admin SQL didn't match ES filters

`JobService.FilterJobs()` was:
```csharp
job => job.IsActive && job.ExpireDate > DateTime.Now && job.JobSourceId == JobSource.Wanted
```

But ES + UI silently exclude jobs with empty `Region` (out-of-BC), `LocationId=0` (city-name fallback failed), `NocCodeId2021 = NULL` (NOC tagger missed), or those present in `ExpiredJobs` / `DeletedJobs`. Admin counted these; UI hid them.

### 3. `PurgeJobs()` only checked staging-row presence

The third pass in `PurgeJobs()`:
```csharp
List<string> moreJobsToPurge = elasticJobsId.Except(sqlJobIds).ToList();
// sqlJobIds == ImportedJobsWanted.JobId list
```

So a job whose `Jobs.IsActive` flipped to `FALSE`, or whose `Jobs.ExpireDate` passed, but whose `ImportedJobsWanted` row still existed, was **left in ES indefinitely**. Verified case during investigation: `cmot12lz7889ftpryvh4nj0cv`:

```text
in_jobs=t  in_deleted=f  in_expired=f  in_staging=t
IsActive=t  ExpireDate=2026-05-13 09:59:00  expiry_ok=f
```

Was still appearing in ES even though it had expired-by-date.

---

## Code changes

### A. `src/WorkBC.Importers.Innovibe/src/Config/AppConfig.php`

Added a `jobExpiryDays` setting sourced from env (`JOB_EXPIRY_DAYS`, default 30). Must mirror `WantedSettings:JobExpiryDays` on the C# side.

### B. `src/WorkBC.Importers.Innovibe/src/Service/JobImportService.php` (around line 493)

```php
$validThrough = !empty($j['dateValidThrough']) ? strtotime((string) $j['dateValidThrough']) : false;
$expireDate   = $validThrough !== false
    ? date('Y-m-d H:i:s', $validThrough)
    : date('Y-m-d H:i:s', strtotime($datePosted . ' +' . $this->cfg->jobExpiryDays . ' days'));
```

### C. `src/WorkBC.ElasticSearch.Indexing/Services/XmlParsingServiceWanted.cs` (around line 60–143, JSON path)

```csharp
string dateValidThroughStr = j.Value<string>("dateValidThrough");
DateTime esExpireDate;
if (!string.IsNullOrWhiteSpace(dateValidThroughStr)
    && DateTime.TryParse(dateValidThroughStr, out var dvt))
{
    esExpireDate = dvt;
}
else
{
    esExpireDate = refreshedDate.AddDays(WantedJobExpiryDays);
}
...
ExpireDate = esExpireDate,
```

The legacy XML branch (TalentNeuron, ~line 534) was intentionally **not** touched — TalentNeuron feed has no `dateValidThrough` and is being phased out.

### D. `src/WorkBC.Admin/Areas/Jobs/Services/JobService.cs`

New helper `VisibleWantedJobs()` mirrors the ES visibility predicates. Used by:
- `case "external":` (no-keyword External filter)
- the no-filter catch-all (Federal stays loose; Wanted gets strict)

Keyword search paths (admin looking up a specific JobId/URL) were intentionally left loose so admins can still find broken/expired jobs.

### E. `src/WorkBC.Indexers.Wanted/Services/WantedIndexService.cs` (`PurgeJobs`)

Added a third purge pass that removes ES docs whose `Jobs` row is inactive or expired-by-date, regardless of whether the staging row still exists:

```csharp
var inactiveOrExpiredInSql = (await _dbContext.Jobs
        .Where(j => j.JobSourceId == JobSource.Wanted
                    && (!j.IsActive || j.ExpireDate <= DateTime.Now))
        .Select(j => j.JobId)
        .ToListAsync())
    .ToHashSet();

var elasticOrphansByState = elasticJobsId.Where(inactiveOrExpiredInSql.Contains).ToList();
foreach (string jobId in elasticOrphansByState)
{
    await PostToElasticSearch(string.Empty, jobId, "DELETE");
    Console.Write("[D3]");
}
```

---

## One-time SQL cleanup applied to dev DB

Run in order. Verified on `relevance-logic` against the dev Postgres.

### Step 0 — Establish the strict-admin predicate (re-usable CTE)

```sql
-- Used in many places below
WITH strict_admin AS (
  SELECT j.*
  FROM "Jobs" j
  JOIN "ImportedJobsWanted" w
    ON w."JobId" = j."JobId" AND NOT w."IsFederalOrWorkBc"
  WHERE j."JobSourceId" = 2
    AND j."JobId" !~ '^[0-9]+$'
    AND j."IsActive" = TRUE
    AND j."ExpireDate" > NOW()
    AND j."LocationId" <> 0
    AND j."NocCodeId2021" IS NOT NULL
    AND j."City" <> '' AND j."Title" <> '' AND j."EmployerName" <> ''
    AND NOT EXISTS (SELECT 1 FROM "ExpiredJobs" e WHERE e."JobId" = j."JobId")
    AND NOT EXISTS (SELECT 1 FROM "DeletedJobs" d WHERE d."JobId" = j."JobId")
)
SELECT COUNT(*) AS admin_count FROM strict_admin;
```

### Step 1 — Deactivate expired-but-active rows

Removed 66 rows from dev where `IsActive=TRUE AND ExpireDate <= NOW()`.

```sql
BEGIN;

INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
SELECT "JobId", NOW(), FALSE
FROM "Jobs"
WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW()
ON CONFLICT ("JobId") DO UPDATE
  SET "DateRemoved" = NOW(), "RemovedFromElasticsearch" = FALSE;

DELETE FROM "ImportedJobsWanted"
WHERE "JobId" IN (
  SELECT "JobId" FROM "Jobs"
  WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW()
);

UPDATE "Jobs"
SET "IsActive" = FALSE, "LastUpdated" = NOW()
WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW();

COMMIT;
```

### Step 2 — Snap `Jobs.ExpireDate` to `COALESCE(dateValidThrough, updatedAt + 30d)`

Updated 25,575 rows on dev; deactivated 5,476 newly-revealed expired rows.

```sql
BEGIN;

UPDATE "Jobs" j
SET "ExpireDate" = COALESCE(
      ((w."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz),
      ((w."JobPostEnglish"::jsonb->>'updatedAt')::timestamptz) + INTERVAL '30 days'
    ),
    "LastUpdated" = NOW()
FROM "ImportedJobsWanted" w
WHERE w."JobId" = j."JobId"
  AND NOT w."IsFederalOrWorkBc"
  AND j."JobSourceId" = 2
  AND LEFT(w."JobPostEnglish",1) = '{'
  AND COALESCE(
        ((w."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz),
        ((w."JobPostEnglish"::jsonb->>'updatedAt')::timestamptz) + INTERVAL '30 days'
      ) IS DISTINCT FROM j."ExpireDate";

INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
SELECT "JobId", NOW(), FALSE
FROM "Jobs"
WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW()
ON CONFLICT ("JobId") DO UPDATE
  SET "DateRemoved" = NOW(), "RemovedFromElasticsearch" = FALSE;

DELETE FROM "ImportedJobsWanted"
WHERE "JobId" IN (
  SELECT "JobId" FROM "Jobs"
  WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW()
);

UPDATE "Jobs"
SET "IsActive" = FALSE, "LastUpdated" = NOW()
WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW();

COMMIT;
```

`+30 days` must match `WantedSettings:JobExpiryDays` in deployed config. If that setting changes, update the SQL accordingly.

### Step 3 — Verify

```sql
SELECT COUNT(*) AS admin_strict_after_cleanup
FROM "Jobs" j
WHERE j."JobSourceId" = 2
  AND j."JobId" !~ '^[0-9]+$'
  AND j."IsActive" = TRUE
  AND j."ExpireDate" > NOW()
  AND j."LocationId" <> 0
  AND j."NocCodeId2021" IS NOT NULL
  AND j."City" <> '' AND j."Title" <> '' AND j."EmployerName" <> ''
  AND NOT EXISTS (SELECT 1 FROM "ExpiredJobs" e WHERE e."JobId" = j."JobId")
  AND NOT EXISTS (SELECT 1 FROM "DeletedJobs" d WHERE d."JobId" = j."JobId")
  AND EXISTS (SELECT 1 FROM "ImportedJobsWanted" w
              WHERE w."JobId" = j."JobId" AND NOT w."IsFederalOrWorkBc");
```

| Stage | Count |
|---|---:|
| Before any fix (old admin) | 28,665 |
| After `JobService.cs` strict filter | 27,085 |
| After Step 1 (66 rows) | 27,085 |
| After Step 2 (25,575 updated + 5,476 deactivated) | **23,185** |
| Live ES (via UI `JobSearch` API) | 22,306 |
| Residual gap | ~879 (transient — indexer-lag / Region-empty / etc.) |

---

## Drift-monitoring queries

Save these and rerun whenever someone reports a count mismatch.

### Quick health check

```sql
SELECT
  (SELECT COUNT(*) FROM "Jobs"
    WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" > NOW())   AS admin_loose,
  (SELECT COUNT(*) FROM "Jobs" j
    JOIN "ImportedJobsWanted" w ON w."JobId"=j."JobId" AND NOT w."IsFederalOrWorkBc"
    WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE AND j."ExpireDate" > NOW()
      AND j."LocationId" <> 0 AND j."NocCodeId2021" IS NOT NULL
      AND j."City" <> '' AND j."Title" <> '' AND j."EmployerName" <> ''
      AND NOT EXISTS (SELECT 1 FROM "ExpiredJobs" e WHERE e."JobId"=j."JobId")
      AND NOT EXISTS (SELECT 1 FROM "DeletedJobs" d WHERE d."JobId"=j."JobId")
  )                                                                       AS admin_strict;
```

`admin_loose - admin_strict` should be a small positive number (a few hundred). If it grows large, the strict filter is dropping new buckets — investigate.

### Find rows where `Jobs.ExpireDate` disagrees with the ES rule

```sql
SELECT COUNT(*) AS expire_date_drift
FROM "Jobs" j
JOIN "ImportedJobsWanted" w ON w."JobId" = j."JobId" AND NOT w."IsFederalOrWorkBc"
WHERE j."JobSourceId" = 2
  AND LEFT(w."JobPostEnglish",1) = '{'
  AND j."IsActive" = TRUE
  AND COALESCE(
        ((w."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz),
        ((w."JobPostEnglish"::jsonb->>'updatedAt')::timestamptz) + INTERVAL '30 days'
      ) IS DISTINCT FROM j."ExpireDate";
```

Should be ~0 after deploy. If it grows, either the importer regressed or the staging JSON shape changed.

### Definitive Admin↔ES diff (runs against live ES)

Inside the `cli` pod:

```bash
dotnet WorkBC.Indexers.Wanted.dll --debug > /tmp/diff.txt 2>&1
```

Produces:
- `Active jobs found in Elasticsearch: …` (UI side)
- `Active jobs found in the Jobs table: …` (loose admin side)
- `JobIds found in Elasticsearch but not in the Jobs table:` — ES orphans
- `JobIds found in the Jobs table but not in Elasticsearch:` — admin overcount

---

## Production deploy checklist

1. **Verify env config**
   ```bash
   kubectl exec -n app -c web jb-d56c8978f-vcxzh -- grep -A2 WantedSettings /app/appsettings.json
   ```
   Confirm `"JobExpiryDays": 30` (or whatever is intended). Set `JOB_EXPIRY_DAYS` on the importer pod to the same value.
2. **Apply the code changes** (PHP + C# + admin). Rebuild & deploy importer, indexer, admin.
3. **Run the one-time SQL cleanup** (Steps 1–3 above) against the prod Postgres in a transaction so it can be rolled back.
4. **Run the indexer once** so `PurgeJobs()`'s new third pass removes ES docs for the 5,476 deactivations.
5. **Verify counts** — admin should match `Hits.Total.Value` from `/api/Search/JobSearch?SearchJobSource=2`.

---

## Out of scope (deferred)

- **Duplicate cuids from Innovibe.** Innovibe assigns multiple `id` values to the same underlying `sourceJobId`. Currently 19 visible URL groups on prod have 2–3 cuids each. The right fix is on the vendor side; SQL export of the duplicates is available via the queries in this conversation history.
- **Old TalentNeuron (numeric JobId) rows.** ~1.4M legacy rows in `Jobs` with `JobSourceId=2 AND JobId ~ '^[0-9]+$'`. Most are already `IsActive=FALSE` and never expire. A separate cleanup (`expiry-cleanup.sql.md` Bucket A) addresses these.
- **Out-of-BC postings.** The PHP importer accepts jobs where no `jobLocations[]` entry has `state = "British Columbia"`. The C# indexer assigns `Region=""` to those and the UI region facet hides them, but admin still counts them via the `City <> ''` fallback. Lower-priority.
