# ADR-006: The public search embeds into the Drupal page via iframe (Drupal owns chrome)

- **Status:** Accepted — **extended by [ADR-009](ADR-009-same-site-session-auth-for-embed.md)**
- **Date:** 2026-07-23
- **Deciders:** Architecture owner
- **Jira:** —

> **ADR-009 extends this decision** with what was unknown when it was written: the app must be served
> **same-site** (`*.workbc.ca`) for session auth to survive the boundary; CORS/cache behaviour for that
> host is **CDN-owned**; the embed touches **five** surfaces, not one (search, account, the
> career/industry save button, the Drupal header's login state, and the server-to-server widgets); and
> **login is deliberately not placed inside a cross-origin iframe**. §5 below ("the frame and the app
> must agree on auth — covered when that epic is scoped") is the part ADR-009 scopes.

## Context
WorkBC.ca is a **Drupal** site that owns the page chrome (header, nav, footer, branding) and the
"Find a Job" page content. Today the search UI is an **Angular SPA** (`jb-search`) embedded *inline*
into that Drupal page (`/find-job/search-jobs`), rendered client-side with **hash routing**
(`#/job-search;…`). Confirmed from the live site: the chrome is server-rendered by Drupal; the search
UI is a client-side app injected into a container.

Two facts shape this decision:
1. **SEO is not a driver.** The current hash-routed SPA is not crawlable, and the product owner has
   confirmed search-engine indexing is **not a requirement**. (This weakens — but does not reverse —
   the server-rendering rationale in ADR-002; Livewire is already built.)
2. The rewrite should be **"a stack update"** — swap the .NET+Angular service for the Laravel service
   at the same slot, with **minimal infrastructure change**. Our app is server-rendered Blade +
   Livewire (ADR-002), which **cannot** be inline-embedded the way a client-side SPA can (Livewire
   needs its own server request/response cycle).

## Decision
**The public search embeds into the Drupal-hosted page via an `<iframe>`.** Specifically:
- **Drupal owns** the page, the chrome (header/nav/footer/branding), and any hero/intro copy
  (including the "Search {N} jobs in B.C." heading — the count comes from our `gettotaljobs` API).
- **Our Laravel `/jobs`** renders a **chrome-less** search UI (no WorkBC header/footer) to be shown
  inside the iframe. It stays a separately-deployed service the WorkBC infrastructure routes to (as
  the current job-board service is today).
- **Deep-links** — shareable searches (SRCH-6) and email-alert links — **open our app directly**
  (its own URL), not through the iframe (the browser URL inside a frame does not reflect search
  state). Alert emails point at the app URL.
- **Auto-height** via `postMessage` (the app posts its content height; a small Drupal snippet sizes
  the frame).
- The **Drupal-facing JSON API** (SRCH-10) is unchanged — Drupal keeps calling `/api/*` for the
  "Recent Jobs" and other embedded result widgets on career/industry/region pages.

## Consequences
**Easier / positive**
- Drupal remains the **single source** of chrome and page content — the WorkBC web team's existing
  workflow, with **no chrome to replicate or keep in sync** on our side.
- **Loose coupling** (an iframe `src`); our app is a self-contained search service in the same
  deployment slot — genuinely "a stack update," minimal infra change.
- **Less UI work** for us: FND-4 needs a bare layout, not a full WorkBC-matching site chrome.

**Trade-offs accepted**
- **iframe UX** must be handled: auto-height (`postMessage`), scroll, mobile, and focus/keyboard
  a11y across the frame boundary. (WCAG 2.1 AA still applies inside the frame — see the a11y gate.)
- The **browser URL does not reflect search state** inside the frame → shareable/alert links open
  the app directly (handled above).
- **Search content in the frame is not indexed** to the parent URL — accepted, since SEO is not a
  requirement.

**Follow-up work**
- **FND-4:** the base layout must support a **chrome-less "embed" mode** (no header/footer) for the
  iframe, alongside a minimal standalone layout for local dev / direct access.
- Add the `postMessage` height bridge; point alert/share links at the app URL.
- Confirm the Drupal host page + the reverse-proxy/routing that exposes the app to the iframe.
- **Retain** the SEO artifacts already built (sitemap, canonical, `JobPosting` JSON-LD) — harmless
  in a frame and they preserve a future "flip to crawlable direct access" option — but they are no
  longer drivers.

## Alternatives Considered
- **Reverse-proxy the search URL to Laravel; our app renders WorkBC chrome** (server-rendered full
  page at `workbc.ca/find-job/search-jobs`). Rejected: forces us to replicate/sync WorkBC chrome and
  tightens coupling; only worth it if SEO or URL-in-the-address-bar mattered, which they don't.
- **Drupal server-side-include (SSI/ESI/module) of our HTML fragment.** Rejected: complex with
  Livewire's JS + `wire:` endpoints; needs Drupal-module work — more moving parts, not less.
- **Inline-bundle embed like the current Angular SPA.** Rejected: not possible for server-rendered
  Livewire (no client bundle to drop into the Drupal DOM).

## Compliance / Scope
Relates to **ADR-002** (server-rendered Blade + Livewire): that decision stands (already built), but
its SEO justification is downgraded — Livewire is retained for its own merits, not for crawlability.
Standing rule: the public search is delivered **chrome-less for iframe embedding**; FND-4's layout
must provide an embed mode. WCAG 2.1 AA still applies within the frame.
