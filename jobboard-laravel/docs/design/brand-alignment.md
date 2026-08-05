# WorkBC brand alignment

**Status:** Reference + proposed work. Companion to [ADR-006](../adr/ADR-006-drupal-embed-iframe.md)
(the search renders inside a Drupal iframe, so we own its look 100% — Drupal's theme CSS does **not**
reach into the frame).

This captures the **real** WorkBC design tokens (extracted from the live `workbc` Drupal theme) and
what it takes to make our Laravel search look native to WorkBC.ca instead of the generic Tailwind
design it has today.

> **Why this matters:** the old Angular board looked native because Drupal loaded WorkBC's
> `bootstrap.min.css` into the same page. Our iframe'd app is CSS-isolated — it will **not** inherit
> any WorkBC styling. Matching the brand is now an explicit task on our side, and this doc is the spec.

---

## 1. Source of truth

All tokens below come from `drupal/src/web/themes/custom/workbc/scss/`:

| Tokens | File |
|---|---|
| Palette, spacing, breakpoints | `abstracts/_variables_workbc.scss` |
| Fonts (`@font-face`) | `base/_fonts.scss` |
| Typography scale | `base/_typography.scss` |
| Buttons | `components/_buttons.scss`, `abstracts/_mixins_buttons.scss` |
| Form fields | `base/_forms.scss` |
| Search widget treatment | `components/_job-search.scss` |
| Links | `abstracts/_mixins_links.scss` |

The **BC Sans** webfont is bundled in the theme at `assets/fonts/BCSans/` (woff2/woff/otf/ttf + an
**OFL license** — it's the open BC Gov brand font, free to reuse). We copy those files into our app.

---

## 2. Design tokens

### Colour
| Role | Hex | Notes |
|---|---|---|
| Brand navy (dark primary) | `#002857` | Dark-theme backgrounds, primary-button hover |
| **Brand blue (secondary)** | `#2E6AB0` | **The workhorse** — links, primary buttons, input borders |
| Cyan accent (tertiary) | `#029CDD` | Accents |
| Body text | `#333333` | Default text |
| Muted text | `#666666`, `#8A8D8B` | Captions, footnotes |
| Light grey (tertiary) | `#9D9D9D` | Placeholders, "(required)" |
| Mist (light secondary) | `#f2f2f2` | Subtle backgrounds, button hover fill |
| Gold highlight | `#E7BF3E` | Trim / underline accents |
| Alert | `#791d5f` | Alert ribbon |
| Steel (search panel) | `#accad7` / hover `#8eadbb` | Search tabs & 3px borders |
| White | `#FFFFFF` | Surfaces |

Shadows: `0 0 15px 0 #00000026` (card), `0 5px 10px -5px #00000026` (bottom).

### Typography
- **Font stack:** `"BCSans", "Noto Sans", Verdana, Arial, sans-serif`
- **Base:** 16px, line-height 1.5, colour `#333333`
- **Page title:** 2.25rem → 2.8125rem (≥lg), gradient underline
- **Section title:** 1.875rem → 2.25rem, weight 700
- Weights used: 400 / 700 (BC Sans Regular + Bold)

### Spacing & layout
- Default block spacing **30px**, gutter **20px**
- Colored "trim" bars: 6px / 4px / 3px (WorkBC's signature top-accent stripes)
- **Breakpoints (bootstrap-based):** sm 600 · md 768 · lg 992 · xl 1200 · xxl 1300

### Buttons (distinctive)
- **Primary:** fill `#2E6AB0`, white text, **2px** solid `#2E6AB0` border, radius **6px**,
  padding `8px 30px`, weight **700**. Hover **inverts** → text `#2E6AB0` on `#f2f2f2`.
- **Secondary:** white fill, `#2E6AB0` text + 2px border, radius 6px. Hover → `#f2f2f2`.
- **Submit (`btn-blue`):** fill `#2E6AB0` → hover **navy** `#002857`, radius 4px, padding `10px 64px`.

### Form fields (distinctive)
- Inputs are **square** (`border-radius: 0`) with a **2px solid `#2E6AB0`** border.
- Placeholder colour `#9D9D9D`; required marker is the italic text "(required)", not an asterisk.

---

## 3. Proposed Tailwind mapping

Add a token layer to `tailwind.config.js` so views reference brand tokens, not raw hexes:

```js
theme: {
  extend: {
    colors: {
      workbc: {
        navy:  '#002857',   blue:  '#2E6AB0',   cyan:  '#029CDD',
        ink:   '#333333',   muted: '#666666',   grey:  '#9D9D9D',
        mist:  '#f2f2f2',   gold:  '#E7BF3E',   alert: '#791d5f',
        steel: '#accad7',   'steel-dark': '#8eadbb',
      },
    },
    fontFamily: {
      sans: ['BCSans', 'Noto Sans', 'Verdana', 'Arial', 'sans-serif'],
    },
    boxShadow: {
      workbc:        '0 0 15px 0 #00000026',
      'workbc-btm':  '0 5px 10px -5px #00000026',
    },
    // Optional: match WorkBC/bootstrap breakpoints for pixel-parity inside the frame
    screens: { sm: '600px', md: '768px', lg: '992px', xl: '1200px', '2xl': '1300px' },
  },
}
```

Load **BC Sans** — copy `assets/fonts/BCSans/BCSans-{Regular,Italic,Bold,BoldItalic}.{woff2,woff}`
into `resources/fonts/BCSans/` and add the four `@font-face` blocks (mirror `base/_fonts.scss`) in our
app CSS. Keep the OFL `LICENSE_OFL.txt` alongside.

Component recipes (for the FND-4 shared components):
```html
<!-- Primary button -->
<button class="inline-block rounded-[6px] border-2 border-workbc-blue bg-workbc-blue
               px-[30px] py-2 font-bold text-white transition-colors
               hover:bg-workbc-mist hover:text-workbc-blue focus:bg-workbc-mist focus:text-workbc-blue">

<!-- Text input (square, 2px blue border) -->
<input class="rounded-none border-2 border-workbc-blue px-[10px] py-2
              placeholder-workbc-grey text-workbc-ink">

<!-- Link -->
<a class="text-workbc-blue underline hover:text-workbc-navy">
```

---

## 4. Gap vs. our app today

Our search is built on **Tailwind defaults**; the deltas to WorkBC:

| Aspect | Our app today | WorkBC |
|---|---|---|
| Font | system/Inter stack | **BC Sans** |
| Primary blue | `blue-800` `#1e40af` / `blue-600` | `#2E6AB0` (+ navy `#002857`) |
| Body text | `slate-700` `#334155` | `#333333` |
| Cards/borders | `slate-200`, `rounded-lg` (8px) | steel `#accad7`, brand shadows |
| Buttons | filled `blue-*`, rounded-lg | 2px-bordered, radius 6px, **hover inverts** |
| Inputs | `rounded-md` | **square**, 2px blue border |

Nothing structural changes — it's a **re-skin**: swap tokens + component styles, layout/logic stay.

---

## 5. Scope of a brand-alignment pass (story-sized, decoupled, low-risk)

1. **Token layer** — `tailwind.config.js` extension above + BC Sans `@font-face` + copy font files.
2. **Re-skin the FND-4 shared components** (button, form field, alert, pagination) to the recipes in
   §3. Because the search/detail views should consume these, most of the change cascades from here.
3. **Sweep the search + detail Blade views** — replace ad-hoc `slate-*`/`blue-*` utilities with the
   `workbc-*` tokens / shared components (this is the bulk of the file-touch count, since we styled
   with utilities inline).
4. **Contrast / a11y check (WCAG 2.1 AA)** — verify `#2E6AB0`. White-on-`#2E6AB0` and
   `#2E6AB0`-on-white land **near the 4.5:1 threshold for small text**; where a link/label fails,
   darken to navy `#002857` (comfortably passes). Keep the a11y CI gate green.
5. **Verify inside the embed frame** — render `?embed=1`, confirm it visually matches a WorkBC page
   wrapped around it (chrome from Drupal, brand-matched search inside).

**Cost/risk:** self-contained, no backend/logic changes, fully reversible, zero Drupal coupling. The
main effort is step 3 (utility sweep) because we styled with inline utilities rather than a central
theme — a good reason to centralize into the shared components now to prevent future drift.

**Suggested tracking:** a single story (e.g. `BRAND-1 — Apply WorkBC visual identity`) or a follow-up
under FND-4. Not started — this doc is the spec.

---

## 6. Answering "same or different from localhost today?"

- **Mechanically:** whatever our app looks like is exactly what renders in the frame (iframe isolates
  CSS both ways).
- **Today (no pass):** the framed search would look like our current generic Tailwind design — a
  visible seam against WorkBC chrome.
- **After this pass:** it looks native to WorkBC.ca. Recommended before go-live if the goal is
  "looks like it belongs on the site."
