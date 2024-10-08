Importing data manually into PostgreSQL
========================================

- Obtain a package of exported CSV files from a recent snapshot of MSSQL WorkBC_JobBoard.
- Restore using DBeaver unless otherwise noted. Specific instructions given for each file.
- TODO Import `AdminUsers.LockedByAdminUserId` and `AdminUsers.LockedByAdminUserId`

| File | Instructions |
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
