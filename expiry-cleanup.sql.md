# Innovibe Expiry Cleanup — Dev DB

Three drift buckets inflating admin count vs UI:

| Bucket | Cause | Dev count |
|---|---|---|
| **A** Legacy integer-id ghosts | XML staging rows resurrected as active by `updateExistingJobs` | 10,930 |
| **B** Jobs/ExpiredJobs desync | `IsActive=TRUE` while row exists in `ExpiredJobs` | 2,450 |
| **C** 90-day overshoot | `ExpireDate` ignores Innovibe's `dateValidThrough` | 410 |

Baseline `admin_count = 38,567` → expected after = `~24,777`.

Run each block below in order. Inspect output, then `COMMIT;` or `ROLLBACK;`.

---

## 0 · Snapshot

```sql
SELECT
  (SELECT COUNT(*) FROM "Jobs"
     WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW())                       AS admin_count,
  (SELECT COUNT(*) FROM "Jobs"
     WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "JobId" ~ '^[0-9]+$')                        AS bucket_A,
  (SELECT COUNT(*) FROM "Jobs" j JOIN "ExpiredJobs" e ON e."JobId"=j."JobId"
     WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE)                                              AS bucket_B,
  (SELECT COUNT(*) FROM "Jobs" j JOIN "ImportedJobsWanted" ij ON ij."JobId"=j."JobId"
     WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE AND j."ExpireDate" >= NOW()
       AND LEFT(ij."JobPostEnglish",1)='{'
       AND (ij."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz < NOW())              AS bucket_C;
```

---

## 1 · Bucket A — legacy integer-id ghosts

```sql
BEGIN;

UPDATE "Jobs"
SET "IsActive"=FALSE, "LastUpdated"=NOW()
WHERE "JobSourceId"=2 AND "JobId" ~ '^[0-9]+$';

INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
SELECT "JobId", NOW(), FALSE FROM "Jobs"
WHERE "JobSourceId"=2 AND "IsActive"=FALSE AND "JobId" ~ '^[0-9]+$'
ON CONFLICT ("JobId") DO NOTHING;

DELETE FROM "ImportedJobsWanted" WHERE "JobId" ~ '^[0-9]+$';

-- both must be 0
SELECT
  (SELECT COUNT(*) FROM "Jobs"
     WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "JobId" ~ '^[0-9]+$')   AS active_left,
  (SELECT COUNT(*) FROM "ImportedJobsWanted" WHERE "JobId" ~ '^[0-9]+$')   AS staging_left;

COMMIT;
```

---

## 2 · Bucket B — Jobs/ExpiredJobs desync

```sql
BEGIN;

UPDATE "Jobs"
SET "IsActive"=FALSE, "LastUpdated"=NOW()
WHERE "JobSourceId"=2 AND "IsActive"=TRUE
  AND "JobId" IN (SELECT "JobId" FROM "ExpiredJobs");

-- must be 0
SELECT COUNT(*) FROM "Jobs" j JOIN "ExpiredJobs" e ON e."JobId"=j."JobId"
WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE;

COMMIT;
```

---

## 3 · Bucket C — snap ExpireDate to `dateValidThrough`

```sql
BEGIN;

UPDATE "Jobs" j
SET "ExpireDate"  = (ij."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz,
    "LastUpdated" = NOW()
FROM "ImportedJobsWanted" ij
WHERE ij."JobId" = j."JobId"
  AND j."JobSourceId" = 2
  AND LEFT(ij."JobPostEnglish",1) = '{'
  AND (ij."JobPostEnglish"::jsonb->>'dateValidThrough') IS NOT NULL
  AND (ij."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz IS DISTINCT FROM j."ExpireDate";

UPDATE "Jobs"
SET "IsActive"=FALSE, "LastUpdated"=NOW()
WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" < NOW();

-- must be 0
SELECT COUNT(*) FROM "Jobs" j JOIN "ImportedJobsWanted" ij ON ij."JobId"=j."JobId"
WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE AND j."ExpireDate" >= NOW()
  AND LEFT(ij."JobPostEnglish",1)='{'
  AND (ij."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz < NOW();

COMMIT;
```

---

## 4 · Drop stale ExpiredJobs (jobs that came back active)

```sql
BEGIN;

DELETE FROM "ExpiredJobs"
WHERE "JobId" IN (
  SELECT j."JobId" FROM "Jobs" j
  JOIN "ImportedJobsWanted" w ON w."JobId" = j."JobId"
  WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE
);

COMMIT;
```

---

## 5 · Final verify

All `*_remaining` should be `0`. `admin_after` should be `~24,777`.

```sql
SELECT
  (SELECT COUNT(*) FROM "Jobs"
     WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW())                       AS admin_after,
  (SELECT COUNT(*) FROM "Jobs"
     WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "JobId" ~ '^[0-9]+$')                        AS A_remaining,
  (SELECT COUNT(*) FROM "Jobs" j JOIN "ExpiredJobs" e ON e."JobId"=j."JobId"
     WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE)                                              AS B_remaining,
  (SELECT COUNT(*) FROM "Jobs" j JOIN "ImportedJobsWanted" ij ON ij."JobId"=j."JobId"
     WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE AND j."ExpireDate" >= NOW()
       AND LEFT(ij."JobPostEnglish",1)='{'
       AND (ij."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz < NOW())              AS C_remaining;
```

After this, run the .NET indexer (no `-r`) so `PurgeJobs` propagates to ES.

---

## 6 · Match UI count (~19,000)

After steps 1–5, admin lands at ~27,278 but UI is ~19,000. The 8,278 gap is jobs admin marks active but ES rejected on data-quality filters. Pure SQL: deactivate any active CUID job with a data-quality gap.

### 6.1 · Diagnose the gap

```sql
SELECT
  COUNT(*) AS total_active,
  COUNT(*) FILTER (WHERE "EmployerName" IS NULL OR "EmployerName"='') AS no_employer,
  COUNT(*) FILTER (WHERE "City" IS NULL OR "City"='')                  AS no_city,
  COUNT(*) FILTER (WHERE "LocationId"=0)                               AS no_location,
  COUNT(*) FILTER (WHERE "NocCodeId2021" IS NULL)                      AS no_noc,
  COUNT(*) FILTER (WHERE "Title" IS NULL OR "Title"='')                AS no_title,
  COUNT(*) FILTER (WHERE
       "EmployerName" IS NULL OR "EmployerName"=''
    OR "City"         IS NULL OR "City"=''
    OR "LocationId"=0
    OR "NocCodeId2021" IS NULL
    OR "Title" IS NULL OR "Title"='')                                  AS any_gap
FROM "Jobs"
WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW();
```

### 6.2 · Deactivate every active CUID job with any data-quality gap

```sql
UPDATE "Jobs"
SET "IsActive" = FALSE, "LastUpdated" = NOW()
WHERE "JobSourceId" = 2
  AND "IsActive" = TRUE
  AND (
       "EmployerName" IS NULL OR "EmployerName" = ''
    OR "City"         IS NULL OR "City"         = ''
    OR "LocationId" = 0
    OR "NocCodeId2021" IS NULL
    OR "Title" IS NULL OR "Title" = ''
  );
```

### 6.3 · Mark them expired so PurgeJobs drops them from ES

```sql
INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
SELECT "JobId", NOW(), FALSE FROM "Jobs"
WHERE "JobSourceId" = 2 AND "IsActive" = FALSE
  AND ("EmployerName" IS NULL OR "EmployerName"=''
    OR "City" IS NULL OR "City"=''
    OR "LocationId"=0
    OR "NocCodeId2021" IS NULL
    OR "Title" IS NULL OR "Title"='')
ON CONFLICT ("JobId") DO NOTHING;
```

### 6.4 · Verify

```sql
SELECT COUNT(*) AS admin_after
FROM "Jobs"
WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW();
```

- `admin_after ≈ 19,000` → done.
- `< 19,000` → over-deactivated. Re-activate by relaxing predicate (remove `"NocCodeId2021" IS NULL` first):
  ```sql
  UPDATE "Jobs" SET "IsActive"=TRUE
  WHERE "JobSourceId"=2 AND "IsActive"=FALSE
    AND "NocCodeId2021" IS NULL
    AND "EmployerName" <> '' AND "City" <> '' AND "LocationId" <> 0
    AND "Title" <> '';
  ```
- `> 19,000` → ES rejects on something else. Only deterministic fix is the indexer's `--debug` output (run `dotnet WorkBC.Indexers.Wanted.dll --debug`, capture the "JobIds in Jobs but not in Elasticsearch" list, deactivate exactly those).

---

## 7 · Admin vs UI gap diagnostic (post-cleanup)

After steps 1–6 the admin count tracks `Jobs.IsActive=TRUE AND ExpireDate>=NOW()`, but the UI count comes from Elasticsearch. The remaining gap is jobs that admin counts but the indexer rejects or the UI's region/NOC filters hide.

### 7.1 · Segment the active set by every UI-relevant dimension

Boolean-typed columns (`FullTime`, `PartTime`, etc.) compare with `=FALSE`, not `=0`.

```sql
WITH active AS (
  SELECT j.*
  FROM "Jobs" j
  WHERE j."JobSourceId"=2 AND j."IsActive"=TRUE AND j."ExpireDate" >= NOW()
)
SELECT
  COUNT(*)                                                                                              AS admin_count,

  -- Indexer-level rejections (the doc never reaches ES)
  COUNT(*) FILTER (WHERE NOT EXISTS (SELECT 1 FROM "ImportedJobsWanted" ij WHERE ij."JobId" = a."JobId")) AS no_staging_row,
  COUNT(*) FILTER (WHERE EXISTS (SELECT 1 FROM "ImportedJobsWanted" ij
                                  WHERE ij."JobId" = a."JobId" AND LEFT(ij."JobPostEnglish",1)<>'{'))     AS staging_not_json,

  -- UI-level filters (doc reaches ES but UI hides it)
  COUNT(*) FILTER (WHERE "LocationId" = 0)                                                              AS no_location,
  COUNT(*) FILTER (WHERE "NocCodeId2021" IS NULL)                                                       AS no_noc,
  COUNT(*) FILTER (WHERE "City" IS NULL OR "City" = '')                                                 AS no_city,
  COUNT(*) FILTER (WHERE "Title" IS NULL OR "Title" = '')                                               AS no_title,
  COUNT(*) FILTER (WHERE "EmployerName" IS NULL OR "EmployerName" = '')                                 AS no_employer,
  COUNT(*) FILTER (WHERE "Salary" IS NULL)                                                              AS no_salary,
  COUNT(*) FILTER (WHERE "FullTime"=FALSE AND "PartTime"=FALSE AND "Casual"=FALSE
                     AND "Seasonal"=FALSE AND "Permanent"=FALSE AND "Temporary"=FALSE)                  AS no_employment_type,

  -- Any UI-blocker (rows fail multiple buckets, so this is < sum of buckets)
  COUNT(*) FILTER (WHERE
       "LocationId" = 0
    OR "NocCodeId2021" IS NULL
    OR "City" IS NULL OR "City" = ''
    OR "Title" IS NULL OR "Title" = ''
    OR "EmployerName" IS NULL OR "EmployerName" = ''
  )                                                                                                     AS any_ui_blocker,

  -- Jobs that look perfect for ES → should match UI count
  COUNT(*) FILTER (WHERE
       "LocationId" <> 0
    AND "NocCodeId2021" IS NOT NULL
    AND "City"         IS NOT NULL AND "City" <> ''
    AND "Title"        IS NOT NULL AND "Title" <> ''
    AND "EmployerName" IS NOT NULL AND "EmployerName" <> ''
  )                                                                                                     AS clean_for_ui
FROM active a;
```

Reading the result:
- `clean_for_ui ≈ UI count` → gap is purely data-quality; admin includes broken rows.
- `any_ui_blocker ≈ admin_count - UI count` → confirms the gap size.
- Whichever individual bucket is largest = single highest-leverage fix.

### 7.2 · Drill into the top bucket — sample 20 broken rows

Replace the predicate with whichever bucket dominated 7.1:

```sql
SELECT "JobId", "Title", "City", "EmployerName", "LocationId", "NocCodeId2021", "ExpireDate"
FROM "Jobs"
WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW()
  AND "LocationId" = 0          -- ← swap to whichever bucket dominated
LIMIT 20;
```

Then look at the staging JSON for those JobIds to see why `map()` produced `LocationId=0` / `NULL NocCodeId2021`:

```sql
SELECT ij."JobId",
       ij."JobPostEnglish"::jsonb->'jobLocations'  AS locations_json,
       ij."JobPostEnglish"::jsonb->'nocMatches'    AS noc_matches_json
FROM "ImportedJobsWanted" ij
WHERE ij."JobId" IN ( /* paste 5–10 JobIds from above */ );
```

### 7.3 · Option A — deactivate broken rows so admin matches UI

```sql
UPDATE "Jobs"
SET "IsActive"=FALSE, "LastUpdated"=NOW()
WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW()
  AND (
       "LocationId" = 0
    OR "NocCodeId2021" IS NULL
    OR "City" IS NULL OR "City" = ''
    OR "Title" IS NULL OR "Title" = ''
    OR "EmployerName" IS NULL OR "EmployerName" = ''
  );

INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
SELECT "JobId", NOW(), FALSE FROM "Jobs"
WHERE "JobSourceId"=2 AND "IsActive"=FALSE AND "ExpireDate" >= NOW()
  AND (
       "LocationId" = 0
    OR "NocCodeId2021" IS NULL
    OR "City" IS NULL OR "City" = ''
    OR "Title" IS NULL OR "Title" = ''
    OR "EmployerName" IS NULL OR "EmployerName" = ''
  )
ON CONFLICT ("JobId") DO NOTHING;
```

After this, admin should match UI. Re-run the indexer (no `-r`) so `PurgeJobs` removes the deactivated docs from ES.

### 7.4 · Option B — keep rows active, fix `map()` instead

If most failures are `LocationId=0` due to city-name mismatches (e.g. "Vancouver, BC" vs "Vancouver"), the fix belongs in `JobImportService::getBestAvailableLocationId()` (PHP) — improve the fuzzy match, or fall back to province when city is unknown. Then re-run import + reindex.

If most failures are `NocCodeId2021=NULL`, the staging JSON's `nocMatches` is empty/invalid for those rows — Innovibe's NOC tagger missed them. Either accept the gap, or relax `isValidNoc2021()` to allow a default code.

### 7.5 · Verify

```sql
SELECT COUNT(*) AS admin_after
FROM "Jobs"
WHERE "JobSourceId"=2 AND "IsActive"=TRUE AND "ExpireDate" >= NOW();
```

Compare to UI count. Should now match (within ~50 for in-flight imports).
