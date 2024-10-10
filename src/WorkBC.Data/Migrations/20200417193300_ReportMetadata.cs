using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ReportMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSeekerAccountReport");

            migrationBuilder.CreateTable(
                name: "ReportMetadata",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 4, nullable: false),
                    ReportKey = table.Column<string>(maxLength: 10, nullable: true),
                    StatLabel = table.Column<string>(maxLength: 100, nullable: true),
                    StatOrder = table.Column<short>(nullable: false),
                    GroupKey = table.Column<string>(maxLength: 20, nullable: true),
                    GroupLabel = table.Column<string>(maxLength: 100, nullable: true),
                    GroupOrder = table.Column<short>(nullable: false),
                    IsTotal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportMetadata", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekerStats",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(nullable: false),
                    ReportMetadataKey = table.Column<string>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerStats", x => new { x.WeeklyPeriodId, x.ReportMetadataKey });
                    table.ForeignKey(
                        name: "FK_JobSeekerStats_ReportMetadata_ReportMetadataKey",
                        column: x => x.ReportMetadataKey,
                        principalTable: "ReportMetadata",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSeekerStats_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerStats_ReportMetadataKey",
                table: "JobSeekerStats",
                column: "ReportMetadataKey");

            migrationBuilder.Sql("DELETE FROM JobsByRegion");
            migrationBuilder.Sql("DELETE FROM JobSeekersByLocation");
            migrationBuilder.Sql("DELETE FROM ReportPersistenceControl");

            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('REGD','JS_ACCOUNT','New Registrations',1,'ACC_STATUS','Accounts by Status',1,0)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('NOAC','JS_ACCOUNT','Awaiting Email Activation',2,'ACC_STATUS','Accounts by Status',1,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('DEAC','JS_ACCOUNT','Deactivated',3,'ACC_STATUS','Accounts by Status',1,0)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('DEL','JS_ACCOUNT','Deleted',4,'ACC_STATUS','Accounts by Status',1,0)");

            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('APPR','JS_ACCOUNT','Apprentice',1,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('INDP','JS_ACCOUNT','Indigenous',2,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('MAT','JS_ACCOUNT','Mature',3,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('IMMG','JS_ACCOUNT','Newcomer to B.C.',4,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('PWD','JS_ACCOUNT','Person with a Disability',5,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('STUD','JS_ACCOUNT','Student',6,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('VET','JS_ACCOUNT','Veteran',7,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('VMIN','JS_ACCOUNT','Visible Minority',8,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('YTH','JS_ACCOUNT','Youth',9,'EMPLOY_GRP','Job Seeker Employment Group',2,1)");

            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('LOGN','JS_ACCOUNT','Total Logins',1,'ACC_ACTVTY','Account Activity',3,0)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('ALRT','JS_ACCOUNT','Job Seekers with Job Alerts',2,'ACC_ACTVTY','Account Activity',3,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('CAPR','JS_ACCOUNT','Job Seekers with Saved Career Profiles',3,'ACC_ACTVTY','Account Activity',3,1)");
            migrationBuilder.Sql("INSERT INTO ReportMetaData ([Key], ReportKey, StatLabel, StatOrder, GroupKey, GroupLabel, GroupOrder, IsTotal) VALUES ('INPR','JS_ACCOUNT','Job Seekers with Saved Industry Profiles',4,'ACC_ACTVTY','Account Activity',3,1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSeekerStats");

            migrationBuilder.DropTable(
                name: "ReportMetadata");

            migrationBuilder.CreateTable(
                name: "JobSeekerAccountReport",
                columns: table => new
                {
                    WeeklyPeriodId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Group = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    SortOrder = table.Column<short>(type: "smallint", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerAccountReport", x => new { x.WeeklyPeriodId, x.Label });
                    table.ForeignKey(
                        name: "FK_JobSeekerAccountReport_WeeklyPeriods_WeeklyPeriodId",
                        column: x => x.WeeklyPeriodId,
                        principalTable: "WeeklyPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
