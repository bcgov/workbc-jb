# ADR-002: Server-rendered Blade + Livewire; no SPA

- **Status:** Accepted
- **Date:** 2026-06-26
- **Deciders:** Architecture owner
- **Jira:** —

## Context
Individual job pages must be indexable by search engines (Google Jobs, per-listing indexing) —
a core Job Board requirement. The current Angular SPA renders job content client-side and uses
hash routing (`#/job-details/{id}`), so crawlers see an empty shell and detail pages are not
independently indexable. The rewrite must fix this. The stack is already Laravel/PHP (importers,
Filament admin, Drupal parent site), and the app is predominantly content, forms, and faceted
search — not an app-like, real-time surface.

## Decision
We will build a **server-rendered** application — **no SPA, no Inertia/React**:
- **Public pages (search results, job detail):** **Blade**, fully server-rendered. Job-detail
  pages emit `schema.org/JobPosting` structured data; a `sitemap.xml` lists job URLs; URLs are
  **path-based and crawlable** (no hash routing).
- **Authenticated job-seeker area** (dashboard, saved jobs, alerts, recommended jobs, profiles,
  settings): **Blade + Livewire** for interactivity (behind login; SEO not required).
- **Admin:** **Filament 4**.
- **Alpine.js** (bundled with Livewire) for view state; **Livewire** for data-backed reactivity.

## Consequences
- **Positive:** SEO fixed everywhere by default; one language (PHP) end-to-end; no Node/SSR
  service to operate; Filament covers the most interactive surface (admin) for free; faceted
  search (Livewire's strength) fits naturally.
- **Negative:** no React ecosystem / SPA-style client navigation; rich client-only interactions
  (if ever needed) require a deliberate JS island.
- **Follow-up:** define the Blade layout + Livewire setup in the Foundation epic; preserve the
  existing search URL/query-param format (alerts deep-link via stored `UrlParameters`).

## Alternatives Considered
- **Inertia + React (no SSR):** rejected — reproduces the SEO gap for public job pages.
- **Inertia + React with SSR:** rejected — adds a Node SSR service and its ops for UX the app
  doesn't require.
- **All-Blade, no Livewire:** rejected — the alert builder and faceted search need reactive
  server components; Livewire provides them without React.

## Compliance / Scope
Standing rule. Supersedes the "Inertia + React, no SSR" line in the draft standards. The **BC
Design System is not a required dependency** — the app uses its own Blade + Livewire component
library — so there is no BCDS-driven pull toward React. The frontend decision is fully settled:
server-rendered Blade + Livewire + Filament, no React.
