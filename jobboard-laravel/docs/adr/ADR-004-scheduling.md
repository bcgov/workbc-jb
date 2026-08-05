# ADR-004: Scheduling — retain pg_cron for DB-resident work

- **Status:** Accepted
- **Date:** 2026-06-26
- **Deciders:** Architecture owner
- **Jira:** —

## Context
The existing database has the **pg_cron** extension (`cron.job`, `cron.job_run_details`).
Scheduled DB-resident work — the stats stored procedures (`usp_GenerateJobStats`,
`usp_GenerateJobSeekerStats`) and `tvf_*`-backed refreshes — currently runs on pg_cron **inside
PostgreSQL**. The rewrite must not duplicate scheduling or run the same job in two schedulers.

## Decision
**Retain pg_cron** as the scheduler for **DB-resident scheduled work** (the stats procedures and
any SQL-level maintenance). The Laravel app **calls** those procedures; **pg_cron invokes them on
schedule** — the app does not re-schedule or reimplement them.

pg_cron runs **SQL only**, so app-level queued jobs that cannot be expressed as SQL — **feed
imports, indexing, and job-alert notifications** — are triggered **outside** pg_cron. **Confirmed
mechanism:** these run as **Kubernetes CronJobs** (as they do today for the importer, indexer, and
notification containers); in the rewrite each k8s CronJob invokes a `php artisan` command that
dispatches to the **Redis queue**. **Do not schedule the same job in both pg_cron and the app.**

## Consequences
- **Positive:** keeps proven, in-DB stats scheduling untouched; one scheduler per job; no
  duplication.
- **Negative:** two scheduling mechanisms exist (pg_cron for SQL; k8s/Laravel cron for app jobs) —
  documented and bounded by job type, so ownership is unambiguous.

## Alternatives Considered
- **Move all scheduling to the Laravel scheduler:** rejected — reimplements working in-DB
  scheduling; the stats procs are SQL-native and already scheduled.
- **Trigger app jobs from pg_cron** (via `pg_net`/http or a queue-table poll): rejected as the
  default — convoluted; a normal cron→artisan trigger is simpler and observable.

## Compliance / Scope
Standing rule. **pg_cron** owns DB-resident scheduled work (stats procs); **Kubernetes CronJobs**
own app-level scheduling (imports, indexing, notifications) → `php artisan` → Redis queue.
