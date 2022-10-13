# Getting Started

* See [Workstation_Setup.md](docs/Workstation_Setup.md) for instructions related to developer workstation setup. 
* See [Server_Setup.md](docs/Server_Setup.md) for instructions related to test/prod server setup. 

# Code Structure

## Website Projects

* [WorkBC.Web](src/WorkBC.Web):  The public interface for the Job Board.  This is .NET Core API Application with and Angular front-end.
    * The Angular application is in [WorkBC.Web/ClientApp](src/WorkBC.Web/ClientApp), and it is split into 3 projects:
        * [WorkBC.Web/ClientApp/project/jb-search](src/WorkBC.Web/ClientApp/project/jb-search) - single page application for the search interface
        * [WorkBC.Web/ClientApp/project/jb-account](src/WorkBC.Web/ClientApp/project/jb-account) - single page application for the My Account interface
        * [WorkBC.Web/ClientApp/project/jb-lib](src/WorkBC.Web/ClientApp/project/jb-lib) - library of components shared between jb-search and jb-account

* [WorkBC.Admin](src/WorkBC.Admin):  The admin interface for the Job Board.  This is .NET Core MVC Application.

## Scheduled Tasks Projects (Console Applications)

* [WorkBC.Importers.Federal](src/WorkBC.Importers.Federal) - Imports job XML from https://jobbank.gc.ca/ into the ImportedJobsFederal table. Also parses the XML and stores key metadata in the Jobs table for back-end reporting purposes.
* [WorkBC.Indexers.Federal](src/WorkBC.Indexers.Federal) - Parses XML in the ImportedJobsFederal table and indexes jobs in Elasticsearch
* [WorkBC.Importers.Wanted](src/WorkBC.Importers.Wanted) - Imports job XML from the Gartner (a.k.a. "Wanted") API into the ImportedJobsWanted table. Also parses the XML and stores key metadata in the Jobs table for back-end reporting purposes.
* [WorkBC.Indexers.Wanted](src/WorkBC.Indexers.Wanted) - Parses XML in the ImportedJobsWanted table and indexes jobs in Elasticsearch
* [WorkBC.Notifications.JobAlerts](src/WorkBC.Notifications.JobAlerts) - Sends daily emails for job alerts

## Other Projects 

* [WorkBC.Data](src/WorkBC.Data) - Entity Framework models & migrations
* [WorkBC.Elasticsearch.Indexing](src/WorkBC.Elasticsearch.Indexing) - XML parsing logic and other shared code used by Importers and Indexers projects
* [WorkBC.ElasticSearch.Models](src/WorkBC.ElasticSearch.Models) - Strongly typed objects used for Elasticsearch documents & requests (serializing, deserializing, searching)
* [WorkBC.ElasticSearch.Search](src/WorkBC.ElasticSearch.Search) - Library for building Elasticsearch queries based on the job board search interface
* [WorkBC.Shared](src/WorkBC.Shared) - Miscellaneous helpers, constants, extensions, shared by different projects 
* [WorkBC.Tests](src/WorkBC.Tests) - C# unit tests & integration tests (xunit)

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