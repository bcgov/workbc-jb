#  Getting Started

[![Lifecycle:Maturing](https://img.shields.io/badge/Lifecycle-Maturing-007EC6)](https://github.com/bcgov/workbc-main)

# Code Structure

See [Workstation_Setup.md](docs/Workstation_Setup.md) for instructions related to developer workstation setup.
See [Linux_Setup.md](docs/Linux_Setup.md) for instructions related to Linux developer setup.

## Website Projects

* [WorkBC.Web](src/WorkBC.Web):  The public interface for the Job Board.  This is .NET Core API Application with and Angular front-end.
    * The Angular application is in [WorkBC.Web/ClientApp](src/WorkBC.Web/ClientApp), and it is split into 3 projects:
        * [WorkBC.Web/ClientApp/project/jb-search](src/WorkBC.Web/ClientApp/project/jb-search) - single page application for the search interface
        * [WorkBC.Web/ClientApp/project/jb-account](src/WorkBC.Web/ClientApp/project/jb-account) - single page application for the My Account interface
        * [WorkBC.Web/ClientApp/project/jb-lib](src/WorkBC.Web/ClientApp/project/jb-lib) - library of components shared between jb-search and jb-account

* [WorkBC.Admin](src/WorkBC.Admin):  The admin interface for the Job Board.  This is .NET Core MVC Application.

## Scheduled Tasks Projects (Console Applications)

* [WorkBC.Importers.Federal](src/WorkBC.Importers.Federal) - *(Legacy .NET)* Imports job XML from https://jobbank.gc.ca/ into the ImportedJobsFederal table. Replaced by Federal V2.
* [WorkBC.Importers.Federal.V2](src/WorkBC.Importers.Federal.V2) - *(PHP)* Replacement federal importer. Pulls job XML from jobbank.gc.ca, writes to ImportedJobsFederal and Jobs tables. Supports `--reimport` and `--maxjobs` flags. ECR image: `jb-importers-federal`.
* [WorkBC.Indexers.Federal](src/WorkBC.Indexers.Federal) - Parses XML in the ImportedJobsFederal table and indexes jobs in Elasticsearch
* [WorkBC.Importers.Innovibe](src/WorkBC.Importers.Innovibe) - *(PHP)* Imports jobs from the Innovibe API into the ImportedJobsWanted table and syncs to the Jobs table. Runs as a Docker container / Kubernetes CronJob. ECR image: `jb-importers-innovibe`.
* [WorkBC.Indexers.Wanted](src/WorkBC.Indexers.Wanted) - Parses XML in the ImportedJobsWanted table and indexes jobs in Elasticsearch
* [WorkBC.Notifications.JobAlerts](src/WorkBC.Notifications.JobAlerts) - Sends daily emails for job alerts

### PHP CLI Container

The [php-cli.Dockerfile](src/php-cli.Dockerfile) builds a combined shell container with **both** the Federal V2 and Innovibe PHP importers. ECR image: `jb-cli-innovibe` (name is historical — the container includes both importers).

Available commands inside the container:

```bash
# Federal V2 (jobbank.gc.ca)
cd /app/workbc-importers-federal-v2
php src/import.php                  # daily diff import
php src/import.php --reimport       # re-sync staging → Jobs only
php src/import.php --maxjobs=50     # bounded test run

# Innovibe
cd /app/workbc-importers-innovibe
php src/import.php                  # yesterday + today
php src/import.php --bulk           # full re-import

# Postgres
psql "host=$DB_HOST port=$DB_PORT user=$DB_USER dbname=$DB_NAME"
```

## Other Projects

* [WorkBC.Data](src/WorkBC.Data) - Entity Framework models & migrations
* [WorkBC.Elasticsearch.Indexing](src/WorkBC.Elasticsearch.Indexing) - XML parsing logic and other shared code used by Importers and Indexers projects
* [WorkBC.ElasticSearch.Models](src/WorkBC.ElasticSearch.Models) - Strongly typed objects used for Elasticsearch documents & requests (serializing, deserializing, searching)
* [WorkBC.ElasticSearch.Search](src/WorkBC.ElasticSearch.Search) - Library for building Elasticsearch queries based on the job board search interface
* [WorkBC.Shared](src/WorkBC.Shared) - Miscellaneous helpers, constants, extensions, shared by different projects
* [WorkBC.Tests](src/WorkBC.Tests) - C# unit tests & integration tests (xunit)

## Troubleshooting steps

* If the appSettings are not being read properly in the Visual Studio solution or their values appear to be being overwritten, please check the "User secrets".
* To access "User secrets", please right click on a project in Solution Explorer and click "Manage User secrets".
* Check if the settings here are the ones that are overwriting the project appSettings. If yes, please delete the user secrets.

## License

    Copyright 2022 Province of British Columbia

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
