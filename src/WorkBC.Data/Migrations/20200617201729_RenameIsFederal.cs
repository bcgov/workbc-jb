using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameIsFederal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "IsFederal",
                "ImportedJobsWanted",
                "IsFederalOrWorkBc");

            migrationBuilder.Sql("update w set w.IsFederalOrWorkBc = 1 from ImportedJobsWanted w inner join Jobs j on w.JobId = j.JobId Where j.ExternalSourceName = 'WorkBC'");

            migrationBuilder.Sql("delete from JobVersions where JobId in (select JobId from jobs where ExternalSourceName = 'WorkBC')");

            migrationBuilder.Sql("delete from SavedJobs where JobId in (select JobId from jobs where ExternalSourceName = 'WorkBC')");

            migrationBuilder.Sql("delete from JobViews where JobId in (select JobId from jobs where ExternalSourceName = 'WorkBC')");

            migrationBuilder.Sql("delete from jobs where ExternalSourceName = 'WorkBC'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "IsFederalOrWorkBc",
                "ImportedJobsWanted",
                "IsFederal");
        }
    }
}