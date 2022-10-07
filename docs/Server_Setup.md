# WorkBC JobBoard Server Setup 

#### dev = zuko.idir.bcgov
#### test = katara.idir.bcgov
#### prod 1 = appa.idir.bcgov (job board api 1)
#### prod 2 = aang.idir.bcgov (job board api 2)
#### prod 3 = momo.idir.bcgov (scheduled tasks / job board admin)

##### 1. Enable Windows Roles
* Launch the Server Manager
* Choose "Add roles and features"
* Enable the "Web Server (IIS)" role
* Leave all default Role services selected, but also include the following
    * HTTP Redirection
    * IP and Domain Restrictions
    * IIS Management Scripts and Tools
    * Management Service

##### 2. Configure proxy in IE
* Forward proxy requests to:
    * dev/test: [dev.forwardproxy.aest.gov.bc.ca](http://dev.forwardproxy.aest.gov.bc.ca/):80
    * prod: [forwardproxy.aest.gov.bc.ca](http://forwardproxy.aest.gov.bc.ca/):80
* SSL does not work through the proxy.  
* In order to get Nuget working with the SSL certificate errors, you will need to use MMC to install the Baltimore Cybertrust Root certifiate into the Trusted Root Certificate Authorities for the computer account

#### 3. Install 3rd Party Apps
* DotNet Core 5.0 SDK  (we will use Runtime on prod, not SDK)
* DotNet Core Hosting Bundle
* Microsoft Web Deploy 4.0 (select “configure for Non Administrator Deployments”)

Install these on DEV & TEST ONLY
* Node JS 64 bit for Windows  (used by build agent)
* Git for Windows 64 bit
* Chrome 

* _Note: SSL errors may prevent direct download.  So download the apps on your PC and paste into the RDP window.  I kept a copy of everything I downloaded in e:\Downloads	on zuko_

#### 4. Download and Install ElasticSearch 
* At the time of writing these instructions we have confirmed that the site works with versions 7.0.0 - 7.16.1
* I unzipped ElasticSearch into E:\elasticsearch  (folders with spaces like “Program Files” cause Java issues)
* Installed and started the service
    * E:\elasticsearch\bin>elasticsearch-service install
    * E:\elasticsearch\bin>elasticsearch-service start    
    * Set the startup type to Automatic AND set the Java options as shown in the file Elasticsearch-JVM-Options.png
        * E:\elasticsearch\bin>elasticsearch-service manager
* Go to http://localhost:9200 in Chrome to confirm that it is running
* NOTE: PRODUCTION WILL BE A 3-NODE CLUSTER.  MORE CONFIG IS NEEDED WHEN SETTING UP PROD! EACH WEB SERVER WILL ACCESS THE CLUSTER FROM IT'S LOCAL OWN LOCALHOST ADDRESS. SEE [CLUSTER NOTES](Cluster/README.md)

#### 5. Download and Install Redis
* Microsoft Version: https://github.com/MicrosoftArchive/redis/releases
* use the msi installer (version 3.2.100)
	* Add Redis to the windows path when prompted
	* Add firewall exception when prompted
	* Set max memory to 1024 MB
* command prompt as administrator
```
C:\Program Files\Redis>redis-server.exe redis.windows-service.conf --service-install
```

* Test Redis
```
C:\Program Files\Redis>redis-cli.exe
127.0.0.1:6379> SET FOO BAR
OK
127.0.0.1:6379> GET FOO
"BAR"
127.0.0.1:6379> exit
```
* NOTE: PRODUCTION WILL BE A 3-NODE CLUSTER WITH SENTINELS.  MORE CONFIG IS NEEDED WHEN SETTING UP PROD! EACH WEB SERVER WILL ACCESS THE CLUSTER FROM IT'S LOCAL OWN LOCALHOST ADDRESS. SEE [CLUSTER NOTES](Cluster/README.md)

#### 6. Create Service Accounts
* The IDIR service account will be used to connect to SQL server using a trusted connection, without requiring usernames and passwords to be stored in configuration files.  
* WRKBCJBD (DEV), WRKBCJBT (TEST) & WRKBCJBP (PROD). 
* The service account will need to be a member of the "db_owner" role on both databases (we are using code-first migrations so it needs to be able to drop, alter and create objects in the SQL schema)
* The service account will also need the "Log on as a batch job" right on both Zuko and Katara. (because we are going to use it with Windows Task Scheduler)

#### 7. Create Databases
* Created databases on stuart.idir.bcgov called WorkBC_JobBoard_DEV and WorkBC_JobBoard_TEST
* Add IDIR\s_WKBCJB_DT$ to database owner role on WorkBC_JobBoard_Dev
* Add IDIR\s_WKBCJB_DT$ to database owner role on WorkBC_JobBoard_Test
* The service account also needs read access to the WorkBC_Enterprise database

#### 8. Copy the folder structure from an existing environment
* Copy these 3 folders and their sub-folders from an existing server environment (e.g dev)
    * E:\Logs
    * E:\Scheduled Tasks
    * E:\Websites
* Their contents will be replaced later once TFS is configured.  But this step allows IIS to be configured which is a prerequisite to configuring TFS.  

#### 9. Configure IIS for the admin website
* Add a new website called WorkBC.Admin
* Set the Physical Path to E:\Websites\WorkBC.Admin
* Set the port to 8080
* Go to http://localhost:8080 in Chrome.  After a delay you should get an error 500.30 In-Process Start Failure
* Edit E:\Websites\WorkBC.Admin\appsettings.json and change the DefaultConnection to 
	* Dev: "Server=stuart.idir.bcgov\\SQL2017;integrated security=true;Database=WorkBC_JobBoard_Dev"
	* Test: "Server=stuart.idir.bcgov\\SQL2017;integrated security=true;Database=WorkBC_JobBoard_Test"
* Change the "WorkBCLogPath" to "E:\\Logs" 
* Make sure ApplyMigrations is set to "true"
* Change the WorkBC.Admin Application pool in IIS to run as IDIR\s_WKBCJB_DT$
* Change the .NET CLR version to "No Managed Code"
* Under advance Application Pool settings, set "Load User Profile" to "True"
* Load the website again.  It should work this time.  __And it should create the database tables too.__
* __Troubleshooting__:  Run this command to start the website from the console _"E:\Websites\WorkBC.Admin>workbc.admin.exe"_. Read any errors that appear.

* NOTE: FOR PRODUCTION BOTH THE ADMIN AND PUBLIC WEBSITE WILL BE SET UP ON ALL THREE SERVERS TO SIMPIFY TFS CONFIGURATION.

#### 10. Start importing data (test the importer)
* Edit E:\Scheduled_Tasks\WorkBC.Importers.Federal\appsettings.json
* Change the connection string the the stuart connection from above 
* If it works with no errors, you can press ctrl-c if you don't want to wait for all the jobs to import

#### 11. Configure IIS for the public website
* Stop the Default Website if it is running
* Create a new website called WorkBC.JobBoard
* Physical Path = E:\Websites\WorkBC.Web
* Bindings = All Unassigned: 80
* Change the WorkBC.JobBoard App pool to run as IDIR\s_WKBCJB_DT$
* Change the .NET CLR version to "No Managed Code"
* Under advance Application Pool settings, set "Load User Profile" to "True"
* Go to http://localhost in Chrome.  You should see a WorkBC website. 

* NOTE 1: FOR PRODUCTION BOTH THE ADMIN AND PUBLIC WEBSITE WILL BE SET UP ON ALL THREE SERVERS TO SIMPIFY TFS CONFIGURATION.

* NOTE 2: THE TWO PUBLIC WEBSITE PRODUCTION SERVERS WILL NEED TO BE LOAD BALANCED

* NOTE 3: The Kentico servers make a request to the Job Board Servers to get a version number for cache busting.  A binding must be available internally. The URL accesed on dev is https://dev-api-jobboard.workbc.ca/api/SystemSettings/Version. (the URL comes from the 	JobBoardApiServer AppSetting setting in the Kentico site web.config)

##### 12. Configure TFS Build and Deployment Agents
* Add two new agent queues
    * http://bonnie.idir.bcgov/tfs/Economy/WorkBC%20-%20Job%20Board/_admin/_AgentQueue
    * Click “New queue” and call the queue “WorkBC-JobBoard-Dev-Deployment” or “WorkBC-JobBoard-Test-Deployment”
    * Click “New queue” and call the queue “WorkBC-JobBoard-Build” (dev and test share this queue)
* NOTE: PRODUCTION SERVERS ONLY NEED A DEPLOYMENT QUEUE, NOT A BUILD QUEUE!


* Create 2 local user accounts called tfsdeployment & tfsbuild
    * Open “Local Security Policy”
    * Under Local Policies / User Rights Assignment make sure the users both have “Log on as a service”
* NOTE: PRODUCTION SERVERS ONLY tfsdeployment NOT tfsbuild!

* Add tfsdeployment to the local Administrators group (this is needed to deploy IIS websites through TFS)

* Download the TFS agent onto your local machine and then copy it to the E:\Downloads folder on the server
    * http://bonnie.idir.bcgov/tfs/Economy/_admin/_AgentPool

* Unzip the file into E:\Agents\devdeploymentagent (dev) or E:\Agents\testdeploymentagent (test)

* Start a powershell session as admin

```
PS E:\Agents\devdeploymentagent> ./config.cmd

>> Connect:

Enter server URL > http://bonnie.idir.bcgov/tfs/Economy
Enter authentication type (press enter for Integrated) >
Connecting to server ...

>> Register Agent:

Enter agent pool (press enter for default) > WorkBC-JobBoard-Dev-Deployment
Enter agent name (press enter for ZUKO) > zuko.devdeploymentagent
Scanning for tool capabilities.
Connecting to the server.
Successfully added the agent
Testing agent connection.
Enter work folder (press enter for _work) >
2019-08-15 22:14:30Z: Settings Saved.
Enter run agent as service? (Y/N) (press enter for N) > Y
Enter User account to use for the service (press enter for NT AUTHORITY\NETWORK SERVICE) > .\tfsdeployment
Enter Password for the account ZUKO\tfsdeployment > **************
Granting file permissions to 'ZUKO\tfsdeployment'.
Service vstsagent.bonnie.zuko.devdeploymentagent successfully installed
Service vstsagent.bonnie.zuko.devdeploymentagent successfully set recovery option
Service vstsagent.bonnie.zuko.devdeploymentagent successfully configured
Service vstsagent.bonnie.zuko.devdeploymentagent started successfully
```

* repeat for buildagent/tfsbuild using the E:\Agents\buildagent folder (DEV & TEST ONLY!!)

#### 13. Add TFS environent and set environment variables
* Go to http://bonnie.idir.bcgov/tfs/Economy
* Go to "WorkBC - Job Board" - "Build & Release" - "Deploy Job Board" - "+ Releases" - "Create Release"
* Accept the defaults and click on "Create"
* Click the ellipsis in the release and click on "Open"
* Click on "Environments" - "Add environment" - "Clone"
* Clone a "Test Server" from the "Dev Server"
* Click the ellipsis on the "Test Server" and select "Configure variables", configure the same variables as in Dev Server
* While "Test Server" is highlighted, click "Run on agent", in the right panel, change Deployment queue to "WorkBC-JobBoard-Test-Deployment"

#### 14. Run the TFS Build and Deployment
* Build and deployment should be green before moving to the next step

#### 15. Confirm that the JobBoard website and the Admin website both work
* Go to http://localhost in Chrome.  You should see a WorkBC JobBoard website. 
* Go to http://localhost:8080 in Chrome for the admin website


#### 16. Bulk import wanted jobs
* Execute E:\Scheduled_Tasks\WorkBC.Importers.Wanted\WorkBC.Importers.Wanted.exe --bulk

#### 17. Set up Scheduled Tasks

##### a. Importers and Indexers

* Before you set this up, make sure you can run all the tasks below from the command line.
* Use the windows task scheduler to schedule the following console apps in the order below..
    * E:\Scheduled_Tasks\WorkBC.Importers.Federal\WorkBC.Importers.Federal.exe
    * E:\Scheduled_Tasks\WorkBC.Indexers.Federal\WorkBC.Indexers.Federal.exe
        * when you run this task for the first time, you need to add the command-line argument --reindex
    * E:\Scheduled_Tasks\WorkBC.Importers.Wanted\WorkBC.Importers.Wanted.exe
    * E:\Scheduled_Tasks\WorkBC.Indexers.Wanted\WorkBC.Indexers.Wanted.exe
* For the schedule tasks, run them under the identity IDIR\s_WKBCJB_DT$
* I currently have them scheduled to run every 6 hours

##### b. Job Alert Email Service

* E:\Scheduled_Tasks\WorkBC.Notifications.JobAlerts\WorkBC.Notifications.JobAlerts.exe
* This should run every day.  I think the old site runs it at 6:00am.

* NOTE: PRODUCTION ONLY NEEDS THESE TO RUN ON ONE OF THE THREE SERVERS.  THEY WILL RUN ON THE SERVER THAT HOSTS THE ADMIN SITE

#### 18. Changes to TFS Configuration

* The task Run EFMigrationRunner.exe should only be run on the server that hosts the admin site and runs the schedule tasks (Prod 3 / Momo).
* To control this by environment, set AppSettings.ApplyMigrations to either true or false in the TFS environment variables
