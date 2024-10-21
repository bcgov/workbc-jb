using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SavedCareerIndustryProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DELETE FROM SAVEDJOBS WHERE JobId NOT IN (SELECT JOBID FROM JOBS)");

            migrationBuilder.RenameColumn("Noc2016", "Jobs", "NocCode");

            migrationBuilder.DropColumn(
                name: "LockBy",
                table: "AdminUsers");

            migrationBuilder.DropTable(name: "ConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "LockDate",
                table: "AdminUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLocked",
                table: "AdminUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockedByAdminUserId",
                table: "AdminUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedByAdminUserId",
                table: "AdminUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SavedCareerProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CareerProfileId = table.Column<int>(nullable: false),
                    AspNetUserId = table.Column<string>(nullable: true),
                    DateSaved = table.Column<DateTime>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCareerProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedIndustryProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndustryProfileId = table.Column<int>(nullable: false),
                    AspNetUserId = table.Column<string>(nullable: true),
                    DateSaved = table.Column<DateTime>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedIndustryProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    FieldType = table.Column<int>(nullable: false),
                    ModifiedByAdminUserId = table.Column<int>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.UniqueConstraint("AK_SystemSettings_Name", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_JobId",
                table: "SavedJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_LockedByAdminUserId",
                table: "AdminUsers",
                column: "LockedByAdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminUsers_AdminUsers_LockedByAdminUserId",
                table: "AdminUsers",
                column: "LockedByAdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedJobs_Jobs_JobId",
                table: "SavedJobs",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);


            var seedAdminUsers =
                @"SET identity_insert AdminUsers ON
                  GO
                  IF NOT EXISTS (SELECT * FROM AdminUsers WHERE Id = 1)
                  -- CREDENTIALS WERE REMOVED FOR GITHUB MIGRATION. THIS WAS USED TO SEED THE DATABASE INITIALLY AND WILL NEVER BE RUN AGAIN
                  INSERT INTO [AdminUsers]
                               ([Id],[SamAccountName],[Email],[FirstName],[LastName],[DateUpdated],[AdminLevel],[DateCreated],[Deleted],[Guid],[ModifiedByAdminUserId])
                         VALUES(1,'','','','',GetDate(),3,GetDate(),0,'',1)
                  GO
                  SET identity_insert AdminUsers OFF";

            migrationBuilder.Sql(seedAdminUsers);

            migrationBuilder.Sql("INSERT INTO [SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES ('jbSearch.job.newJobStatus', '3', 'A job is considered new if it was posted within this number of days', 3, 1, GetDate())");
            migrationBuilder.Sql("INSERT INTO [SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES ('jbAccount.jobAlert.email.newAlert.emailBody', 'Hello {0}\n\nYour {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:\n\n{3}\n\nYour job alerts will only be sent when new job opportunities are available. You can edit or delete your job alert by signing in to your account on WorkBC.\n\nPlease disregard this message if you received it in error.', 'This text will be used when sending a job-seeker an email notifying them of new jobs matching their job alert settings.', 2, 1, GetDate())");
            migrationBuilder.Sql("INSERT INTO [SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES ('jbSearch.filters.moreFilters.toolTip.youth', 'Young people between 15 and 30 years of age', 'The tooltip text for the youth option in the more filters filter.', 1, 1, GetDate())");
            migrationBuilder.Sql("INSERT INTO [SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES ('jbAccount.jobSeeker.survey.active', '0', 'Toggle the job seekers survey on or off', 4, 1, GetDate())");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminUsers_AdminUsers_LockedByAdminUserId",
                table: "AdminUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedJobs_Jobs_JobId",
                table: "SavedJobs");

            migrationBuilder.DropTable(
                name: "SavedCareerProfiles");

            migrationBuilder.DropTable(
                name: "SavedIndustryProfiles");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_SavedJobs_JobId",
                table: "SavedJobs");

            migrationBuilder.DropIndex(
                name: "IX_AdminUsers_LockedByAdminUserId",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "NocCode",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DateLocked",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "LockedByAdminUserId",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "ModifiedByAdminUserId",
                table: "AdminUsers");

            migrationBuilder.AddColumn<short>(
                name: "Noc2016",
                table: "Jobs",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "LockBy",
                table: "AdminUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockDate",
                table: "AdminUsers",
                type: "timestamp",
                nullable: true);
        }
    }
}
