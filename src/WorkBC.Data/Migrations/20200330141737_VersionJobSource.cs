using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class VersionJobSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Jobs", 
                "JobSource");

            migrationBuilder.AddColumn<byte>(
                name: "JobSource",
                table: "JobVersions",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.Sql(
                "update jobversions set jobsource = jobids.JobSource from jobids inner join jobversions on jobversions.jobid = jobids.id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobSource",
                table: "JobVersions");

            migrationBuilder.RenameColumn(
                name: "JobSource",
                table: "Jobs",
                "Source");
        }
    }
}
