# Linux setup

Running Job Board locally using Docker Compose.

## Initial setup
- Install [`elasticsearch-dump`](https://github.com/elasticsearch-dump/elasticsearch-dump)
- Build the app and run the database for the initial import:
```bash
cd src
alias docker-compose-jb='docker-compose -f docker-compose.yml -f docker-compose.linux-dev.yml'
docker-compose-jb build
docker-compose-jb run postgres
```
- In a separate window, import an initial snapshot of the `jobboard` database because it cannot be rebuilt from original migrations (which were written for MSSQL Server)
```bash
gunzip -k -c scripts/jobboard-full.sql.gz | docker-compose-jb exec -T postgres psql --username workbc jobboard
```
- Stop the running container (Ctrl-C), then start the full app:
```bash
docker-compose-jb up
```
- Allow for ~30 minutes to repopulate the full database, including jobs import, until the Docker logs settle.
- Run the full indexing job manually:
```bash
docker-compose-jb exec dotnet-cli
cd /app/workbc-indexers-wanted
dotnet WorkBC.Indexers.Wanted.dll -r
cd /app/workbc-indexers-federal
dotnet WorkBC.Indexers.Federal.dll # without -r to avoid resetting the index
```

## Backup/restore database and index
- Take a full database snapshot:
```bash
docker-compose-jb exec -T postgres pg_dump --clean --username workbc jobboard | gzip > scripts/jobboard-full.sql.gz
```
- Restore a full database snapshot:
```bash
docker-compose-jb exec -T postgres psql -U workbc jobboard < scripts/jobboard-reset.sql \
&& gunzip -k -c scripts/jobboard-full.sql.gz | docker-compose-jb exec -T postgres psql --username workbc jobboard
```
- Take a full index snapshot:
```bash
elasticdump --input=http://localhost:9200/jobs_en --output=$ | gzip > scripts/jobs_en.json.gz
elasticdump --input=http://localhost:9200/jobs_fr --output=$ | gzip > scripts/jobs_fr.json.gz
```

## Troubleshooting
- If you encounter errors running `docker-compose-jb build`, such as:
> `ERROR [src_dotnet-cli internal] load metadata for docker.io/library/src_notifications-job-alerts:latest`

You can try building each service individually to avoid race conditions:
```bash
docker-compose-jb build jb-dotnet-build
docker-compose-jb build web
docker-compose-jb build admin
...
```
