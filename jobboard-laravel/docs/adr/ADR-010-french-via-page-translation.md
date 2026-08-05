# ADR-010: French via page translation (parity), not localization — for now

- **Status:** Accepted
- **Date:** 2026-07-30
- **Deciders:** Architecture owner
- **Jira:** —

## Context

WorkBC.ca has an ANGLAIS/FRANÇAIS toggle. Investigating how it works produced a surprise worth
recording, because the mechanism is not what the codebase suggests.

**How French works today.** It is the **Google Translate website widget**, translating the live DOM
client-side. Confirmed from the running site: `translate.google.com` / `translate.googleapis.com` /
`www.gstatic.com` load on the page, and the URL never changes (`/account#/dashboard`,
`/find-job/search-jobs#/job-search;` — no `/fr/` path, no server round-trip). Corroborating
artifacts visible in the translated output:

- "Maison" for Home (a human translator writes "Accueil")
- **Trail, B.C. → "Sentier"** — a real city machine-translated into "path"
- "ACCURASEE MÉCANIQUE" — an employer's registered name partly translated
- "Filter" and "By:" left in English mid-toolbar

There is **no authored French anywhere**: 0 of 113 `SystemSettings` rows contain French, the Angular
app has no translation files (only the unused `extract-i18n` scaffold), and Drupal's `language`
module is not enabled with no `fr` language entity.

**Why the embed breaks it.** The widget reaches job-board content only because the Angular app is
*inline-injected into the Drupal document*. Under [ADR-006](ADR-006-drupal-embed-iframe.md) our app
becomes a **cross-origin iframe**, which a page-level translator cannot reach. Left alone, Drupal's
chrome would translate while our search results and account area stayed English.

**The data situation.** A `jobs_fr` OpenSearch index exists with genuine French from the federal Job
Bank feed — proper titles (`cuisinier/cuisinière`), with cities and employer names correctly left
alone. But (verified against the restored dev data):

| Index | Active | Federal | External |
|---|---|---|---|
| `jobs_en` | 13,478 | 7,436 | 6,042 |
| `jobs_fr` | **7,435** | 7,435 | **0** |

The French index is *exactly* the federal feed. Every external (Innovibe) posting — **44% of active
jobs** — has no French source and never will. And `jobs_fr` is **not used today**: the capability
exists (`POST /api/Search/JobSearch/fr`) but nothing calls it, which is why the gap has never
surfaced.

## Decision

**Replicate current behaviour: load the Google Translate widget inside the embed document**, and
coordinate the toggle across the frame boundary with `postMessage` on the channel FND-4 already
introduces for iframe auto-height.

Consequently, and deliberately:

- **No `lang/fr` translation files.** The widget rewrites the whole DOM including UI labels — that is
  how "Profil du compte" and "Trier par" appear today with zero authored French. Adding a
  localization layer would duplicate what the widget already does.
- **`jobs_fr` remains dormant.** Federal jobs get machine-translated like everything else, even
  though real French exists for them.
- **Parity includes the defects.** "Sentier" and mangled employer names carry forward. This is
  accepted as the cost of matching today's behaviour.

## Consequences

- **Positive:** small scope — the widget plus one message type on an existing channel, entirely
  within FND-4. 100% job coverage, matching today. No per-character translation cost. No new
  content to author or get signed off.
- **Negative:** we carry forward known-wrong output, including a fabricated place name for a real
  B.C. city, on a government service.
- **Negative:** 7,435 genuinely French postings stay unused. Recorded here so the index is not later
  mistaken for dead data.
- **Negative:** anything outside the browser DOM stays English — most notably **ACCT-4 alert emails**,
  which a page translator can never reach. French users receive English alerts today too, so this is
  parity, not a regression, but it will not be fixed by this decision.
- **Implementation risk:** the widget walks the DOM once. **Livewire replaces DOM on update**, so
  fresh search results arrive untranslated unless translation is re-applied after each update. This
  must be built in, not discovered in review.

## Alternatives Considered

- **Hybrid — real French for federal + server-side machine translation for external + `lang/fr` UI:**
  the better end state, and deferred rather than rejected. Same coverage, but correct data (cities
  and employers excluded from translation, killing the "Sentier" class of bug), no dependency on a
  legacy Google product, translations cached so each posting is paid for once, and it reaches
  non-DOM surfaces like alert emails. Rejected **for now** on scope, and because replacing machine
  translation with authored French changes what users see and needs product sign-off.
- **`jobs_fr` only:** rejected — hides 44% of active jobs from French users. A far worse harm than
  bad translation.
- **Do nothing:** rejected — Drupal chrome would translate while our content stayed English, which is
  a visible regression against today.

## Open questions (do not block this decision)

1. **Widget lifecycle.** Google's website translator is a legacy product. It works today; its support
   status should be confirmed before investing further in it.
2. **Display vs. search.** This translates *rendered output only*. External postings are indexed in
   English, so a French query (`cuisinier`) will not match them however well the results are
   translated afterwards. Today has the identical flaw — the query already ran in English — so this
   is parity. Genuine French *search* across all jobs would need translated text **in the index**,
   which is indexer territory and out of scope under constraint #4 (ingestion/indexing).

## Compliance / Scope

Standing until revisited. French is delivered by page translation, not localization: do **not** add
`lang/fr` files, and do **not** wire `jobs_fr` into search, without superseding this ADR. Revisit if
the widget is retired, if bilingual alert emails are required, or if authored French is mandated.
Implementation lives in **FND-4**; the Drupal-side toggle change is in
[`integration/drupal-embed.md`](../integration/drupal-embed.md).
