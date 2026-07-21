# ADR-001: OpenSearch is a derived read model

- **Status:** Accepted
- **Date:** 2026-06-26
- **Deciders:** Architecture owner
- **Jira:** —

## Context
In the current system, the search indexer independently recomputes job fields (`ExpireDate`,
salary) that the importer already computes, then drifts out of sync. This hid ~8,600 active
jobs from search (search-visible 26,889 vs DB date-active 35,531) because the indexer used a
different expiry basis than `Jobs.ExpireDate`. We need an unambiguous rule for the role of
OpenSearch relative to PostgreSQL.

## Decision
We will treat OpenSearch strictly as a **derived read model** of the `Jobs` table. Derived job
fields — `ExpireDate`, annualized `Salary`, NOC/location resolution, min-wage eligibility — are
computed **once** in the ingestion pipeline, written to `Jobs`, and **copied verbatim** into
OpenSearch documents by the indexer. The indexer **never recomputes** a derived field. The full
index must be **rebuildable from PostgreSQL** at any time; no data may exist only in OpenSearch.

## Consequences
- **Positive:** eliminates importer/indexer drift; single source of truth; the index is
  disposable and rebuildable; redeploys are non-destructive.
- **Negative:** the indexer must read `Jobs` (a DB read per batch) rather than parse the staged payload alone.
- **Follow-up:** indexer reads `Jobs.ExpireDate` (and other derived fields) directly; add a
  full-reindex command; add a check/review guard against any recompute of a derived field.

## Alternatives Considered
- **Keep recomputing but match the constant (e.g. 30→90 days):** rejected — cannot reproduce
  refresh-extended values (observed 90 *and* 157 days) and reintroduces drift.
- **Filter on `IsActive`/`ExpireDate` in the search query only:** rejected — treats the symptom, not the divergence.

## Compliance / Scope
Standing rule. Enforces Architecture Rules A and B. Applies to all indexers (federal and external).
