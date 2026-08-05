# Epics — Roadmap

Four epics deliver the app/web-layer rewrite. Work them in order; within an epic, follow each
story's **Depends on** line. Full guardrails: `../../.github/copilot-instructions.md`.

## Order & dependencies

```
1. EPIC-FOUNDATION   (FND-1 … FND-8)   ── the skeleton everything builds on
        │
        ├─► 2. EPIC-SEARCH            (SRCH-1 … SRCH-10 + 7b)   needs FND-1/4/7
        │
        ├─► 3. EPIC-ACCOUNT          (ACCT-1 … ACCT-7)         needs FND-1/2/4/5/7 (+ Search filters)
        │
        └─► 4. EPIC-ADMIN-REPORTING  (ADM-1 … ADM-9)           needs FND-1/2/6
```

- **Foundation first**, in order: FND-1 (scaffold) → FND-2/3/4 → FND-5/6/7 → FND-8.
- **Search** and **Admin/Reporting** can proceed in parallel once Foundation is done; **Account**
  reuses the Search filter components (ACCT-3/4) and OpenSearch (ACCT-5).

## Epics

| # | Epic | File | Delivers |
|---|---|---|---|
| 1 | Foundation | [EPIC-FOUNDATION.md](EPIC-FOUNDATION.md) | Scaffold, models, queue, layout, auth, search foundation, CI/CD |
| 2 | Public Job Search | [EPIC-SEARCH.md](EPIC-SEARCH.md) | Search page, facets, detail (+ external descriptions), map, sitemap, Drupal API |
| 3 | Job Seeker Account | [EPIC-ACCOUNT.md](EPIC-ACCOUNT.md) | Dashboard, saved jobs, alerts (+ PHP sender), recommended, profiles, settings |
| 4 | Admin & Reporting | [EPIC-ADMIN-REPORTING.md](EPIC-ADMIN-REPORTING.md) | Filament admin (parity) + reports + chart dashboard |

## Turning a story into a ticket

- Bulk import: `../jira-import.csv` (see the CSV-import steps).
- Each ticket's **Description** carries condensed acceptance criteria + a pointer to its epic file.
  The **authoritative acceptance criteria live in the epic file** — keep the repo as the source of
  truth and let tickets reference it (avoids Jira/repo drift).
- Dependencies are noted per story; add Jira "blocks/depends-on" links after import if you want them enforced.
