using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobSourceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update jobversions set jobsource = jobids.JobSource from jobids inner join jobversions on jobversions.jobid = jobids.id");

            migrationBuilder.RenameColumn(
                name: "JobSource",
                table: "JobsByRegion",
                newName: "JobSourceId");

            migrationBuilder.RenameColumn(
                name: "JobSource",
                table: "JobVersions",
                newName: "JobSourceId");

            migrationBuilder.RenameColumn(
                name: "JobSource",
                table: "Jobs",
                newName: "JobSourceId");

            migrationBuilder.RenameColumn(
                name: "JobSource",
                table: "JobIds",
                newName: "JobSourceId");

            migrationBuilder.RenameColumn(
                name: "JobSource",
                table: "ExpiredJobs",
                newName: "JobSourceId");

            migrationBuilder.CreateTable(
                name: "JobSources",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    GroupName = table.Column<string>(maxLength: 50, nullable: true),
                    ListOrder = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSources", x => x.Id);
                });

            migrationBuilder.Sql(
                "insert into JobSources values(1, 'Federal', 'Jobs from National Job Bank XML', 1)");

            migrationBuilder.Sql(
                "insert into JobSources values(2, 'Wanted', 'Jobs from External API', 2)");

            migrationBuilder.CreateIndex(
                name: "IX_JobsByRegion_JobSourceId",
                table: "JobsByRegion",
                column: "JobSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobSourceId",
                table: "Jobs",
                column: "JobSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_JobIds_JobSourceId",
                table: "JobIds",
                column: "JobSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiredJobs_JobSourceId",
                table: "ExpiredJobs",
                column: "JobSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpiredJobs_JobSources_JobSourceId",
                table: "ExpiredJobs",
                column: "JobSourceId",
                principalTable: "JobSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_JobIds_JobSources_JobSourceId",
                table: "JobIds",
                column: "JobSourceId",
                principalTable: "JobSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobSources_JobSourceId",
                table: "Jobs",
                column: "JobSourceId",
                principalTable: "JobSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_JobsByRegion_JobSources_JobSourceId",
                table: "JobsByRegion",
                column: "JobSourceId",
                principalTable: "JobSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
