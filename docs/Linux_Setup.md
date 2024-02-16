# Linux setup

Running Job Board locally using Docker Compose.

## Initial setup
- `cd src`
- `docker-compose -f docker-compose.yml -f docker-compose.linux-dev.yml up`
- `docker-compose -f docker-compose.linux-dev.yml exec mssql /opt/mssql-tools/bin/sqlcmd -U sa -P 18^fh4M08aB@`
```
create database WorkBC_JobBoard_DEV;
go
create login jobboard with password = 'password', default_database = WorkBC_JobBoard_DEV, check_expiration = off, check_policy = off;
use WorkBC_JobBoard_DEV;
create user jobboard for login jobboard;
alter role [db_owner] add member [jobboard];
go
```
- Stop the containers and re-up to run the Job Board data migrations - allow 30 minutes running time.
- `docker cp /path/to/WorkBC_Enterprise_DEV.bak src-mssql-1:/var/opt/mssql/data/WorkBC_Enterprise_DEV.bak`
- `docker-compose -f docker-compose.linux-dev.yml exec mssql /opt/mssql-tools/bin/sqlcmd -U sa -P 18^fh4M08aB@`
```
restore database [WorkBC_Enterprise_DEV] from disk = N'/var/opt/mssql/data/WorkBC_Enterprise_DEV.bak'
with replace, move 'WorkBCEnterprise' to '/var/opt/mssql/data/WorkBC_Enterprise_DEV.mdf',
move 'WorkBCEnterprise_log' to '/var/opt/mssql/data/WorkBC_Enterprise_DEV.ldf'
go
```
- Download SSOT dump at https://github.com/bcgov/workbc-ssot/blob/master/ssot-full.sql.gz
- `gunzip -k -c ssot-full.sql.gz | docker-compose exec -T postgres psql --username workbc ssot && docker-compose kill -s SIGUSR1 ssot`
