# OpenSearch — the derived search read model

The two indexes the app **reads** (never writes — Rule B, ADR-001). Populated by the existing PHP
indexer container (out of scope here).

| Index | Role |
|---|---|
| `jobs_en` | English — the `DefaultIndex`; used for every request except French |
| `jobs_fr` | French — used when the request `language == "fr"` (`General.FrenchIndex`) |

Bilingual **split of the same jobs**, not different jobs: a job with French content (federal / Job
Bank) appears in both, `jobs_fr` carrying French title/description + French text analysis. Selection
is purely by language (see `WorkBC.Web/Controllers/SearchController.cs`).

## Files here

- **`jobs-index.json`** — the real `jobs_en` mapping (captured from the live cluster) wrapped as a
  `PUT /{index}` create body. Use it to stand up a **local** index for dev/fake data:
  ```bash
  curl -XPUT localhost:9200/jobs_en -H 'Content-Type: application/json' --data-binary @docs/opensearch/jobs-index.json
  curl -XPUT localhost:9200/jobs_fr -H 'Content-Type: application/json' --data-binary @docs/opensearch/jobs-index.json
  ```
  > Mapping **and** `settings.analysis` are captured from the live cluster. Two local tweaks vs.
  > prod: `number_of_replicas: 0` (single-node → stays green; prod uses 1) and `number_of_shards: 1`.
  > `max_result_window: 50000` matches prod. For a **snapshot restore** you don't need this file at
  > all — settings + mapping come with the snapshot.
  >
  > **`jobs_fr` note:** this file is the **English** index. `jobs_fr` almost certainly uses a
  > **French** `default` analyzer (french stemmer/stopwords). Using this file for `jobs_fr` locally
  > is fine for dev/fake data, but grab `GET /jobs_fr/_settings` + `/_mapping` for true FR parity.

## Text analysis (search matching)

From the real settings — matters for the keyword parser and match parity:

- **`default` analyzer** (applied to every `text` field with no explicit analyzer) is a full English
  pipeline: `asciifolding → english_possessive_stemmer → lowercase → english_stop → english_stemmer
  → synonym` (standard tokenizer). So loose full-text queries are **stemmed, stop-worded, and
  synonym-expanded**.
- **Synonyms** are a small curated list: `janitor⇄custodian`, `hairdresser⇄hairstylist`. A search
  for one matches the other — reproduce this list in the rewrite.
- **`english_exact`** (the `.exact` sub-fields) = `standard` tokenizer + `lowercase` **only** — no
  stemming/stopwords. Used for **exact / quoted-phrase** matching (SRCH-1 keyword parser: quoted =
  exact via `.exact`; loose terms hit the stemmed `default`).
- **`lowercase_normalizer`** (the `.normalize` keyword sub-fields) = `lowercase + asciifolding` —
  case/accent-insensitive exact-value matching (e.g. `City.normalize`).

## Field notes for the query layer (FND-7 / SEARCH)

Fields the current search actually queries/sorts on — cross-check against `contracts.md §2.1`:

- **Base filter / dates:** `ExpireDate`, `DatePosted`, `LastUpdated`, `StartDate` (all `date`).
- **Text search:** `Title` / `EmployerName` / `NocJobTitle` / `City` each have `.exact`
  (`english_exact`) and `.normalize` (`lowercase_normalizer`) sub-fields; `JobDescription`,
  `AllSkills` have `.exact`. Keyword parser targets these (SRCH-1).
- **Salary:** `Salary` (`float`), `SalarySort.Ascending` / `SalarySort.Descending` (`float`, the
  unknown-salary `<= -99999999` sentinel), `SalaryConditions.Description.keyword`.
- **Facets:** `EduLevel.keyword`, `NaicsId` (`integer`), `Noc` (`long`) / `Noc2021` (`float`),
  `WorkplaceType.Id` (`long`; virtual = **15141**), `EmployerTypeId` (`long`; placement agency = 1),
  the `Is*` booleans (equity groups + `IsFederalJob`).
- **Location / map:** `LocationGeo` (`geo_point`, for `geo_distance` + radius/sort);
  `Location.Lat` / `Location.Lon` are **`text`** (display only); `City.normalize`, `PostalCode`, `Region`, `Province`.
- **External jobs (SRCH-7b):** `ExternalSource` is **`nested`** → `Source.Source` + `Source.Url`
  (query `ExternalSource.Source.Source.keyword`). `JobDescription` holds the on-site description.
- **Skills:** `SkillCategories` is **`nested`**; its `Skills` `copy_to` the flattened `AllSkills`.

`Lang` records the document language. Object (non-nested) wrappers: `EmploymentTerms`,
`HoursOfWork`, `PeriodOfEmployment`, `WorkLangCd`, `WorkplaceType`, `SalaryConditions` — each a
`{ Description }` (WorkplaceType also has `Id`).
