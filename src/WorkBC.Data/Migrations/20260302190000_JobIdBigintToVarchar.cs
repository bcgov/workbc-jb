using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    /// <summary>
    /// Migrates all JobId columns from bigint to varchar(255) to support CUID-based IDs
    /// from the Innovibe API.
    /// Affected tables: JobIds, Jobs, ImportedJobsWanted, ImportedJobsFederal,
    ///                  ExpiredJobs, JobVersions, JobViews, DeletedJobs, SavedJobs
    /// </summary>
    public partial class JobIdBigintToVarchar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop existing FK constraints
                ALTER TABLE ""DeletedJobs"" DROP CONSTRAINT IF EXISTS ""FK_DeletedJobs_Jobs_JobId_990626572"";
                ALTER TABLE ""JobVersions"" DROP CONSTRAINT IF EXISTS ""FK_JobVersions_Jobs_JobId_62623266"";
                ALTER TABLE ""JobViews""    DROP CONSTRAINT IF EXISTS ""FK_JobViews_Jobs_JobId_1442104178"";
                ALTER TABLE ""SavedJobs""   DROP CONSTRAINT IF EXISTS ""FK_SavedJobs_Jobs_JobId_1362103893"";

                ALTER TABLE ""Jobs""                DROP CONSTRAINT IF EXISTS ""FK_Jobs_JobIds_JobId_1602104748"";
                ALTER TABLE ""ImportedJobsWanted""  DROP CONSTRAINT IF EXISTS ""FK_ImportedJobsWanted_JobIds_JobId_1586104691"";
                ALTER TABLE ""ImportedJobsFederal"" DROP CONSTRAINT IF EXISTS ""FK_ImportedJobsFederal_JobIds_JobId_1570104634"";
                ALTER TABLE ""ExpiredJobs""         DROP CONSTRAINT IF EXISTS ""FK_ExpiredJobs_JobIds_JobId_1726629194"";

                -- Change column types (root table first, then dependents)
                ALTER TABLE ""JobIds""              ALTER COLUMN ""Id""    TYPE varchar(255) USING ""Id""::text;
                ALTER TABLE ""Jobs""                ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""ImportedJobsWanted""  ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""ImportedJobsFederal"" ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""ExpiredJobs""         ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""JobVersions""         ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""JobViews""            ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""DeletedJobs""         ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;
                ALTER TABLE ""SavedJobs""           ALTER COLUMN ""JobId"" TYPE varchar(255) USING ""JobId""::text;

                -- Re-add FK constraints
                ALTER TABLE ""Jobs""                ADD CONSTRAINT fk_jobs_jobids               FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""ImportedJobsWanted""  ADD CONSTRAINT fk_importedjobswanted_jobids  FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""ImportedJobsFederal"" ADD CONSTRAINT fk_importedjobsfederal_jobids FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""ExpiredJobs""         ADD CONSTRAINT fk_expiredjobs_jobids         FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""DeletedJobs""         ADD CONSTRAINT fk_deletedjobs_jobs           FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
                ALTER TABLE ""JobVersions""         ADD CONSTRAINT fk_jobversions_jobs           FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
                ALTER TABLE ""JobViews""            ADD CONSTRAINT fk_jobviews_jobs              FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
                ALTER TABLE ""SavedJobs""           ADD CONSTRAINT fk_savedjobs_jobs             FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop FK constraints
                ALTER TABLE ""DeletedJobs""         DROP CONSTRAINT IF EXISTS fk_deletedjobs_jobs;
                ALTER TABLE ""JobVersions""         DROP CONSTRAINT IF EXISTS fk_jobversions_jobs;
                ALTER TABLE ""JobViews""            DROP CONSTRAINT IF EXISTS fk_jobviews_jobs;
                ALTER TABLE ""SavedJobs""           DROP CONSTRAINT IF EXISTS fk_savedjobs_jobs;
                ALTER TABLE ""Jobs""                DROP CONSTRAINT IF EXISTS fk_jobs_jobids;
                ALTER TABLE ""ImportedJobsWanted""  DROP CONSTRAINT IF EXISTS fk_importedjobswanted_jobids;
                ALTER TABLE ""ImportedJobsFederal"" DROP CONSTRAINT IF EXISTS fk_importedjobsfederal_jobids;
                ALTER TABLE ""ExpiredJobs""         DROP CONSTRAINT IF EXISTS fk_expiredjobs_jobids;

                -- Revert column types back to bigint
                ALTER TABLE ""SavedJobs""           ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""DeletedJobs""         ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""JobViews""            ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""JobVersions""         ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""ExpiredJobs""         ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""ImportedJobsFederal"" ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""ImportedJobsWanted""  ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""Jobs""                ALTER COLUMN ""JobId"" TYPE bigint USING ""JobId""::bigint;
                ALTER TABLE ""JobIds""              ALTER COLUMN ""Id""    TYPE bigint USING ""Id""::bigint;

                -- Re-add original FK constraints
                ALTER TABLE ""Jobs""                ADD CONSTRAINT ""FK_Jobs_JobIds_JobId_1602104748""                FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""ImportedJobsWanted""  ADD CONSTRAINT ""FK_ImportedJobsWanted_JobIds_JobId_1586104691""  FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""ImportedJobsFederal"" ADD CONSTRAINT ""FK_ImportedJobsFederal_JobIds_JobId_1570104634"" FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""ExpiredJobs""         ADD CONSTRAINT ""FK_ExpiredJobs_JobIds_JobId_1726629194""         FOREIGN KEY (""JobId"") REFERENCES ""JobIds""(""Id"");
                ALTER TABLE ""DeletedJobs""         ADD CONSTRAINT ""FK_DeletedJobs_Jobs_JobId_990626572""            FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
                ALTER TABLE ""JobVersions""         ADD CONSTRAINT ""FK_JobVersions_Jobs_JobId_62623266""             FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
                ALTER TABLE ""JobViews""            ADD CONSTRAINT ""FK_JobViews_Jobs_JobId_1442104178""              FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
                ALTER TABLE ""SavedJobs""           ADD CONSTRAINT ""FK_SavedJobs_Jobs_JobId_1362103893""             FOREIGN KEY (""JobId"") REFERENCES ""Jobs""(""JobId"");
            ");
        }
    }
}
