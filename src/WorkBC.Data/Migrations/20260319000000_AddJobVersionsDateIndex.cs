using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    /// <summary>
    /// Adds a composite index on JobVersions(DateVersionStart, DateVersionEnd)
    /// to dramatically improve the performance of tvf_GetJobsForDate, which
    /// scans these columns in a CTE to find active job versions for a given date.
    /// Without this index, the function does a sequential scan on the entire
    /// JobVersions table, causing timeouts on the Jobs by City, Jobs by Industry,
    /// and Jobs by NOC Code reports.
    /// </summary>
    public partial class AddJobVersionsDateIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_JobVersions_DateVersionStart_DateVersionEnd""
                ON public.""JobVersions"" (""DateVersionStart"", ""DateVersionEnd"");
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS public.""IX_JobVersions_DateVersionStart_DateVersionEnd"";
            ");
        }
    }
}
