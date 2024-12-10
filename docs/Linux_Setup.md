# Linux setup

Running Job Board locally using Docker Compose.

## Initial setup
- `cd workbc-jb/src`
- `alias docker-compose-jb='docker-compose -f docker-compose.yml -f docker-compose.linux-dev.yml'`
- `docker-compose-jb up`
- Import an initial dump of the `jobboard` database because it cannot be rebuilt from original migrations (which were written for MSSQL Server)
```bash
gunzip -k -c scripts/jobboard-full.sql.gz | docker-compose-jb exec -T postgres psql --username workbc jobboard
```
- Stop the containers and restart to run the Job Board data migrations - allow 15 minutes running time for the `jobboard` database to be fully populated.

## Backup/restore database
- Take a full database dump: `docker-compose-jb exec -T postgres pg_dump --clean --username workbc jobboard | gzip > scripts/jobboard-full.sql.gz`
- Restore a full database dump:
```bash
docker-compose-jb exec -T postgres psql -U workbc jobboard < scripts/jobboard-reset.sql \
&& gunzip -k -c scripts/jobboard-full.sql.gz | docker-compose-jb exec -T postgres psql --username workbc jobboard
```
