# WorkBC Job board Visual Studio Setup 

##### 1. Prerequisite
* NodeJS
* MS SQL Server
* Elastic Search 7
* Visual Studio
* .Net Core 5.0

##### 1.a NodeJS
Download the latest version of NodeJS here: https://nodejs.org/en/download/

##### 1.b
Download SQL Server 2017 or newer : https://www.microsoft.com/en-ca/download/details.aspx?id=55994
Be sure to download SQL Server Management Studio as well : https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15

##### 1.c Elastic Search
* Download **ElasticSearch** v7 here: https://www.elastic.co/downloads/elasticsearch
    * *NOTE*: ElasticSearch version 8+ require code changes due to lack of backword compability(_doc issue)

* Download **Kibana** v7 here: https://www.elastic.co/downloads/kibana

    * Extract both files on disk
    * To run go to the ElasticSearch folder, browse to the **bin** folder and run the *elasticsearch.bat* file. This will start the ElasticSearch service.
    * Now navigate to the Kibana folder, browse to the **bin** folder and run the *kibana.bat* file. This will start the Kibana service. This will take a bit longer to start, once done you should be able to navigate to _http://localhost:5601/_
    * *NOTE*: Both bat files need to be running in order to access ElasticSearch and Kibana.
    * You might encounter a request to configure Kibana and Elasticsearch:
        * Enrolment token:
            * get enrolment tocket
                ```elasticsearchFolder\bin\elasticsearch-create-enrollment-token -s kibana --url https://localhost:9200```
            * copy and paste into configuration wizard in browser
            * if failes try manual setup
        * Manual setup:
            * (optional) reset password for kibana_system user (optional)
                ```elasticsearchFolder\bin\elasticsearch-reset-password -i -u kibana_system --url https://localhost:9200```
            * check certification checkbox
            * login at configuration wizard with kibana_system user
            * get verification code
                ```kibanaFolder\bin\kibana-verification-code.bat```
            * put verification code into configuration wizard
            * rest goes automatically

##### 1.d Visual Studio
Download Visual Studio 2019 or later: https://visualstudio.microsoft.com/downloads/
The community version will work fine for the project.


##### 2. Setting up databases

There are two type of databases in the project:
* Elastic Search
    * This will be used when doing searches on the website for jobs.
* SQL Server database
    * This will be used to save profiles, and any other database activities.
    * A record of the XML for the jobs will also be saved in the SQL database. 

##### Setup
* Follow steps 1c to get Elastic Search services up and running
* Open Microsoft SQL Server Management Studio
* Connect
* Create a new database called "WorkBC_jobboard_dev"
* Download the backup database, saving it inside your MS SQL server instance 
_(Program Files > Microsoft SQL Server > YOUR_SERVER_VERSION_FOLDER > MSSQL > Backup \\files\Personal\\[Windows Username]\WorkBC_Enterprise_DEV.bak)_
* Go back to Microsoft SQL Server Management Studio and right-click on Databases and “Restore Database…”
* Select “Device” option and find the extracted file to select it and restore
* Add the following connection strings from the root of the WorkBC.Web folder by running the following commands in PowerShell (Setting up .Net secret keys)

    ___Elastic search connection___
    ```
     dotnet user-secrets set ConnectionStrings:ElasticSearchServer "http://localhost:9200"
    ```

    ___Connection to the Enterprise database___
    ```
    dotnet user-secrets set ConnectionStrings:EnterpriseConnection "server=YOUR_SERVER_NAME; Integrated security=true;Database=WorkBC_Enterprise_Dev"
    ```

    ___Connection to the new Job Board database___
    ```
    dotnet user-secrets set ConnectionStrings:DefaultConnection "server=YOUR_SERVER_NAME;Integrated security=true;Database=WorkBC_JobBoard_Dev"
    ```

* Next we need to run the SQL migrations to create the database tables for the new job board database. 
    * Open the solution in Visual Studio
    * Go to Visual Studios > Tools > NuGet Package Manager > Package Manager Console
    * ```update-database -context jobboardcontext```

* Run the dll files in each following folders:
    * NOTE: The Federal API whitelisted an IP, if it fails the IP is wrong where the request is coming from. 
    * WorkBC.Importers.Federal\bin\Debug\net5.0 (Import Federal jobs to the SQL database)
    ```.\WorkBC.Importers.Federal.exe```
    * WorkBC.Importers.Wanted\bin\Debug\net5.0 (Import WantedAPI jobs to the SQL database)
    ```.\WorkBC.Importers.Wanted.exe```
    * WorkBC.Indexers.Federal\bin\Debug\net5.0 (Index the Federal jobs in ElasticSearch)
    ```.\WorkBC.Indexers.Federal.exe --reindex```
    * WorkBC.Indexers.Wanted\bin\Debug\net5.0 (Index the WantedAPI jobs in ElasticSearch)
    ```.\WorkBC.Indexers.Wanted.exe```

##### 3. Running the job board web project (WorkBC.Web)

* Go to the "ClientApp" folder on disk
* Open PowerShell and run ```npm install```
* Run the following command to build the JbAccount, Jblib, and JbSearch Angular projects. It will be watching for any changes and recompile when any changes are made while running:
    * ```npm run watchAll```
    
* Open the project solution file in Visual Studio
* Ensure that the "WorkBC.Web" project is set as the startup project
* Run the project with or without debugging

##### 4. Running the Admin web project (MVC project)

* Browse to the "WorkBC.admin" project on disk.
* Open PowerShell and runn the following commands:
    * ```npm install```
    * ```npm run watch```
* Open the Visual Studio solution in Visual Studio
* Set the "WorkBC.admin" project as the startup project inside Visual Studio
* Run the project
* Notes: 
    * The database migrations for the admin site are run automatically 
    * Database migrations can be run from the indexers by using ```--migrate``` - this is good for debugging.

**Common issues:**
* The styling is not displaying correctly
    * This is usually because webpack did not run, you need to run webpack to compile the css for the admin project. 
* CSS changes isn't showing on the site
    * Webpack needs to run in order for your change to display. 
