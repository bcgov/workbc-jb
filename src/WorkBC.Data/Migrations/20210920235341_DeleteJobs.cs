using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class DeleteJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from ImportedJobsWanted");
            migrationBuilder.Sql("delete from DeletedJobs");
            migrationBuilder.Sql("delete from ExpiredJobs");
            migrationBuilder.Sql("delete from ImportedJobsFederal");
            migrationBuilder.Sql("delete from JobVersions");
            migrationBuilder.Sql("delete from Jobs");
            migrationBuilder.Sql("delete from JobStats");
            migrationBuilder.Sql("delete from JobViews");
            migrationBuilder.Sql("delete from JobIds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
