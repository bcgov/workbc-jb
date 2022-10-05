# Data Migration Steps (upgrading from old job board to new job board)

## First run

* Create the database (in SSMS) and run the migrations
* Ensure that your user account has permission to these databases: Margo.JobBank_Site_Refresh (read), Margo.WorkBC_CMS_Site_Refresh (read), and WorkBC_JobBoard (db owner), 
* Edit the appsettings.json file for the WorkBC.UserMigration console app in E:\Scheduled_Tasks.  The setting below are for PROD.

```
  "AppSettings": {
    "CmsDatabase": "Margo.WorkBC_CMS_Site_Refresh",
    "JobBankDatabase": "Margo.JobBank_Site_Refresh",
    "MaxUsersToImport": 300
  }
```

* The importer will run in batches of 300.  Repeatedly restarting the exe is a workaround for an entity framework performance issue.
* Now run the user importer using import.bat.  This will take a long time (12-20 hours)
* DO NOT RUN THE ADDITIONAL DATA MIGRATION SCRIPTS AT THIS TIME (see "Final run" below)

## Subsequent runs pseudo-code

* The first time the importer was run, a table called JobBankUserImport was created in the WorkBC_JobBoard DB.
  * Back up the new JobBoard database 
  * Rename this table to JobBankUserImport2
  * Change MaxUsersToImport to 0

```
  "AppSettings": {
    "CmsDatabase": "Margo.WorkBC_CMS_Site_Refresh",
    "JobBankDatabase": "Margo.JobBank_Site_Refresh",
    "MaxUsersToImport": 0
  }
```

  * Run the importer again using WorkBC.UserMigration.exe and it will create another JobBankUserImport table but it won't import any users
  * Run the _Subsequent.sql script
  * Change MaxUsersToImport to 300
* Run the import.bat to import the new users and modified users
* Drop JobBankUserImport and JobBankUserImport2
* Rename JobBankUserImport3 to JobBankUserImport

## Final run (launch day)

* Do all the steps in "Subsequent runs"
* Run the 3 data migration scripts in the soltution folder under Build\DataMigrationScripts