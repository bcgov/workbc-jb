# WorkBC Job Board — Laravel Rewrite

The **app/web layer** for the WorkBC Job Board: public job search + detail, the job-seeker account,
the Filament admin, reporting, and the Drupal-facing JSON API — built on **Laravel 11 / PHP 8.3**
with **Blade + Livewire + Filament 3** (server-rendered, no SPA).

> This repo currently contains the **planning package** (agent guardrails, architecture, decisions,
> epics, and a Jira import). The Laravel application itself is scaffolded in **FND-1** (see the
> Foundation epic).

## Non-negotiable scope & constraints

- **Existing database, existing data** — the app runs on the current PostgreSQL schema. Eloquent
  **maps** to the existing PascalCase tables; we **do not** create a new schema or alter existing
  tables. See `docs/data-model.md §0`.
- **App/web layer only** — the **feed importers** and the **OpenSearch indexer** are existing PHP
  **standalone containers**, reused as-is and **not** modified or reimplemented here. The app reads
  `Jobs` / OpenSearch. See `docs/architecture.md §1a`.
- **Develop against the TEST environment, never production** — because the app maps to the existing
  schema, use the test database / OpenSearch / Redis for development.
- **Accessibility (WCAG 2.1 AA)** and **FOIPPA** (no PII in logs) are hard requirements.

## Read these first

| Doc | What it is |
|---|---|
| [`.github/copilot-instructions.md`](.github/copilot-instructions.md) | Always-on guardrails: stack, enforced constraints, DoD, self-check |
| [`docs/architecture.md`](docs/architecture.md) | Structure, rules, flows, scope |
| [`docs/glossary.md`](docs/glossary.md) | Domain terms (IsActive vs ExpireDate, JobSource, …) |
| [`docs/data-model.md`](docs/data-model.md) | Existing schema → Eloquent mapping |
| [`docs/contracts.md`](docs/contracts.md) | `JobSearchFilters` + the Drupal-facing API |
| [`docs/adr/`](docs/adr/) | Architecture Decision Records (001–004) |
| [`docs/epics/`](docs/epics/) | Foundation + Search + Account + Admin/Reporting epics |
| [`docs/jira-import.csv`](docs/jira-import.csv) | Bulk-import file for the ticket backlog |

## Stack

Laravel 11 · PHP 8.3 · Blade + Livewire (+ Alpine, bundled) · Filament 3 · PostgreSQL 15 ·
Redis 7 (cache/queue/session) · OpenSearch · AWS EKS via GitHub Actions.

## Getting started

1. Read `.github/copilot-instructions.md` and `docs/architecture.md`.
2. Work the **Foundation epic** in order (FND-1 → FND-8), then Search → Account → Admin/Reporting.
3. **FND-1** scaffolds the Laravel app and wires connections to the **existing** DB / Redis / OpenSearch
   (test environment). One ticket per PR; every PR completes the self-check in the copilot instructions.

## Auth (summary — see ADR-003)

Job seekers → Laravel **session** auth (verify legacy ASP.NET Identity hashes, rehash to bcrypt;
email-only reset). Admin → **Keycloak OIDC** → Filament.
