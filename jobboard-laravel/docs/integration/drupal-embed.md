# Drupal embed integration (WorkBC.ca)

**Status:** Reference / hand-over. Decisions in [ADR-006](../adr/ADR-006-drupal-embed-iframe.md)
(iframe) and [ADR-009](../adr/ADR-009-same-site-session-auth-for-embed.md) (same-site session auth).
**Audience:** whoever applies the Drupal-side change + us (app side).

This documents how the job board integrates with WorkBC.ca today, and the change to point it at the
Laravel app. Everything below was verified against the live Drupal module and production
infrastructure on 2026-07-29 — it supersedes an earlier version of this doc that described the
profile-save endpoints as server-to-server (they are not) and cited untracked file paths.

> The Drupal codebase is a separate, live-site repo: **`C:\workbcdurpal`** (note the spelling),
> branch `dev2`. **The git-tracked module is `src/web/modules/custom/workbc_jobboard/`.** There is a
> second, **untracked** copy at `drupal/src/web/...` that differs — do not edit that one. Apply
> changes on a **branch**, test on **TEST**, route through the web team's review. Never against
> production. (The repo also contains multi-hundred-MB prod SQL dumps — leave them alone.)

---

## 0. Origin map (confirmed 2026-08-04, DNS-verified)

| Env | Drupal parent | App (job seeker) | Admin (Filament) |
|---|---|---|---|
| **prod** | `www.workbc.ca` | `api-jobboard.workbc.ca` | `admin-jobboard.workbc.ca` |
| **test** | `test.workbc.ca` | `test-api-jobboard.workbc.ca` | `test-admin-jobboard.workbc.ca` |
| **dev** | `dev2.workbc.ca` | `workbc-jb2.a55eb5-dev.stratus.cloud.gov.bc.ca` ⚠️ | `dev2-admin-jobboard.workbc.ca` |

> ⚠️ **dev is NOT `dev2-api-jobboard.workbc.ca`** — corrected 2026-08-05. That hostname **does not
> resolve** (`ERR_NAME_NOT_RESOLVED` in-browser), despite an earlier row here claiming it was
> DNS-verified. Note that WSL `nslookup` returns a CloudFront CNAME for it, which is misleading;
> trust the browser. Ground truth is view-source on
> `dev2.workbc.ca/search-and-prepare-job/find-jobs`, which shows
> `<app-root api="https://workbc-jb2.a55eb5-dev.stratus.cloud.gov.bc.ca/">`.
>
> Two consequences. **(a)** The k8s deployment `jb2` is dev2's *live* job-board backend, not a spare
> environment — replacing its `web` container replaces dev2's job board. **(b)** dev is the one
> environment that is **cross-site**: everywhere else the app sits under `workbc.ca` alongside the
> Drupal parent, but the Stratus hostname does not. So on dev2, ADR-009 session auth will not work
> and the referrer-restricted Google Maps key is rejected. Framing does not help — an iframe sends
> its own origin as the referrer. Only a real `workbc.ca` hostname for `jb2` fixes either.

Each job-board host is its own CloudFront distribution, and **each Drupal environment currently
points at its matching Stratus origin** (`a55eb5-{prod,test,dev}`) — so the repoint in §7 is a
per-environment change, and TEST can be exercised end to end before prod.

Everything is same-site under `workbc.ca`, so ADR-009's session model holds in all three
environments. The values FND-4 needs for `frame-ancestors` are the Drupal-parent column above.

---

## 1. Verified infrastructure facts

| Fact | Value |
|---|---|
| App hostnames (same backend) | `api-jobboard.workbc.ca` (CloudFront `E3D5JEAK4A4WX7`) → origin `workbc-jb.a55eb5-prod.stratus.cloud.gov.bc.ca` |
| Prod Drupal currently points at | the **Stratus** hostname (cross-site with `www.workbc.ca`) |
| **Required target** | `api-jobboard.workbc.ca` — **same-site**, which is what makes session auth work (ADR-009) |
| Cookies → origin | `Cookies.Forward: all` (also in the cache key) |
| Cached methods | `HEAD, GET` only — Livewire POSTs never cached |
| `DefaultTTL` | `86400`, no path behaviours → responses without explicit `Cache-Control` cache for 24h |
| CORS (`cors-api-jobboard`) | origins `https://www.workbc.ca`, `https://workbc.ca`; **`AllowCredentials: true`**; `OriginOverride: true` |
| Framing | **not blocked** — no `X-Frame-Options`, no CSP at the CDN |
| Origin custom header | `X-Forwarded-Host: api-jobboard.workbc.ca` |
| Sibling hosts | `admin-jobboard.workbc.ca` exists; `jobs.workbc.ca` unregistered |

**CORS is CDN-owned.** `OriginOverride: true` means CloudFront's headers override the app's, so
`config/cors.php` has no effect here. CORS changes are an infra change, not a code deploy.

---

## 2. How it works today

The job board UI is **inline-injected** into the Drupal page DOM — not an iframe, not server-rendered
from API data. Two separate Angular builds, in two node templates rendered by the `workbc_jobboard`
module:

```twig
<link rel="stylesheet" href="{{jobboard_api_url_frontend}}/dist/jb-search/styles.css?v=...">
<app-root api="{{jobboard_api_url_frontend}}/" jbsearch="{{find_job_url}}" jbaccount="{{find_job_account_url}}"></app-root>
<script src="{{jobboard_api_url_frontend}}/dist/jb-search/main-es2017.js?v=..."></script>
{# …6 bundles total, plus the Google Maps script on the search page #}
```

| Piece | File | Role |
|---|---|---|
| Preprocess + template routing | `workbc_jobboard.module` | Injects config vars; picks the template by matching the node's path alias against `jobboard.find_job_url` / `find_job_account_url` |
| Search page | `templates/node-page-jobboard-findjobs.html.twig` | Drupal chrome + inline `dist/jb-search` |
| Account page (owns **login**) | `templates/node-page-jobboard-account.html.twig` | Drupal chrome + inline `dist/jb-account` |
| Save-profile button | `src/Plugin/Block/WorkbcJobboardSaveProfile.php` | Themed block; pushes status/save URLs to **`drupalSettings`** (browser-side) |
| Site-wide JS | `js/workbc_jobboard.js` | Save-profile calls, header login state, logout |
| Server-to-server client | `src/Controller/WorkBcJobboardController.php` | Drupal PHP → backend API (§6) |
| Recent Jobs / count | `src/Plugin/Block/WorkbcJobboardSidebar.php`, `workbc_jobboard.module` | Consume the server-to-server API |

**Because the app is injected rather than framed, it runs on the `www.workbc.ca` origin.** That shared
origin is what makes the current auth work: the JWT cookie Angular sets is readable by Drupal's own JS.

```js
// js/workbc_jobboard.js — save-profile button, today
const token = readCookie('currentUser.token');
if (token) { $.ajax({ url: settings.jobboard.status, headers: { 'Authorization': `Bearer ${token}` }, … }) }
```

The same cookies drive the **header nav** (`currentUser.username` toggles logged-in menu items) and
`navLogout()` (clears six `currentUser.*` cookies).

> **Not used:** `WorkBcJobboardController::getCallOptions()` has
> `saveProfile`/`statusProfile`/`saveIndustryProfile`/`statusIndustryProfile` branches, including
> `Authorization` forwarding. **Nothing calls them** — dead code, superseded by the browser-side JS
> above. It is the source of the retracted "Drupal forwards the Authorization header" claim.

---

## 3. Target state — five surfaces

They do **not** all get the same treatment.

| # | Surface | Mechanism | Rationale |
|---|---|---|---|
| 1 | Find Jobs page | **iframe** → `/jobs?embed=1` | Drupal keeps hero/breadcrumbs/sidebar; we render the body |
| 2 | Account area (login, dashboard, saved jobs, alerts) | **full-page navigation** to the app | **Not** an iframe — password managers are unreliable in cross-origin frames and users cannot verify the URL they type credentials into (ADR-009) |
| 3 | Career / Industry profile pages | **native Drupal button + credentialed fetch** | It is one button on a Drupal content page; keeps native theming, keeps pages cacheable |
| 4 | Site header login state + logout | **credentialed fetch** to a session endpoint | Drupal can no longer read a same-origin cookie |
| 5 | Recent Jobs block, sitewide count, city autocomplete | **unchanged** server-to-server API | Already built and tested (§6) |

Livewire cannot be shipped as a bundle injected into another site's DOM (it renders server-side HTML
bound to its own session and CSRF), so the inline-embed pattern ends regardless of preference.

### Surface 1 — search iframe

Replace **only** the `<div class="layout-container">…</div>` block in
`templates/node-page-jobboard-findjobs.html.twig`:

```twig
<div class="layout-container">
  <iframe id="jobboard-search"
          src="{{ jobboard_search_url }}?embed=1"
          title="Search jobs in B.C."
          style="width:100%;border:0;min-height:60vh"
          loading="lazy"></iframe>
</div>
```

Add the height listener to `js/workbc_jobboard.js` (already loaded site-wide — no new library):

```js
window.addEventListener('message', function (e) {
  if (e.origin !== 'https://api-jobboard.workbc.ca') return;   // trust only our origin
  if (e.data && e.data.type === 'jobboard:height') {
    var f = document.getElementById('jobboard-search');
    if (f) { f.style.height = e.data.height + 'px'; }
  }
});
```

**The FRANÇAIS toggle must also notify the frame** (`ADR-010`). Today's toggle is the **Google
Translate widget** rewriting the DOM; it reaches job-board content only because the current app is
inline-injected into this document. A page translator **cannot** reach into a cross-origin iframe, so
without this the Drupal chrome would translate while the framed search stayed English:

```js
// wherever the ANGLAIS/FRANÇAIS toggle is handled
var f = document.getElementById('jobboard-search');
if (f) {
  f.contentWindow.postMessage(
    { type: 'jobboard:lang', lang: /* 'fr' | 'en' */ },
    'https://api-jobboard.workbc.ca'
  );
}
```

Our side runs the widget inside the frame and re-applies it after Livewire updates (FND-4).

### Surface 2 — account area

Point `jobboard.find_job_account_url` at the app and let the menu/CTAs navigate there directly, rather
than rendering a Drupal node with an embedded app. The app supplies its own WorkBC chrome (BRAND-1).

> **Hostname note:** users will see this host in the address bar while entering credentials.
> `api-jobboard.workbc.ca` is a poor trust signal for a login page; a user-facing name
> (`jobs.workbc.ca`, unregistered) is preferable, with `api-jobboard` retained for §6. Same site
> either way, so nothing in the cookie analysis changes.

### Surface 3 — save career / industry profile

`WorkbcJobboardSaveProfile.php` needs no structural change (it is config-driven). The change is in
`js/workbc_jobboard.js`:

| Today | Target |
|---|---|
| `readCookie('currentUser.token')`, branch on token presence | delete — no token exists |
| `headers: { Authorization: Bearer … }` | delete; add `xhrFields: { withCredentials: true }` |
| logged-out detected *before* calling | detect via **401** from the status call |
| `window.location.href = '/account#/login'` | the app's login URL |

**CSRF.** The `XSRF-TOKEN` cookie → `X-XSRF-TOKEN` header double-submit pattern **cannot** work
cross-origin — Drupal's JS cannot read a cookie belonging to our host. Instead the **status response
carries the token** (`{ saved: false, csrf: "…" }`) and the save POST sends it back; no extra
round-trip, since status already fires on page load. The header must also be added to
`AccessControlAllowHeaders` in the CloudFront policy, which currently lacks it.

**Save-then-login replay.** Today the profile id is stashed in `localStorage`
(`tmpSavedCareerProfile`) and replayed by the Angular app after login. Keep the whole stash-and-replay
in Drupal's JS — on page load, if the stash exists and the session endpoint reports logged-in, fire
the save and clear it. That keeps the flow on one origin instead of splitting it across the boundary.

### Surface 4 — header login state and logout

```js
// today
const currentUser = readCookie('currentUser.username');
if (currentUser != '') { /* show logged-in menu */ }
$('#menu-item-logout').on('click', navLogout);  // clears 6 currentUser.* cookies
```

Both break: the cookies will belong to our origin, not Drupal's. Replace with a credentialed fetch to
the app's session endpoint for menu state, and route logout through the app. **This was not in scope
in any prior version of this doc** — ADR-006 §5 only noted "the frame and the app must agree on auth."

---

## 4. What drops out of Drupal

- **`google_maps_key`** — no longer injected; the app owns the map and its own browser key (SRCH-9)
- **The `sha` cache-buster, six bundle `<script>`s, `bootstrap.min.css` / `styles.css` `<link>`s** — the
  Laravel/Vite build hashes and serves its own assets
- **CSS/JS bleed** — today the board's `bootstrap.min.css` loads into the Drupal DOM and can collide
  with theme styles; inside an iframe it cannot leak
- **The four dead profile branches** in `WorkBcJobboardController.php`
- **All `currentUser.*` token handling** in `js/workbc_jobboard.js`

Net: Drupal manages *less* than it does now.

---

## 5. Our (app) side — checklist

- [x] **Trusted proxies** — `X-Forwarded-Host`/`-Proto` honoured, else canonical/sitemap/redirect URLs
      leak the internal origin hostname. `bootstrap/app.php`; test `tests/Feature/TrustedProxiesTest.php`
- [ ] **FND-4 embed layout** — `?embed=1` renders search with no WorkBC header/footer
- [ ] **FND-4 height beacon** — post on load, on resize, and after Livewire DOM updates:
      ```js
      parent.postMessage({ type: 'jobboard:height', height: document.body.scrollHeight },
                         'https://www.workbc.ca');
      ```
- [ ] **`Content-Security-Policy: frame-ancestors https://www.workbc.ca https://workbc.ca`** (and no
      `X-Frame-Options: DENY`). Nothing at the CDN blocks framing, so this is ours to set
- [ ] **Session endpoint** for surface 4 (menu state) — returns auth state, no PII beyond display name
- [ ] **Session-authenticated profile routes** (ACCT-6) returning **401** when anonymous — replacing
      `EnsureJobSeekerToken`; note these must carry **session middleware** (the `api` group has none)
- [ ] **Explicit `no-store, private`** on any route outside the session-bearing `web` group
      (`DefaultTTL` is 24h with no path carve-outs)
- [ ] **Prod env:** `SESSION_SECURE_COOKIE=true`, correct `APP_URL`, `TRUSTED_PROXIES`
- [ ] **`TrustHosts`** in production (`^(.+\.)?workbc\.ca$`) — the origin is directly reachable, so
      `X-Forwarded-Host` is otherwise spoofable (ADR-009 "Host spoofing")

---

## 6. API parity — the three genuine server-to-server endpoints

Independent of the embed, Drupal PHP calls these for its **own** widgets. Our SRCH-10 API preserves
the exact paths/shapes, so swapping the backend is transparent.

| Drupal action | Method | Path (appended to `jobboard_api_url_backend`) | Powers |
|---|---|---|---|
| `SearchPost` | POST (JSON) | `api/Search/JobSearch` | Recent Jobs sidebar block |
| `getTotalJobs` | GET | `api/Search/gettotaljobs` | `hook_cron` → `state('jobboard_total_jobs')`, the sitewide "Search N jobs" count |
| `getCities` | GET | `api/location/cities/{cityname}/true` | Drupal's own `/api/getCities` route → autocomplete |

Notes:
- `getCities` is **path-param** style, not a query string; the trailing segment is a flag.
- Casing (`api/Search/…`) is sent literally by Drupal — our routes match case-for-case.
- `jbTestConnection()` health-checks the API and hides the save button when it is down.
- **Not server-to-server:** the career/industry profile endpoints (see §2 note). `contracts.md §2.4`
  is retired as a server-to-server contract.
- `GET api/career-profiles/topjobs/{noc}` is dead — never called by Drupal. Not implemented.

Status of our implementation: see [`api-status.md`](api-status.md).

---

## 7. Rollout & testing

1. App deployed and reachable at a **`*.workbc.ca`** hostname (ADR-009); `frame-ancestors` set;
   `X-XSRF-TOKEN` added to the CloudFront CORS policy.
2. Repoint Drupal config at the **TEST** app; apply the template + JS changes on a Drupal **branch**;
   verify on TEST: framed search + auto-height, account navigation + login, save-profile button
   (logged in *and* anonymous → login → replay), header menu state, logout, Recent Jobs, sitewide count.
3. Confirm §6 parity so the widgets and count still work.
4. Promote.

> **Code vs config.** The template/JS edits are branchable and reviewable. The URL values
> (`jobboard_api_url_frontend`/`_backend`, `find_job_url`, `find_job_account_url`) live in Drupal's
> **active config (database)** per environment — they are **not** in `config/sync` or version control.
> So repointing is a live config change with **no git trail**, and reverting the branch does **not**
> restore the old URLs. Roll back config separately (`drush config:set`, or the admin UI).

---

## 8. File reference (Drupal repo — git-tracked copy)

Root: `C:\workbcdurpal\src\web\modules\custom\workbc_jobboard\`

- `templates/node-page-jobboard-findjobs.html.twig` — surface 1, the block to swap
- `templates/node-page-jobboard-account.html.twig` — surface 2
- `src/Plugin/Block/WorkbcJobboardSaveProfile.php` — surface 3 block (config-driven; likely unchanged)
- `js/workbc_jobboard.js` — surfaces 1 (height), 3 (save), 4 (header/logout)
- `workbc_jobboard.module` — preprocess/theme vars; add the iframe/account URL vars
- `src/Controller/WorkBcJobboardController.php` — §6 endpoints; delete the 4 dead profile branches
- `src/Plugin/Block/WorkbcJobboardSidebar.php` — Recent Jobs block (§6 consumer)
- `workbc_jobboard.routing.yml` — Drupal's `/api/getCities` proxy route
