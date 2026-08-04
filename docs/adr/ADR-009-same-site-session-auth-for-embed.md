# ADR-009: Same-site session auth for the Drupal-embedded app

- **Status:** Accepted
- **Date:** 2026-07-29
- **Deciders:** Architecture owner
- **Jira:** —

## Context

[ADR-006](ADR-006-drupal-embed-iframe.md) decided the search UI embeds into WorkBC.ca via an
`<iframe>`, and [ADR-003](ADR-003-authentication.md) decided job seekers authenticate with a
**Laravel session** (no JWT). Neither ADR established *how* a session survives the boundary between
the Drupal parent page and our app, because the actual integration mechanism had not been inspected.

Investigation of the live Drupal module (`workbcdurpal`, branch `dev2`) and the production
infrastructure established the following facts. They are recorded here because several contradict
what `contracts.md` and `drupal-embed.md` previously asserted.

**How the integration actually works today**

1. The Angular apps are **inline-injected** into the Drupal page DOM (`<app-root>` + six JS bundles),
   not iframed — so they run on the **`www.workbc.ca` origin**. There are two builds:
   `dist/jb-search` (find-jobs page) and `dist/jb-account` (account page, which owns login).
2. Because of that shared origin, the JWT cookie Angular sets (host-only, `Path=/`) is trivially
   readable by Drupal's own JS. `js/workbc_jobboard.js` reads `currentUser.token` and calls the
   career/industry profile endpoints **from the browser** with `Authorization: Bearer …`.
3. Drupal's *header nav* also reads those cookies (`currentUser.username`) to toggle
   logged-in/logged-out menu items, and `navLogout()` clears six `currentUser.*` cookies.
4. `WorkBcJobboardController.php` contains `saveProfile`/`statusProfile`/`saveIndustryProfile`/
   `statusIndustryProfile` branches — including `Authorization` forwarding — but **nothing calls
   them**. It is dead code, superseded by (2). This is the origin of the incorrect
   "Drupal forwards the job seeker's Authorization header" claim in `contracts.md §2.4`.
5. Genuine server-to-server calls from Drupal PHP are only three: `api/Search/JobSearch`
   (Recent Jobs block), `api/Search/gettotaljobs` (sitewide count), `api/location/cities`
   (autocomplete on Drupal's own form).

**Production infrastructure (verified)**

| Fact | Value |
|---|---|
| App reachable at | `api-jobboard.workbc.ca` (CloudFront `E3D5JEAK4A4WX7`) **and** `workbc-jb.a55eb5-prod.stratus.cloud.gov.bc.ca` (origin, directly reachable) |
| Prod Drupal currently points at | the **Stratus** hostname — i.e. **cross-site** with `www.workbc.ca` |
| Cookie forwarding | `ForwardedValues.Cookies.Forward: all` — cookies reach the origin, and are in the cache key |
| Cached methods | `HEAD, GET` only — Livewire's POSTs are never cached |
| `DefaultTTL` | `86400` (24h), `Behaviors: Quantity 0` (no path-specific carve-outs) |
| Response headers policy | `cors-api-jobboard`: origins `https://www.workbc.ca` + `https://workbc.ca`, **`AccessControlAllowCredentials: true`**, `OriginOverride: true`. No `X-Frame-Options`, no CSP |
| Origin custom header | `X-Forwarded-Host: api-jobboard.workbc.ca` |
| Sibling hostname | `admin-jobboard.workbc.ca` exists (convention: `<role>-jobboard.workbc.ca`) |
| `jobs.workbc.ca` | unregistered — and **not planned**; see the origin map below |

### Origin map (confirmed 2026-08-04, all six DNS-verified)

| Env | Drupal parent | App (job seeker) | Admin (Filament) |
|---|---|---|---|
| **prod** | `www.workbc.ca` | `api-jobboard.workbc.ca` | `admin-jobboard.workbc.ca` |
| **test** | `test.workbc.ca` | `test-api-jobboard.workbc.ca` | `test-admin-jobboard.workbc.ca` |
| **dev** | `dev.workbc.ca` | `dev-api-jobboard.workbc.ca` | `dev-admin-jobboard.workbc.ca` |

All nine hosts resolve; the six job-board hosts are each their own CloudFront distribution. Each
Drupal environment currently points at its matching Stratus origin (`a55eb5-{prod,test,dev}`), so
the config repoint described here applies **per environment**.

Three consequences:

1. **Every environment is same-site under `workbc.ca`**, so this ADR's session model holds in all
   three — not only production — and `TrustHosts` can safely use `^(.+\.)?workbc\.ca$` without
   locking out test or dev.
2. **`frame-ancestors` is now writable per environment** (FND-4): `https://www.workbc.ca`,
   `https://test.workbc.ca`, `https://dev.workbc.ca` respectively. Make it configurable, never `*`.
3. **The admin panel is served from a different origin** than the seeker app, so it carries its own
   session cookie. Same-site, so nothing breaks — but ADM-1 onwards should assume two origins, not
   one.

**No user-facing hostname is planned.** The seeker app stays on `api-jobboard.workbc.ca`, so the
ACCT-7 login page shows users a host prefixed `api-`. Recorded as a deliberate position rather than
an oversight; revisit only if a nicer name is provisioned.

The decisive constraint: a **bearer token in a header is immune to same-site cookie rules**, which is
what makes the current cross-site topology work. A session cookie is not. So ADR-003's session
decision is only viable if the app is served from a hostname that is *same-site* with the Drupal parent.

## Decision

**The app MUST be served to browsers from a `*.workbc.ca` hostname** — `api-jobboard.workbc.ca`
today. `www.workbc.ca` and `api-jobboard.workbc.ca` share the registrable domain `workbc.ca`, so they
are **same-site** (different origins, same site). Consequently:

1. **Session cookies work** across the boundary with plain `SameSite=Lax` (Laravel's default) — for
   the iframe load *and* for credentialed cross-origin `fetch` from Drupal pages. `SameSite=None` is
   **not** used; it is the more fragile setting and is unnecessary here.
2. **ADR-003 stands unchanged.** No JWT, no token bridge, no reversal.
3. **Bearer-token auth is removed** from our API surface. `EnsureJobSeekerToken` (which enforced a
   contract that never existed in practice — see fact 4) is replaced by session authentication
   returning **401** when unauthenticated.
4. **Drupal's JS drops all token handling**: no `readCookie('currentUser.token')`, no `Authorization`
   header; instead `withCredentials`, and it branches on **401** rather than on token presence.
5. **CORS is infrastructure-owned, not application-owned.** The CloudFront response-headers policy
   sets `OriginOverride: true`, so it overrides whatever the app emits — `config/cors.php` has no
   effect on this host. Changing the CORS contract is a **CloudFront change, not a code deploy**.
6. **The Drupal-facing profile API in `contracts.md §2.4` is retired** as a server-to-server contract.
   Career/industry profile save/status are ordinary session-authenticated routes called from the
   browser. The three genuine server-to-server endpoints (fact 5) are unaffected.

### Binding implementation constraints

- **Trusted proxies (done).** `bootstrap/app.php` trusts `X-Forwarded-Host`/`-Proto`/`-Port`/`-For`.
  Without this, `url()`/`route()` build absolute URLs from the *origin* hostname, corrupting SRCH-7
  canonical/hreflang links, every SRCH-8 sitemap URL, and post-login redirects. Regression test:
  `tests/Feature/TrustedProxiesTest.php`.
- **Cache-Control.** With `DefaultTTL: 86400` and no path behaviours, any response lacking an explicit
  cache header is cacheable for 24h. Laravel's session middleware already emits
  `no-cache, no-store, private` on every `web` route (verified), so this is satisfied today — but it
  is now a **requirement**, not an incidental default. Any route added outside the session-bearing
  `web` group must set it explicitly.
- **CSRF across origins.** The standard `XSRF-TOKEN` cookie → `X-XSRF-TOKEN` header double-submit
  pattern **cannot** work here: Drupal's JS on `www.workbc.ca` cannot read a cookie belonging to
  `api-jobboard.workbc.ca`. Instead, the **status response carries the token** (`{saved, csrf}`) and
  the save POST returns it — no extra round-trip, since status is already called on page load. The
  header must also be added to `AccessControlAllowHeaders`, which currently lacks it.
- **Host spoofing.** `TRUSTED_PROXIES=*` is used because CloudFront's ingress IP set is large and
  changing. Since the Stratus origin is *also* directly reachable, `X-Forwarded-Host` is spoofable by
  anyone who can reach it (URL poisoning in canonical links, sitemap entries, password-reset links).
  **Mitigate with `TrustHosts` in production**, constraining hosts to `^(.+\.)?workbc\.ca$` — not by
  narrowing the proxy list. Deliberately not enabled yet: the pattern is env/environment-specific and
  a wrong value throws `SuspiciousOperationException` on every request.
- **Login is not placed inside a cross-origin iframe.** Password managers are unreliable at
  autofilling in cross-origin frames and the user cannot verify the URL they are entering credentials
  into. The account area (which owns login) is therefore a **full-page navigation**, not an embed —
  see `drupal-embed.md §3` surface 2.

## Consequences

- **Positive:** ADR-003 survives intact; no token infrastructure to build or secure; the required CORS
  (`AllowCredentials: true` from both parent origins) is *already configured*; nothing at the CDN
  blocks framing; Drupal's JS gets simpler (token handling deleted).
- **Positive:** the CDN change needed is a **config repoint** (Drupal's `jobboard_api_url_frontend` →
  `api-jobboard.workbc.ca`), not new infrastructure — the alias exists and serves the app today.
- **Negative:** CDN caching does almost nothing for our HTML. Cookies are in the cache key and Laravel
  sets a session cookie for essentially every visitor, so each user gets private cache entries. Public
  search performance therefore rests on our Redis caching and OpenSearch latency, not CloudFront. A
  dedicated cookie-stripping behaviour for genuinely public paths is possible later if needed.
- **Negative:** CORS being CDN-owned splits ownership — an app-side developer debugging a CORS failure
  will find no answer in the codebase. Hence this ADR.
- **Negative:** two Drupal behaviours beyond the save button break and must be rebuilt: the header's
  logged-in menu state and `navLogout()` (both read cookies that will no longer be same-origin).

## Alternatives Considered

- **Keep bearer tokens** (stay on the Stratus hostname): rejected — reverses ADR-003, reintroduces a
  non-revocable client-side credential, and requires token issuance/refresh/logout-sync plumbing in
  Drupal, all to avoid a config repoint to a hostname that already exists.
- **`SameSite=None; Secure` session cookies:** rejected — unnecessary once same-site, and actively
  fragile (Safari ITP blocks third-party cookies outright; Chrome is phasing them out).
- **Reverse-proxy the app under `www.workbc.ca/…`:** rejected for now — fully same-origin and the
  cleanest technically, but requires CDN/ALB path-routing work and makes Drupal and Laravel share a
  URL namespace. Worth revisiting only if the iframe's ergonomics prove insufficient.
- **A short-lived signed token minted by Laravel and forwarded by Drupal:** rejected — a partial
  reversal of ADR-003 to solve a problem that same-site hosting solves for free.

## Compliance / Scope

Standing rule. The app is served same-site with the Drupal parent; job-seeker auth is session-based
end to end; bearer tokens are not used for job seekers. CORS and cache behaviour for this host are
**infrastructure-owned** — changes there require an infra change and an update to this ADR.
Corrections applied: `contracts.md §2.4` (retired) and `drupal-embed.md` (rewritten to five surfaces).
