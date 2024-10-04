Importing ddata manually into PostgreSQL
========================================

## Order of import
Restore using DBeaver unless otherwise noted.

| File | Actions |
| ---- | ------- |
| jobboard-schema.sql | Restore using `docker-compose-jb exec -T postgres psql -U workbc jobboard < scripts/sql/WBCAMS-450/jobboard-schema.sql` |
| ___EFMigrationsHistory | |
| AdminUsers | Skip `id`, `LockedByAdminUserId`, `ModifiedByAdminUserId` |
| Countries | |
| Regions | |
| Provinces | |
| NocCodes | |
| NocCodes2021 | |
| NocCategories | |
| NocCategories2021 | |
| Locations | |
| Industries | |
| SecurityQuestions | |
| JobSeekerStatLabels | |
| WeeklyPeriods | Skip `id` |
| DataProtectionKeys | Skip `id` |
| GeocodedLocationCache | Skip `id` |
| AspNetUsers | Set Importer Setting "Set empty strings to NULL" |
| ImpersonationLog | |
| JobSeekerAdminComments | Skip `id` |
| JobSeekerChangeLog | Skip `id` |
| JobSeekerEventLog | Skip `id` |
| JobSeekerFlags | Skip `id` |
| JobSeekerStats | |
| JobSeekerVersions | Skip `id` |
| JobAlerts | Skip `id` |
| JobSources | |
| ReportPersistenceControl | |
| JobStats | |
| JobIds | |
| Jobs | |
| JobVersions | Skip `id` |
| JobViews | |
| SavedJobs | Skip `id` |
| SavedCareerProfiles | Skip `id` |
| SavedIndustryProfiles | Skip `id` |
| ImportedJobsFederal | |
| ImportedJobsWanted | |
| DeletedJobs | |
| ExpiredJobs | |

- TODO `AdminUsers.LockedByAdminUserId` and `AdminUsers.LockedByAdminUserId`
