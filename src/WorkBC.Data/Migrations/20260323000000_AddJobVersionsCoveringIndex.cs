using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    /// <summary>
    /// Adds a covering index on JobVersions to eliminate table lookups
    /// in tvf_GetJobsForDate. The INCLUDE columns allow the function to
    /// read all needed data directly from the index, avoiding expensive
    /// heap fetches and temp file I/O for the 2M+ row result set.
    /// This reduces the Jobs by Industry report query from ~94s to ~22s.
    /// </summary>
    public partial class AddJobVersionsCoveringIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_JobVersions_DateRange_Covering""
                ON public.""JobVersions"" (""DateVersionStart"", ""DateVersionEnd"")
                INCLUDE (""JobId"", ""VersionNumber"", ""JobSourceId"", ""LocationId"",
                         ""NocCodeId2021"", ""IndustryId"", ""DateFirstImported"",
                         ""PositionsAvailable"");
            ");

            // Drop the old non-covering index since the new one supersedes it
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS public.""IX_JobVersions_DateVersionStart_DateVersionEnd"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS public.""IX_JobVersions_DateRange_Covering"";
            ");

            // Restore the original non-covering index
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_JobVersions_DateVersionStart_DateVersionEnd""
                ON public.""JobVersions"" (""DateVersionStart"", ""DateVersionEnd"");
            ");
        }
    }
}
