# Drupal embed integration (WorkBC.ca)

**Status:** Reference / hand-over. Decision recorded in [ADR-006](../adr/ADR-006-drupal-embed-iframe.md).
**Audience:** whoever applies the Drupal-side change (WorkBC web team) + us (app side).

This documents exactly how the current search embeds into the WorkBC Drupal site, and the precise,
contained change to point that embed at the new Laravel app via an `<iframe>`. Drupal keeps owning
the page and chrome; our app renders a chrome-less search inside the frame.

> The Drupal codebase is a separate, live-site repo (`workbcdurpal`, branch `dev2` on the machine it
> was inspected on). Nothing here has been applied to it — this is a documented, ready-to-review
> change. Apply it on a **branch**, test on **TEST**, and route it through the web team's review.
> Never against production. (It also contains a multi-hundred-MB `prod_drupal_workbc.sql` prod dump —
> leave it alone.)

---

## 1. How the embed works today

The search page is a Drupal **node** rendered in the `jobboard` view mode by the custom module
**`workbc_jobboard`**. All paths below are under
`drupal/src/web/modules/custom/workbc_jobboard/`.

| Piece | File | Role |
|---|---|---|
| Preprocess + routing | `workbc_jobboard.module` | On the `jobboard` view-mode node, injects config vars and selects the template by matching the node's path alias against `jobboard.find_job_url` / `jobboard.find_job_account_url`. |
| Search page template | `templates/node-page-jobboard-findjobs.html.twig` | Renders Drupal chrome **and** inline-loads the Angular app. |
| Account page template | `templates/node-page-jobboard-account.html.twig` | Same pattern for the logged-in seeker area. |
| Global JS library | `workbc_jobboard.libraries.yml` → `js/workbc_jobboard.js` | Attached site-wide via `hook_page_attachments_alter`. |
| Server-to-server API client | `src/Controller/WorkBcJobboardController.php` | Drupal → job-board **backend** API calls (see §7). |
| Config object | `jobboard.*` (Drupal config) | `jobboard_api_url_frontend`, `jobboard_api_url_backend`, `find_job_url`, `find_job_account_url`, `google_maps_key`, `sha`. |

The Angular app is embedded **inline** — the template drops the app's CSS + `<app-root>` + six JS
bundles + the Google Maps script straight into the Drupal page DOM
(`node-page-jobboard-findjobs.html.twig`, the `<div class="layout-container">` block, ~lines
132–143):

```twig
<div class="layout-container">
  <link rel="stylesheet" href="{{jobboard_api_url_frontend}}/bootstrap/bootstrap.min.css?v={{drupal_config('jobboard', 'sha')}}">
  <link rel="stylesheet" href="{{jobboard_api_url_frontend}}/dist/jb-search/styles.css?v={{drupal_config('jobboard', 'sha')}}">
  <app-root api="{{jobboard_api_url_frontend}}/" jbsearch="{{find_job_url}}" jbaccount="{{find_job_account_url}}" ng-version="12.2.16"></app-root>
  <script src="{{jobboard_api_url_frontend}}/dist/jb-search/runtime-es2017.js?v=..." type="module"></script>
  <script src="{{jobboard_api_url_frontend}}/dist/jb-search/runtime-es5.js?v=..." nomodule defer></script>
  <script src="{{jobboard_api_url_frontend}}/dist/jb-search/polyfills-es2017.js?v=..." type="module"></script>
  <script src="{{jobboard_api_url_frontend}}/dist/jb-search/polyfills-es5.js?v=..." nomodule defer></script>
  <script src="{{jobboard_api_url_frontend}}/dist/jb-search/main-es2017.js?v=..." type="module"></script>
  <script src="{{jobboard_api_url_frontend}}/dist/jb-search/main-es5.js?v=..." nomodule defer></script>
  <script src="https://maps.googleapis.com/maps/api/js?key={{google_maps_key}}" async defer></script>
</div>
```

Everything **outside** that `<div>` (hero, breadcrumbs, social-share, sidebar/sidenav, related
topics, page body/content) is Drupal chrome — it does not change.

---

## 2. The change: swap the block for an iframe

Replace **only** the `<div class="layout-container">…</div>` block above with:

```twig
<div class="layout-container">
  <iframe id="jobboard-search"
          src="{{ jobboard_search_url }}?embed=1"
          title="Search jobs in B.C."
          style="width:100%;border:0;min-height:60vh"
          loading="lazy"></iframe>
</div>
```

That is the whole visible change on the search page. Nothing else in the template moves.

---

## 3. Supporting Drupal-side changes (all inside `workbc_jobboard`)

**a. One config value for the iframe URL.**
Add `jobboard_search_url` (base URL of the Laravel app's search page, e.g.
`https://jobs.workbc.ca/jobs`) to the `jobboard` config, and expose it in the preprocess hook next
to the existing URL vars in `workbc_jobboard.module` → `workbc_jobboard_preprocess_node()`:

```php
$variables['jobboard_search_url'] = \Drupal::config('jobboard')->get('jobboard_search_url');
```

Also declare it in the two theme variable sets in `workbc_jobboard_theme()`
(`node__page_jobboard_findjobs` and `…_account`). *(Or reuse an existing config value and skip the
new key — but a dedicated key is clearer.)*

**b. The iframe auto-height listener (~6 lines).**
`js/workbc_jobboard.js` is already loaded site-wide, so add the listener there — no new library:

```js
// Auto-size the job-board iframe from height messages posted by the app.
window.addEventListener('message', function (e) {
  if (e.origin !== 'https://jobs.workbc.ca') return;      // trust only our app's origin
  if (e.data && e.data.type === 'jobboard:height') {
    var f = document.getElementById('jobboard-search');
    if (f) { f.style.height = e.data.height + 'px'; }
  }
});
```

(Set the origin to whatever host the app is actually served from.)

---

## 4. What drops out of Drupal (net simplification)

An iframe fully isolates the app, so Drupal stops carrying these job-board concerns:

- **`google_maps_key`** — no longer injected into the Drupal page; the app owns the map and supplies
  its own browser Maps key (SRCH-9, from our Secrets Manager). The Google Maps `<script>` line goes.
- **The `sha` cache-buster, the six bundle `<script>`s, and the `bootstrap.min.css` / `styles.css`
  `<link>`s** — gone; the Laravel/Vite build hashes and serves its own assets.
- **CSS/JS bleed** — today the job board's `bootstrap.min.css` loads into the Drupal DOM and can
  collide with theme styles. Inside an iframe it can't leak; the boundary is hard isolation.

Net: Drupal manages *less* than it does now.

---

## 5. The account / dashboard page (ACCOUNT epic, later)

`templates/node-page-jobboard-account.html.twig` is the **same pattern** (same module, routed by
`jobboard.find_job_account_url`). When the ACCOUNT epic is built, it gets the **identical** iframe
swap — point it at the app's authenticated area. Note the seeker session lives in the app, so the
frame and the app must agree on auth (cookies/SSO) — covered when that epic is scoped.

---

## 6. Our (app) side — checklist

- [ ] **Chrome-less `embed` layout mode** (FND-4): `?embed=1` (or middleware) renders search with no
      WorkBC header/footer.
- [ ] **`postMessage` height beacon** (FND-4): the embed page posts its height on load, on resize,
      and after Livewire DOM updates:
      ```js
      parent.postMessage({ type: 'jobboard:height', height: document.body.scrollHeight },
                         'https://www.workbc.ca');
      ```
- [ ] **`Content-Security-Policy: frame-ancestors https://www.workbc.ca`** on app responses (and no
      `X-Frame-Options: DENY`) so the browser permits framing (FND-4/FND-8).
- [ ] **Deep-links open the app directly** — "share this search" and email-alert links target the
      app URL (inside a frame the browser URL doesn't reflect search state).

---

## 7. API parity — endpoints Drupal calls server-to-server (MUST preserve)

Independent of the iframe, Drupal keeps calling the job-board **backend** API
(`jobboard.jobboard_api_url_backend` + path) from `WorkBcJobboardController.php`. Our SRCH-10 API
must expose these **exact paths/shapes** so swapping the backend is transparent:

| Drupal action | Method | Path (appended to backend base URL) | Used by |
|---|---|---|---|
| `SearchPost` | POST (JSON body) | `api/Search/JobSearch` | Recent-jobs / search widgets |
| `getTotalJobs` | GET | `api/Search/gettotaljobs` | `hook_cron` → `state('jobboard_total_jobs')` (the "Search N jobs" count) |
| `getCities` | GET | `api/location/cities/{cityname}/true` | `/api/getCities` autocomplete (`getCitiesJson`) |
| `saveProfile` | POST | `api/career-profiles/save/{profile_id}` | Save career profile |
| `statusProfile` | GET | `api/career-profiles/status/{profile_id}` | Career profile status |
| `saveIndustryProfile` | POST | `api/industry-profiles/save/{profile_id}` | Save industry profile |
| `statusIndustryProfile` | GET | `api/industry-profiles/status/{profile_id}` | Industry profile status |

Notes:
- `getCities` is **path-param** style (`.../cities/{name}/true`), not a query string — the second
  segment (`true`) is a flag. Match this in `LocationApiController`.
- Paths use `api/Search/...` casing as sent by Drupal; confirm our routes match case-for-case (or
  add case-tolerant aliases) during the parity pass.
- Constants live in `WorkBcJobboardController.php` (`SEARCH_POST`, `GETTOTAL_JOBS`, `GET_CITIES`,
  `SAVE_CAREER_PROFILE`, `STATUS_CAREER_PROFILE`, `SAVE_INDUSTRY_PROFILE`,
  `STATUS_INDUSTRY_PROFILE`).

There is also a Drupal-owned route `GET /api/getCities` (in `workbc_jobboard.routing.yml`) that
proxies to the backend `getCities` — that stays in Drupal; we only need to keep the **backend**
endpoint it calls.

---

## 8. Rollout & testing

1. App is deployed and reachable at a URL the browser can frame (subdomain, e.g. `jobs.workbc.ca`,
   or a proxied path); `frame-ancestors` header set.
2. Point `jobboard.jobboard_search_url` at the **TEST** app; apply the template + JS + config change
   on a Drupal **branch**; verify the framed search on the TEST WorkBC page (height auto-size,
   facets, map, detail links, deep-link/alert behaviour).
3. Confirm the backend API parity (§7) so recent-jobs widgets and the total-jobs count still work.
4. Promote: point `jobboard_search_url` at the prod app. **Rollback** = revert the template block to
   the Angular embed (or repoint the config), so it's low-risk and reversible.

---

## 9. File reference (Drupal repo)

- `drupal/src/web/modules/custom/workbc_jobboard/templates/node-page-jobboard-findjobs.html.twig` — the block to swap
- `drupal/src/web/modules/custom/workbc_jobboard/templates/node-page-jobboard-account.html.twig` — same, for ACCOUNT epic
- `drupal/src/web/modules/custom/workbc_jobboard/workbc_jobboard.module` — preprocess/theme vars (add `jobboard_search_url`)
- `drupal/src/web/modules/custom/workbc_jobboard/js/workbc_jobboard.js` — add the height listener
- `drupal/src/web/modules/custom/workbc_jobboard/workbc_jobboard.libraries.yml` — library (already attaches the JS)
- `drupal/src/web/modules/custom/workbc_jobboard/src/Controller/WorkBcJobboardController.php` — backend API endpoints to preserve (§7)
- `drupal/src/web/modules/custom/workbc_jobboard/workbc_jobboard.routing.yml` — Drupal `/api/getCities` proxy route
