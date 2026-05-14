-- ONE-TIME FIX: Refresh ExpireDate on active Wanted jobs to match the JSON
-- payload, then deactivate any that are now past expiry.
-- Idempotent — safe to re-run. Run the diagnostic SELECT first to preview.

-- ── Diagnostic (dry-run preview) ─────────────────────────────────────
WITH new_exp AS (
  SELECT
    j."JobId",
    j."ExpireDate"  AS old_exp,
    COALESCE(
      (w."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz,
      (w."JobPostEnglish"::jsonb->>'updatedAt')::timestamptz + INTERVAL '30 days'
    ) AS new_exp
  FROM "Jobs" j
  JOIN "ImportedJobsWanted" w
    ON w."JobId" = j."JobId"
   AND NOT w."IsFederalOrWorkBc"
  WHERE j."JobSourceId" = 2
    AND j."IsActive" = TRUE
    AND LEFT(w."JobPostEnglish", 1) = '{'
)
SELECT
  COUNT(*)                                                          AS total_active_wanted,
  COUNT(*) FILTER (WHERE new_exp IS DISTINCT FROM old_exp)          AS to_update,
  COUNT(*) FILTER (WHERE new_exp IS NOT NULL AND new_exp <= NOW())  AS expired_after_fix,
  COUNT(*) FILTER (WHERE new_exp IS NOT NULL AND new_exp > NOW())   AS still_active_after_fix
FROM new_exp;

-- ── Fix (run after reviewing the diagnostic) ─────────────────────────
BEGIN;

-- 1. Refresh ExpireDate to match the ES expiry rule
UPDATE "Jobs" j
SET "ExpireDate" = COALESCE(
      (w."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz,
      (w."JobPostEnglish"::jsonb->>'updatedAt')::timestamptz + INTERVAL '30 days'
    ),
    "LastUpdated" = NOW()
FROM "ImportedJobsWanted" w
WHERE w."JobId" = j."JobId"
  AND NOT w."IsFederalOrWorkBc"
  AND j."JobSourceId" = 2
  AND LEFT(w."JobPostEnglish", 1) = '{'
  AND COALESCE(
        (w."JobPostEnglish"::jsonb->>'dateValidThrough')::timestamptz,
        (w."JobPostEnglish"::jsonb->>'updatedAt')::timestamptz + INTERVAL '30 days'
      ) IS DISTINCT FROM j."ExpireDate";

-- 2. Mark newly-expired jobs for ES removal
INSERT INTO "ExpiredJobs" ("JobId", "DateRemoved", "RemovedFromElasticsearch")
SELECT "JobId", NOW(), FALSE
FROM "Jobs"
WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW()
ON CONFLICT ("JobId") DO UPDATE
  SET "DateRemoved" = NOW(), "RemovedFromElasticsearch" = FALSE;

-- 3. Remove expired jobs from the staging table
DELETE FROM "ImportedJobsWanted"
WHERE "JobId" IN (
  SELECT "JobId" FROM "Jobs"
  WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW()
);

-- 4. Deactivate the expired rows
UPDATE "Jobs"
SET "IsActive" = FALSE, "LastUpdated" = NOW()
WHERE "JobSourceId" = 2 AND "IsActive" = TRUE AND "ExpireDate" <= NOW();

COMMIT;
