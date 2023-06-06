# WorkBC Job board Visual Studio Setup 

### 1. Prerequisites
* NodeJS 14
* MS SQL Server
* Elastic Search 7
* Visual Studio
* .NET 6.0 SDK
* Docker Desktop
   * For Docker on a VM, you need to enable nested virtualization

### 1.a NodeJS
Download NodeJS 14 here: https://nodejs.org/en/download/
Use NVM to install it so you can have more than one version of NodeJS on your machine. It is important to note that v14 must be used in this project. Newer versions can cause issues.

### 1.b MS SQL Server
Download SQL Server 2017 or newer : https://www.microsoft.com/en-ca/download/details.aspx?id=55994
Be sure to download SQL Server Management Studio as well : https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15

### 1.c Elastic Search & Kibana

```
cd src
docker-compose -f docker-compose.local-dev.yml up
```

* *NOTE*: Production uses AWS OpenSearch which is a fork of Elasticsearch 7. (so don't upgrade to Elasticsearch 8)

### 1.d Visual Studio
Download Visual Studio 2022 or later: https://visualstudio.microsoft.com/downloads/
The community version will work fine for the project.

### 2. Setting up user secrets & .env file

* A number of secrets were removed fro the appsettings.json files when this application was migrated from TFS to Github
* These are stored in a spreadsheet and can be obtained from TBTB
* Get the commands from "USER-SECRETS" tab in the spreadsheets, and run them in the `/src/WorkBC.Web` folder using Powershell
    * You might need to change the Server name in ConnectionStrings:DefaultConnection, ConnectionStrings:EnterpriseConnection and ConnectionStrings:MigrationRunnerConnection to include an instance name if your SQL server was installed with an instance name (e.g. localhost\SQLEXPRESS)

### 3. Setting up databases

There are two type of databases in the project:
* Elastic Search
    * This will be used when doing searches on the website for jobs.
* SQL Server database
    * This will be used to save profiles, and any other database activities.
    * A record of the XML for the jobs will also be saved in the SQL database. 

### Setup
* Follow steps 1c to get Elastic Search services up and running
* Open Microsoft SQL Server Management Studio
* Connect to your localhost server
* Create a new database called "WorkBC_jobboard_dev"
* Download the backup database, saving it inside your MS SQL server instance 
_(Program Files > Microsoft SQL Server > YOUR_SERVER_VERSION_FOLDER > MSSQL > Backup \\files\Personal\\[Windows Username]\WorkBC_Enterprise_DEV.bak)_
  * Backup is located on the Stuart server here: ```H:\Backups\STUART$SQL2017\WorkBC_Enterprise_DEV\FULL```
* Go back to Microsoft SQL Server Management Studio and right-click on Databases and “Restore Database…”
* Select “Device” option and find the extracted file to select it and restore
* The server needs to have mixed mode authentication to allow connections to the database
   * Right click on the server in Microsoft SQL Server Management Studio and select “Properties”
   * Click on the “Security” page in the left hand menu of the popup that appears
   * Under “Server authentication”, select the radio button for “SQL Server and Windows Authentication mode”
   * Click the “OK” button to save the changes
* A SQL server login needs to be created for the website projects to pull from the database
   * In Microsoft SQL Server Management Studio, expand the “Security” folder under the root server
   * Right click on “Logins” and select the option “New Login”
   * Enter the login name as: jobbank
   *  Select “SQL Server authentication” option
   *  Set the password and password confirmation as: password
   *  Uncheck the option “Enforce password policy”
   *  Under the “Default database” dropdown, select the option “WorkBC_jobboard_dev”
   *  On the left hand menu, click on “User Mapping”
   *  Under “Users mapped to this login”, check the checkbox for “WorkBC_jobboard_dev”
   *  Under “Database role membership”, check the option “db_owner”
   *  Click the “OK” button to create the new login
* We need to set up the database to allow TCP/IP connections
   * Open Sql Server Configuration Manager in Windows
   * On the left menu, select “SQL Server Network Configuration”
   * Double click on “Protocals for {YOUR_MS_SQL_SERVER_NAME}”
   * Double click on the option “TCP/IP”
   * In the popup that appears, click on the dropdown to the right of “Enabled” and select “Yes”
   * Click the “Apply” button to save the configuration and close the popup
* Open Services app in Windows
* Find “SQL Server” instance, right click on it and select “Restart”
* Next we need to run the SQL migrations to create the database tables for the new job board database. 
    * Open the solution in Visual Studio
    * Go to Visual Studios > Tools > NuGet Package Manager > Package Manager Console
    * ```update-database -context jobboardcontext```

* Run the dll files in each following folders:
    * NOTE: The Federal API whitelisted an IP, if it fails the IP is wrong where the request is coming from. 
    * WorkBC.Importers.Federal\bin\Debug\net6.0 (Import Federal jobs to the SQL database)
    ```.\WorkBC.Importers.Federal.exe```
      * Note that this first task takes a long time. It also has a max of 20,000 records. When I ran this, I ended up having to run it twice to capture all 25,000 records required.
    * WorkBC.Importers.Wanted\bin\Debug\net6.0 (Import WantedAPI jobs to the SQL database)
    ```.\WorkBC.Importers.Wanted.exe```
    * WorkBC.Indexers.Federal\bin\Debug\net6.0 (Index the Federal jobs in ElasticSearch)
    ```.\WorkBC.Indexers.Federal.exe --reindex```
    * WorkBC.Indexers.Wanted\bin\Debug\net6.0 (Index the WantedAPI jobs in ElasticSearch)
    ```.\WorkBC.Indexers.Wanted.exe```

### 4. Running the job board web project (WorkBC.Web)

* Go to the "ClientApp" folder on disk
* Open PowerShell and run ```npm install```
* Run the following command to build the JbAccount, Jblib, and JbSearch Angular projects. It will be watching for any changes and recompile when any changes are made while running:
    * ```npm run watchAll```
    
* Open the project solution file in Visual Studio
* Ensure that the "WorkBC.Web" project is set as the startup project
* Run the project with or without debugging

### 5. Running the Admin web project (MVC project)

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

### 6. Running with Docker Desktop

* Get the key/value pairs from the "ENV" tab of the spredsheet in step 2 above, and put them into a file in /src called `.env`
    * You might need to change the Server name in ConnectionStrings__DefaultConnection, ConnectionStrings__EnterpriseConnection and ConnectionStrings__MigrationRunnerConnection to include an instance name if your SQL server was installed with an instance name (e.g. localhost\SQLEXPRESS)
* Create a user account in SQL Server called "jobboard" with the password "password"
* Add this user to the db_owner role on WorkBC_jobboard_dev and WorkBC_enterprise_dev 
* Run these commands from powershell
    ```
    cd /src
    docker-compose build
    docker-compose up
    ```
* Use http://localhost:8081 to access the main web site
* Use http://localhost:8080 to access the admin site
* You can use the console in the dotnet-cli container to run ad-hoc scheduled task commands. Some common examples are below.

    Run the Wanted Importer
    ```
    cd /app/workbc-importers-wanted
    dotnet WorkBC.Importers.Wanted.dll
    ```

    Run the Wanted Importer in bulk mode (get jobs for the past 30 days)
    ```
    cd /app/workbc-importers-wanted
    dotnet WorkBC.Importers.Wanted.dll --bulk
    ```

    Run the Wanted Indexer
    ```
    cd /app/workbc-indexers-wanted
    dotnet WorkBC.Indexers.Wanted.dll
    ```

    Run the Wanted Indexer and re-create the indexes
    ```
    cd /app/workbc-indexers-wanted
    dotnet WorkBC.Indexers.Wanted.dll -r
    ```

    Run the Federal Importer
    ```
    cd /app/workbc-importers-federal
    dotnet WorkBC.Importers.Federal.dll
    ```

    Run the Federal Indexer
    ```
    cd /app/workbc-indexers-federal
    dotnet WorkBC.Indexers.Federal.dll
    ```

    Run the Federal Indexer and re-create the indexes
    ```
    cd /app/workbc-indexers-federal
    dotnet WorkBC.Indexers.Federal.dll -r
    ```

    Run SQL migrations
    ```
    cd /app/efmigrationrunner
    dotnet EFMigrationRunner.dll
    ```

* Linux command are case sensitive.  
* You can use `--help` to see more options e.g. `dotnet WorkBC.Indexers.Federal.dll --help`

