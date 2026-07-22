# ADR-005: Adopt Laravel 12 + Filament 4 (Laravel 11 is security-EOL)

- **Status:** Accepted
- **Date:** 2026-07-21
- **Deciders:** Architecture owner
- **Jira:** —

## Context
Planning assumed **Laravel 11 + Filament 3**. At scaffold time (July 2026), Composer 2 **blocked
every Laravel 11.x release** (11.31–11.55) because they are covered by active security advisories —
Laravel 11 has reached **security end-of-life**. A BC-gov application cannot ship on a framework
line with unpatched security advisories.

## Decision
Adopt **Laravel 12** (the current, security-supported line) with **Filament 4** (Filament 3 does
**not** support Laravel 12). **PHP 8.3 is retained** (supported by Laravel 12). All planning docs,
epics, and the Jira import were updated from "Laravel 11 / Filament 3" to "Laravel 12 / Filament 4".

## Consequences
- Current, security-supported framework; no shipping on an EOL line.
- **Filament 4** (not 3) — minor API differences vs. the version some references were written
  against; verify Filament APIs against the v4 docs during Admin work.
- Ongoing: keep the framework on a supported line — don't let it drift to EOL again.

## Alternatives Considered
- **Stay on Laravel 11, ignore the advisories** (`policy.advisories.block false`): rejected —
  ships a framework with known, unpatched vulnerabilities.
- **Laravel 12 + Filament 3:** rejected — incompatible (Filament 3 supports Laravel 10/11 only).

## Compliance / Scope
Supersedes the "Laravel 11 / Filament 3" version notes throughout the docs. Standing rule: stay on
a security-supported Laravel line.
