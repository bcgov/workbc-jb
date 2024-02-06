# Linux / VS Code Setup

- `cd src`
- `docker-compose -f docker-compose.yml -f docker-compose.linux-dev.yml up`
- `docker-compose exec -T mssql /opt/mssql-tools/bin/sqlcmd -U sa -P 18^fh4M08aB@`
```
create database WorkBC_jobboard_dev;
go
create login jobboard with password = 'password', default_database = WorkBC_jobboard_dev, check_expiration = off, check_policy = off;
use WorkBC_jobboard_dev;
create user jobboard for login jobboard;
alter role [db_owner] add member [jobboard];
go
```
- `docker cp /path/to/WorkBC_Enterprise_DEV.bak src-mssql-1:/var/opt/mssql/data/WorkBC_Enterprise_DEV.bak`
```
restore database [WorkBC_Enterprise_DEV] from disk = N'/var/opt/mssql/data/WorkBC_Enterprise_DEV.bak'
with replace, move 'WorkBCEnterprise' to '/var/opt/mssql/data/WorkBC_Enterprise_DEV.mdf',
move 'WorkBCEnterprise_log' to '/var/opt/mssql/data/WorkBC_Enterprise_DEV.ldf'
go
```
- `docker-compose exec dotnet-cli`
