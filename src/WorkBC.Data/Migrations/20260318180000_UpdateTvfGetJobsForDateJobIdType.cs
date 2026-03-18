using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    /// <summary>
    /// Updates the tvf_GetJobsForDate function to return varchar(255) for JobId
    /// instead of bigint, matching the column type change made in
    /// 20260302190000_JobIdBigintToVarchar.
    ///
    /// Without this fix:
    /// - Jobs by City and Jobs by Industry reports throw PostgresException 42804
    /// - Jobs by Region and Jobs by Source reports show 0 because usp_GenerateJobStats
    ///   silently fails when calling the broken function
    /// </summary>
    public partial class UpdateTvfGetJobsForDateJobIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PostgreSQL does not allow changing return types with CREATE OR REPLACE.
            // Must DROP then CREATE.
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS public.""tvf_GetJobsForDate""(timestamp without time zone);
            ");

            migrationBuilder.Sql(@"
                CREATE FUNCTION public.""tvf_GetJobsForDate""(par_enddateplus1 timestamp without time zone)
                RETURNS TABLE(
                    ""JobId"" varchar(255),
                    ""JobSourceId"" smallint,
                    ""LocationId"" integer,
                    ""NocCodeId2021"" integer,
                    ""IndustryId"" smallint,
                    ""DateFirstImported"" timestamp without time zone,
                    ""PositionsAvailable"" smallint
                )
                LANGUAGE plpgsql
                AS $$
                /*
                Returns a snapshot of a subset job data for a specified date.
                * Only data the is used by existing reports is included in the snapshot.
                *
                * NOTE:
                * Stored procedures and functions are updated using code-first migrations.
                * In order to keep localdev, dev, test and prod environments in sync,
                * they should never be modified directly in the sql database (unless
                * you don't mind having your changes wiped out by a future release).
                * PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
                */
                # variable_conflict use_column
                BEGIN
                    RETURN QUERY
                    WITH periodversion (""JobId"", versionnumber)
                    AS (SELECT
                        ""JobVersions"".""JobId"", MAX(""JobVersions"".""VersionNumber"") AS versionnumber
                        FROM public.""JobVersions""
                        WHERE ""JobVersions"".""DateVersionStart"" < par_EndDatePlus1
                          AND (""JobVersions"".""DateVersionEnd"" IS NULL OR ""JobVersions"".""DateVersionEnd"" >= par_EndDatePlus1)
                        GROUP BY ""JobVersions"".""JobId"")
                    SELECT
                        jv.""JobId"", jv.""JobSourceId"", jv.""LocationId"", jv.""NocCodeId2021"",
                        jv.""IndustryId"", jv.""DateFirstImported"", jv.""PositionsAvailable""
                    FROM public.""JobVersions"" AS jv
                    INNER JOIN periodversion AS pv
                        ON pv.""JobId"" = jv.""JobId"" AND pv.versionnumber = jv.""VersionNumber"";
                END;
                $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS public.""tvf_GetJobsForDate""(timestamp without time zone);
            ");

            migrationBuilder.Sql(@"
                CREATE FUNCTION public.""tvf_GetJobsForDate""(par_enddateplus1 timestamp without time zone)
                RETURNS TABLE(
                    ""JobId"" bigint,
                    ""JobSourceId"" smallint,
                    ""LocationId"" integer,
                    ""NocCodeId2021"" integer,
                    ""IndustryId"" smallint,
                    ""DateFirstImported"" timestamp without time zone,
                    ""PositionsAvailable"" smallint
                )
                LANGUAGE plpgsql
                AS $$
                # variable_conflict use_column
                BEGIN
                    RETURN QUERY
                    WITH periodversion (""JobId"", versionnumber)
                    AS (SELECT
                        ""JobVersions"".""JobId"", MAX(""JobVersions"".""VersionNumber"") AS versionnumber
                        FROM public.""JobVersions""
                        WHERE ""JobVersions"".""DateVersionStart"" < par_EndDatePlus1
                          AND (""JobVersions"".""DateVersionEnd"" IS NULL OR ""JobVersions"".""DateVersionEnd"" >= par_EndDatePlus1)
                        GROUP BY ""JobVersions"".""JobId"")
                    SELECT
                        jv.""JobId"", jv.""JobSourceId"", jv.""LocationId"", jv.""NocCodeId2021"",
                        jv.""IndustryId"", jv.""DateFirstImported"", jv.""PositionsAvailable""
                    FROM public.""JobVersions"" AS jv
                    INNER JOIN periodversion AS pv
                        ON pv.""JobId"" = jv.""JobId"" AND pv.versionnumber = jv.""VersionNumber"";
                END;
                $$;
            ");
        }
    }
}
