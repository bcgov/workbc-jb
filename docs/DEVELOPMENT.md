# Local Development

How to run, develop, and test the WorkBC Job Board **locally**, against a **copy** of the real
data. Read this with `.github/copilot-instructions.md` (the enforced constraints) and
`docs/architecture.md`.

> **Never develop against production.** Point every connection at the local Docker stack or a
> dedicated TEST environment. The importers and the OpenSearch indexer are **existing PHP
> containers** and are **not** part of this repo — locally you load their *output* (DB rows +
> index docs), you do not run them here. See `architecture.md §1a`.

---

## 1. The local stack (Docker)

Everything runs in Docker via `compose.yaml` (Laravel Sail base + an added OpenSearch service).
No PHP, Composer, or Node is required on the host.

| Service        | Image                             | Host port | Purpose                                  |
|----------------|-----------------------------------|-----------|------------------------------------------|
| `laravel.test` | `sail-8.3/app` (PHP **8.3**)      | **8000**  | The app (Blade + Livewire + Filament)    |
| `pgsql`        | `postgres:15-alpine`              | **5432**  | The existing database (your data dump)   |
| `redis`        | `redis:alpine`                    | **6380**  | Cache · session · queue                  |
| `opensearch`   | `opensearchproject/opensearch:2.11.1` | **9200** | Derived search read model (index docs)   |

App is at <http://localhost:8000>. Host ports avoid conflicts on this machine: app is **8000**
(port 80 is reserved on Windows) and redis is **6380** (6379 is taken by the Drupal stack). These
are set via `APP_PORT` / `FORWARD_REDIS_PORT` in `.env`; the app talks to services by their
in-network names (`pgsql`, `redis:6379`, `opensearch:9200`) regardless.

Versions are pinned to match the deployment target (PHP 8.3, PostgreSQL 15) so local behaviour
matches production — see `docs/adr/ADR-005`. OpenSearch runs single-node with the security plugin
disabled locally (`http://localhost:9200`, no auth); production uses TLS + auth via env.

### Why `vendor` / `storage` are on Docker volumes (Windows performance)

The source tree is bind-mounted from Windows (`.:/var/www/html`), but three heavy, non-edited
trees — **`vendor`, `storage`, `bootstrap/cache`** — are on **Docker-managed named volumes**
(`compose.yaml`). Reading `vendor`'s ~10k files over the Windows→Linux mount on every request made
pages take **14–32s**; moving them to native volumes drops that to **<1s**. It also fixes those
dirs being root-owned on the Windows mount. Consequence: after a `docker compose down -v` (which
wipes volumes) you must **re-populate** them — see §3.

---

## 2. Prerequisites

- **Docker Desktop** running (WSL2 backend recommended on Windows 11).
- A shell: **PowerShell** works for the `docker compose` commands below; **Git Bash / WSL** is
  needed only if you want the `sail` helper script.

---

## 3. First run

```powershell
# from the repo root
copy .env.example .env                 # first time only (PowerShell: copy; bash: cp)
docker compose up -d --build           # build the PHP 8.3 image + start all four services

# Populate the named volumes (empty on first create / after `down -v`):
docker compose exec -u root laravel.test sh -c "mkdir -p storage/framework/{cache/data,sessions,views,testing} storage/logs storage/app/{public,private} && chown -R sail:sail vendor storage bootstrap/cache"
docker compose exec -u sail  laravel.test composer install
docker compose exec         laravel.test php artisan filament:assets   # publish admin CSS/JS (gitignored)
docker compose exec         laravel.test npm install
docker compose exec         laravel.test npm run build      # builds public/build (Vite manifest)
docker compose exec         laravel.test php artisan key:generate
```

`vendor`, `storage`, and `bootstrap/cache` live on Docker volumes (see §1), so on a **fresh**
create — or after `docker compose down -v` — they start empty and the block above repopulates
them (skeleton + ownership, then `composer install`, then the Vite build). Without the Vite build
the homepage 500s with *"Vite manifest not found"*.

Sail sets `WWWUSER`/`WWWGROUP` for you; when calling `docker compose` directly on Windows they
default fine (also set in `.env`). If the app image build complains about them, prefix:
`WWWUSER=1000 WWWGROUP=1000 docker compose up -d --build` (Git Bash).

Check it's up: <http://localhost:8000> (app), <http://localhost:8000/admin> (Filament), and
<http://localhost:9200> (OpenSearch).

### A `sail` shortcut (optional)

The commands below use `docker compose exec laravel.test …`. If you use Git Bash/WSL you can
alias the shorter Sail form:

```bash
alias sail='./vendor/bin/sail'
sail up -d          # same as docker compose up -d
sail artisan test   # same as docker compose exec laravel.test php artisan test
```

PowerShell users: just use the `docker compose exec laravel.test …` form throughout.

---

## 4. Loading your data (you do this, from a TEST/local copy)

The app is useless without data because it **maps the existing schema** — it never creates it.
Load a **copy** of the real data into the two stores:

### PostgreSQL (the source of truth)

```powershell
# restore a dump into the pgsql container's database (DB_DATABASE, default: laravel)
# custom-format dump:
type your_dump.dump | docker compose exec -T pgsql pg_restore -U sail -d laravel --no-owner
# or a plain SQL dump:
type your_dump.sql  | docker compose exec -T pgsql psql -U sail -d laravel
```

- Restore into the database named by `DB_DATABASE` in `.env` (default `laravel`), or set
  `DB_DATABASE` to your dump's database name.
- `--no-owner` avoids role-ownership errors from the source cluster.
- The app **must not** run Laravel migrations against existing tables. `php artisan migrate`
  only manages Laravel's *own* `migrations` table — never the PascalCase business tables or
  `__EFMigrationsHistory` (`data-model.md §0`).

### OpenSearch (the derived read model)

Local OpenSearch starts **empty**. Populate `jobs_en` / `jobs_fr` from a copy:

- **Snapshot restore** of a TEST index, **or**
- **Reindex/replay** using the existing PHP indexer pointed at your local cluster (run that
  container separately — it is not in this repo), **or**
- a `_bulk` load of exported documents.

Verify: `curl http://localhost:9200/_cat/indices?v` should list `jobs_en` and `jobs_fr`.
Remember **Rule B** — the app never recomputes derived fields; it reads what the indexer wrote
(`architecture.md`, and the `ExpireDate` drift note in `docs/glossary.md`).

---

## 5. Everyday commands

```powershell
docker compose exec laravel.test php artisan test          # run the test suite
docker compose exec laravel.test php artisan about         # env/driver summary
docker compose exec laravel.test php artisan tinker        # REPL
docker compose exec laravel.test php artisan queue:work     # process the app's own queues
docker compose exec laravel.test composer require <pkg>     # add a dependency (resolves for 8.3)
docker compose logs -f laravel.test                        # tail app logs
docker compose down                                        # stop (add -v to wipe data volumes)
```

Front-end assets (Vite/Tailwind) build inside the container; `docker compose exec laravel.test npm run dev`
for HMR on port 5173.

---

## 6. Testing before you push (the "local PR" workflow)

There is no separate remote needed to review your own work locally:

1. **Branch:** `git switch -c JOBS-123-short-title` (one ticket per branch — see copilot-instructions).
2. **Build + test locally:** `docker compose exec laravel.test php artisan test` must be green;
   run the a11y check where relevant.
3. **Self-review the diff:** `git add -p` and `git diff --staged` — this *is* your local PR;
   walk the diff against the story's acceptance criteria and the DoD self-check.
4. **Commit:** small, focused commits referencing the ticket.
5. **Push only when tested:** push the branch and open the PR on the remote when the story is
   done and green — not before.

To literally preview a PR before a remote exists, you can diff branches
(`git diff main...JOBS-123`) or use `gh pr create` once the repo has a remote.

---

## 7. Troubleshooting

| Symptom | Fix |
|---|---|
| `port is already allocated` | App uses **8000**, redis **6380** here to avoid Windows/Drupal conflicts. Stop the conflicting service or change `APP_PORT` / `FORWARD_DB_PORT` / `FORWARD_REDIS_PORT` / `FORWARD_OPENSEARCH_PORT` in `.env`. |
| Homepage/admin **500** with *"Vite manifest not found"* | Assets aren't built: `docker compose exec laravel.test npm run build`. |
| **500** *"tempnam(): file created in the system's temporary directory"* | `storage` / `bootstrap/cache` not writable by uid 1000. `docker compose exec -u root laravel.test chown -R sail:sail storage bootstrap/cache`. |
| Every request takes 10–30s | The heavy trees aren't on volumes (or the volumes were wiped) — see §1/§3. Confirm `compose.yaml` has the `sail-vendor`/`sail-storage`/`sail-bootstrap-cache` mounts and repopulate them. |
| After `docker compose down -v` the app 500s / autoload fails | `down -v` wiped the named volumes. Re-run the **populate** block in §3 (`composer install`, skeleton + chown, `npm run build`). |
| OpenSearch container exits / `max virtual memory` | Increase Docker Desktop memory; ensure `vm.max_map_count` in the WSL2 VM is ≥ 262144. |
| `SQLSTATE… database "laravel" does not exist` | Create it, or point `DB_DATABASE` at your restored DB name; then restart the app container. |
| App can't reach services | Inside the compose network, hosts are the **service names** (`pgsql`, `redis`, `opensearch`) — not `localhost`. `.env` is already set this way. |
| Stale config after `.env` edit | `docker compose exec laravel.test php artisan config:clear`. |
