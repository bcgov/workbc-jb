# Architecture Decision Records (ADRs)

An **ADR** captures one significant architecture decision: its context, the decision, and its
consequences. ADRs are how we record *why* the system is the way it is so future changes —
and coding agents — don't unknowingly undo a deliberate choice.

## Rules
- **One decision per ADR.** Numbered `ADR-NNN-short-title.md`, append-only.
- ADRs are **never edited after `Accepted`** — you *supersede* (a new ADR sets the old one's
  status to `Superseded by ADR-XXX`), you don't rewrite.
- When a Jira story requires deviating from an Architecture Rule or a prior ADR, add an ADR
  (Proposed) and link it from the story before implementing.

## Index
| ADR | Title | Status |
|---|---|---|
| [ADR-001](ADR-001-opensearch-derived-read-model.md) | OpenSearch is a derived read model | Accepted |
| [ADR-002](ADR-002-rendering-strategy.md) | Server-rendered Blade + Livewire; no SPA | Accepted |
| [ADR-003](ADR-003-authentication.md) | Session auth (seekers) + Keycloak OIDC (admin); email-only reset | Accepted |
| [ADR-004](ADR-004-scheduling.md) | Scheduling — retain pg_cron for DB-resident work | Accepted |
| [ADR-005](ADR-005-laravel-12-filament-4.md) | Adopt Laravel 12 + Filament 4 (Laravel 11 security-EOL) | Accepted |
| [ADR-006](ADR-006-drupal-embed-iframe.md) | Public search embeds into the Drupal page via iframe (Drupal owns chrome); SEO not a driver | Accepted |
| [ADR-007](ADR-007-verify-legacy-md5-passwords.md) | Verify legacy MD5-wrapped password hashes on login + rehash (amends ADR-003; avoids resetting 62% of users) | Accepted |
| [ADR-008](ADR-008-admin-auth-phased-delivery.md) | Build FND-6's admin-auth substance now (guard/policies/impersonation); defer the Keycloak handshake to FND-6b | Accepted |

## Template

```markdown
# ADR-NNN: <short decision title>

- **Status:** Proposed | Accepted | Superseded by ADR-XXX | Deprecated
- **Date:** YYYY-MM-DD
- **Deciders:** <names/roles>
- **Jira:** <JOBS-123, if triggered by a story>

## Context
The problem/forces requiring a decision. Constraints, requirements, current situation. Facts, not opinions.

## Decision
The choice, in active voice ("We will …"), specific enough to act on without interpretation.

## Consequences
What becomes easier and harder. Positive outcomes; trade-offs accepted; follow-up work
(migrations, reindex, config, docs).

## Alternatives Considered
Each option and why it was rejected. (Prevents re-litigating.)

## Compliance / Scope
Which Architecture Rules/standards this affects; one-time exception vs standing rule the agent must follow.
```
