using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FixLocationIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // fixes some bad data that was added by the wanted importers

            migrationBuilder.Sql(@"
                                update jv2
                                set jv2.LocationId = j.LocationId
                                from jobVersions jv2 
                                inner join jobs j on j.JobId = jv2.JobId
                                where jv2.JobSourceId = 2 and jv2.LocationId = -5");

            migrationBuilder.Sql(@"DELETE FROM ReportPersistenceControl WHERE TableName = 'JobStats'");
            migrationBuilder.Sql(@"DELETE FROM JobStats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
