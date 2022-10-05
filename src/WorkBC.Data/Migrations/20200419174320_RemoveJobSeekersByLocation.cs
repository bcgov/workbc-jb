using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveJobSeekersByLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerStats_ReportMetadata_ReportMetadataKey",
                table: "JobSeekerStats");

            migrationBuilder.DropTable(
                name: "JobSeekersByLocation");

            migrationBuilder.DropTable(
                name: "ReportMetadata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSeekerStats",
                table: "JobSeekerStats");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerStats_ReportMetadataKey",
                table: "JobSeekerStats");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerEventLog_DateLogged_EventTypeId",
                table: "JobSeekerEventLog");

            migrationBuilder.DropColumn(
                name: "ReportMetadataKey",
                table: "JobSeekerStats");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "JobSeekerStats",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabelKey",
                table: "JobSeekerStats",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSeekerStats",
                table: "JobSeekerStats",
                columns: new[] { "WeeklyPeriodId", "LabelKey", "RegionId" });

            migrationBuilder.CreateTable(
                name: "JobSeekerStatLabels",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 4, nullable: false),
                    Label = table.Column<string>(maxLength: 100, nullable: true),
                    IsTotal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerStatLabels", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerStats_LabelKey",
                table: "JobSeekerStats",
                column: "LabelKey");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerStats_JobSeekerStatLabels_LabelKey",
                table: "JobSeekerStats",
                column: "LabelKey",
                principalTable: "JobSeekerStatLabels",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("DELETE FROM ReportPersistenceControl");
            migrationBuilder.Sql("DELETE FROM JobSeekerStats");

            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[usp_PopulateJobsByRegion]");
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[usp_PopulateJobSeekersByLocation]");
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[usp_PopulateJobSeekerStats]");

            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('ALRT','Job Seekers with Job Alerts',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('APPR','Apprentice',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('CAPR','Job Seekers with Saved Career Profiles',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('DEAC','Deactivated',0)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('DEL','Deleted',0)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('IMMG','Newcomer to B.C.',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('INDP','Indigenous Person',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('INPR','Job Seekers with Saved Industry Profiles',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('LOGN','Total Logins',0)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('MAT','Mature',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('NOAC','Awaiting Email Activation',0)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('PWD','Person with a Disability',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('REGD','New Registrations',0)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('STUD','Student',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('VET','Veteran',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('VMIN','Visible Minority',1)");
            migrationBuilder.Sql("INSERT INTO JobSeekerStatLabels([Key],[Label],[IsTotal]) VALUES ('YTH','Youth',1)");

            migrationBuilder.Sql("\r\n/* Generates data in the JobStats table for a 1-week period.\r\n *\r\n * Created by Mike Olund <mike@oxd.com>\r\n * March 15, 2020\r\n *\r\n * NOTE:\r\n * Stored procedures and functions are updated using code-first migrations. \r\n * In order to keep localdev, dev, test and prod environments in sync,\r\n * they should never be modified directly in the sql database (unless \r\n * you don't mind having your changes wiped out by a future release).\r\n * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!\r\n */\r\nCREATE PROCEDURE [dbo].[usp_GenerateJobsStats] \r\n(\r\n\t@WeekEndDate DATETIME,\r\n\t@StoreName NVARCHAR(25)\r\n)\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DATETIME;\r\nDECLARE @PeriodId INT;\r\nDECLARE @EndDatePlus1 DATETIME;\r\n\r\nBEGIN TRY\r\n\t-- Get the WeeklyPeriod record\r\n\tSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\n\tFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n\tSET @EndDatePlus1 = DATEADD(DAY,1,@WeekEndDate);\r\n\r\n\t--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t   WHERE Report = @StoreName \r\n\t\t\t   AND WeeklyPeriodId = @PeriodId \r\n\t\t\t   AND DateCalculated < DateAdd(hour, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM ReportPersistenceControl \r\n\t\tWHERE Report = @StoreName AND WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobStats\r\n\t\tDELETE FROM JobStats WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t\t   WHERE WeeklyPeriodId = @PeriodId \r\n\t\t\t\t   AND Report = @StoreName) \r\n\tBEGIN \r\n\t\t-- insert a record into ReportPersistenceControl\r\n\t\tINSERT INTO ReportPersistenceControl (WeeklyPeriodId, Report, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, @StoreName AS Report, \r\n\t\t\tGetDate() AS DateCalculated, \r\n\t\t\t(CASE WHEN DateAdd(hour,48,@WeekEndDate) > GetDate() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\t\t-- insert records into JobStats\r\n\t\tINSERT INTO JobStats (WeeklyPeriodId, JobSourceId, RegionId, JobPostings, PositionsAvailable)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId,\r\n\t\t\tj.JobSourceId, \r\n\t\t\tISNULL(l.RegionId, 0) AS RegionId,\r\n\t\t\tCOUNT(*) AS JobPostings,\r\n\t\t\tSUM(PositionsAvailable) AS PositionsAvailable\r\n\t\tFROM dbo.tvf_GetJobsForDate(@EndDatePlus1) j\r\n\t\t\tLEFT OUTER JOIN Locations l ON l.LocationId = j.LocationId\r\n\t\tWHERE j.OriginalDatePosted >= @StartDate AND j.OriginalDatePosted < @EndDatePlus1\r\n\t\tGROUP BY j.JobSourceId, l.RegionId;\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK\r\n    END\r\n\tRETURN -1\r\nEND CATCH\r\n\r\nRETURN 0\r\n\r\nEND\r\n");
            migrationBuilder.Sql("\r\n/* Generates data in the JobSeekerStats table for a 1-week period.\r\n *\r\n * Created by Mike Olund <mike@oxd.com>\r\n * March 30, 2020\r\n *\r\n * NOTE:\r\n * Stored procedures and functions are updated using code-first migrations. \r\n * In order to keep localdev, dev, test and prod environments in sync,\r\n * they should never be modified directly in the sql database (unless \r\n * you don't mind having your changes wiped out by a future release).\r\n * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!\r\n */\r\nCREATE PROCEDURE [dbo].[usp_GenerateJobSeekerStats] \r\n(\r\n\t@WeekEndDate DATETIME,\r\n\t@StoreName NVARCHAR(25)\r\n)\r\nAS \r\n\r\nBEGIN\r\n\r\nBEGIN TRANSACTION;\r\n\r\nDECLARE @StartDate DATETIME;\r\nDECLARE @EndDatePlus1 DATETIME;\r\nDECLARE @WeekEndDateEOD DATETIME;\r\nDECLARE @PeriodId INT;\r\n\r\n-- Get the WeeklyPeriod record\r\nSELECT @StartDate = WeekStartDate, @PeriodId = Id\r\nFROM WeeklyPeriods WHERE WeekEndDate = @WeekEndDate;\r\n\r\n-- Get the End Date (the day after the WeekEndDate so we can include things that happened since midnight)\r\nSET @EndDatePlus1 = DATEADD(DAY,1,@WeekEndDate);\r\nSET @WeekEndDateEOD = DATEADD(MILLISECOND,-3,@EndDatePlus1);\r\n\r\n-- Store UserRegions in a Table Variable\r\nDECLARE @JobSeekerData TABLE (\r\n    AspNetUserId NVARCHAR(450) PRIMARY KEY,\r\n\tRegionId INT,\r\n\tDateRegistered DATETIME2(7) NOT NULL,\r\n\tAccountStatus SMALLINT NOT NULL,\r\n\tEmailConfirmed BIT NOT NULL,\r\n\tIsApprentice BIT NOT NULL,\r\n\tIsIndigenousPerson BIT NOT NULL,\r\n\tIsMatureWorker BIT NOT NULL,\r\n\tIsNewImmigrant BIT NOT NULL,\r\n\tIsPersonWithDisability BIT NOT NULL,\r\n\tIsStudent BIT NOT NULL,\r\n\tIsVeteran BIT NOT NULL,\r\n\tIsVisibleMinority BIT NOT NULL,\r\n\tIsYouth BIT NOT NULL\r\n);\r\n\r\nINSERT INTO @JobSeekerData\r\nSELECT AspnetUserId, \r\n(CASE WHEN RegionId IS NOT NULL THEN RegionId \r\n      WHEN CountryId = 37 AND ProvinceId <> 2 THEN -1\r\n\t  WHEN (CountryId IS NOT NULL AND CountryId <> 37) AND (ProvinceId IS NULL OR ProvinceId <> 2) THEN -2\r\n\t  ELSE NULL END) AS RegionId\r\n      ,DateRegistered\r\n      ,AccountStatus\r\n      ,EmailConfirmed\r\n      ,IsApprentice\r\n      ,IsIndigenousPerson\r\n      ,IsMatureWorker\r\n      ,IsNewImmigrant\r\n      ,IsPersonWithDisability\r\n      ,IsStudent\r\n      ,IsVeteran\r\n      ,IsVisibleMinority\r\n      ,IsYouth\r\nFROM dbo.tvf_GetJobSeekersForDate(@EndDatePlus1) js\r\nLEFT OUTER JOIN Locations l ON l.LocationId = js.LocationId\r\n\r\n\r\nBEGIN TRY\r\n\t--- Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record\r\n\tIF EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t   WHERE Report = @StoreName \r\n\t\t\t   AND WeeklyPeriodId = @PeriodId \r\n\t\t\t   AND DateCalculated < DATEADD(HOUR, 48, @WeekEndDate)) \r\n\tBEGIN \r\n\t\tDELETE FROM ReportPersistenceControl \r\n\t\tWHERE Report = @StoreName AND WeeklyPeriodId = @PeriodId;\r\n\t\t-- also delete associated record from JobSeekerStats\r\n\t\tDELETE FROM JobSeekerStats WHERE WeeklyPeriodId = @PeriodId;\r\n\tEND\r\n\r\n\tIF NOT EXISTS (SELECT * FROM ReportPersistenceControl \r\n\t\t\t\t   WHERE WeeklyPeriodId = @PeriodId \r\n\t\t\t\t   AND Report = @StoreName)\r\n\tBEGIN \r\n\t\t-- insert a record into ReportPersistenceControl\r\n\t\tINSERT INTO ReportPersistenceControl (WeeklyPeriodId, Report, DateCalculated, IsTotalToDate)\r\n\t\tSELECT @PeriodId AS WeeklyPeriodId, @StoreName AS Report, \r\n\t\t\tGETDATE() AS DateCalculated, \r\n\t\t\t(CASE WHEN DATEADD(HOUR,48,@WeekEndDate) > GETDATE() THEN 1 ELSE 0 END) AS IsTotalToDate;\r\n\r\n\r\n\t\t-- ACCOUNTS BY STATUS\r\n\r\n\t\t--New Registrations\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'REGD', COUNT(*) \r\n\t\tFROM @JobSeekerData\r\n\t\tWHERE DateRegistered >= @StartDate AND DateRegistered < @EndDatePlus1 \r\n\r\n\t\t--Awaiting Email Activation: This is the total at the end of the selected period. \r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'NOAC', COUNT(*) \r\n\t\tFROM @JobSeekerData\r\n\t\tWHERE DateRegistered >= @StartDate AND DateRegistered < @EndDatePlus1 AND AccountStatus = 4;\r\n\r\n\t\t-- Get statistics from the JobSeekerEventLog\r\n\r\n\t\t--Deactivated: This is accounts deactivated for this period.\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'DEAC', COUNT(DISTINCT AspNetUserId) \r\n\t\tFROM JobSeekerEventLog\r\n\t\tWHERE EventTypeId = 4 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1;\r\n\r\n\t\t--Deleted: This is total account deleted for this period.\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'DEL', COUNT(DISTINCT AspNetUserId) \r\n\t\tFROM JobSeekerEventLog\r\n\t\tWHERE EventTypeId = 6 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1;\r\n\r\n\r\n\t\t-- JOB SEEKER EMPLOYMENT GROUPS\r\n\r\n\t\t--Employment Groups: Is this for new registrations or total number of accounts? \r\n\t\t--This is total number of accounts\r\n\t\tDECLARE @IsApprentice INT;\r\n\t\tDECLARE @IsIndigenousPerson INT;\r\n\t\tDECLARE @IsMatureWorker INT;\r\n\t\tDECLARE @IsNewImmigrant INT;\r\n\t\tDECLARE @IsPersonWithDisability INT;\r\n\t\tDECLARE @IsStudent INT;\r\n\t\tDECLARE @IsVeteran INT;\r\n\t\tDECLARE @IsVisibleMinority INT;\r\n\t\tDECLARE @IsYouth INT;\r\n\r\n\t\tSELECT @IsApprentice = SUM(CAST(IsApprentice AS INT)), \r\n\t\t\t@IsIndigenousPerson = SUM(CAST(IsIndigenousPerson AS INT)), \r\n\t\t\t@IsMatureWorker = SUM(CAST(IsMatureWorker AS INT)),\r\n\t\t\t@IsNewImmigrant = SUM(CAST(IsNewImmigrant AS INT)),\r\n\t\t\t@IsPersonWithDisability = SUM(CAST(IsPersonWithDisability AS INT)),\r\n\t\t\t@IsStudent = SUM(CAST(IsStudent AS INT)),\r\n\t\t\t@IsVeteran = SUM(CAST(IsVeteran AS INT)),\r\n\t\t\t@IsVisibleMinority = SUM(CAST(IsVisibleMinority AS INT)),\r\n\t\t\t@IsYouth = SUM(CAST(IsYouth AS INT))\r\n\t\tFROM @JobSeekerData;\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'APPR',@IsApprentice);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'INDP',@IsIndigenousPerson);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'MAT',@IsMatureWorker);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'IMMG',@IsNewImmigrant);\r\n\t\t\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'PWD',@IsPersonWithDisability);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'STUD',@IsStudent);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'VET',@IsVeteran);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'VMIN',@IsVisibleMinority);\r\n\t\t\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tVALUES (@PeriodId,'YTH',@IsYouth);\r\n\r\n\r\n\t\t-- ACCOUNT ACTIVITY\r\n\r\n\t\t-- Get statistics from the JobSeekerEventLog\r\n\t\t-- Logins: This is total number of times users succesfully logged in for this period.\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'LOGN', COUNT(*) \r\n\t\tFROM JobSeekerEventLog\r\n\t\tWHERE EventTypeId = 1 AND DateLogged >= @StartDate AND DateLogged < @EndDatePlus1;\r\n\r\n\t\t--Job Seekers with Job Alerts, Job Seekers with Saved Career Profiles: \r\n\t\t--These are total number of accounts, not new registrations.\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'ALRT', COUNT(DISTINCT AspNetUserId) \r\n\t\tFROM JobAlerts \r\n\t\tWHERE DateCreated < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted > @WeekEndDateEOD);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'CAPR', COUNT(DISTINCT AspNetUserId) \r\n\t\tFROM SavedCareerProfiles \r\n\t\tWHERE DateSaved < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted > @WeekEndDateEOD);\r\n\r\n\t\tINSERT INTO dbo.JobSeekerStats\r\n\t\t\t(WeeklyPeriodId,[LabelKey],[Value])\r\n\t\tSELECT @PeriodId, 'INPR', COUNT(DISTINCT AspNetUserId) \r\n\t\tFROM SavedIndustryProfiles \r\n\t\tWHERE DateSaved < @EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted > @WeekEndDateEOD);\r\n\r\n\tEND\r\n\r\n\tCOMMIT TRANSACTION;\r\n\r\nEND TRY\r\n\r\nBEGIN CATCH\r\n    IF @@TRANCOUNT > 0\r\n    BEGIN\r\n        ROLLBACK;\r\n    END\r\n\tRETURN -1;\r\nEND CATCH\r\n\r\nRETURN 0;\r\n\r\nEND\r\n");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerStats_JobSeekerStatLabels_LabelKey",
                table: "JobSeekerStats");

            migrationBuilder.DropTable(
                name: "JobSeekerStatLabels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSeekerStats",
                table: "JobSeekerStats");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerStats_LabelKey",
                table: "JobSeekerStats");

            migrationBuilder.DropColumn(
                name: "LabelKey",
                table: "JobSeekerStats");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "JobSeekerStats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "ReportMetadataKey",
                table: "JobSeekerStats",
                type: "nvarchar(4)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSeekerStats",
                table: "JobSeekerStats",
                columns: new[] { "WeeklyPeriodId", "ReportMetadataKey" });

            migrationBuilder.CreateTable(
                name: "JobSeekersByLocation",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Users = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekersByLocation", x => new { x.WeeklyPeriodId, x.RegionId });
                    table.ForeignKey(
                        name: "FK_JobSeekersByLocation_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportMetadata",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    GroupKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GroupLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GroupOrder = table.Column<short>(type: "smallint", nullable: false),
                    IsTotal = table.Column<bool>(type: "bit", nullable: false),
                    ReportKey = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    StatLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StatOrder = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportMetadata", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerStats_ReportMetadataKey",
                table: "JobSeekerStats",
                column: "ReportMetadataKey");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerEventLog_DateLogged_EventTypeId",
                table: "JobSeekerEventLog",
                columns: new[] { "DateLogged", "EventTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerStats_ReportMetadata_ReportMetadataKey",
                table: "JobSeekerStats",
                column: "ReportMetadataKey",
                principalTable: "ReportMetadata",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
