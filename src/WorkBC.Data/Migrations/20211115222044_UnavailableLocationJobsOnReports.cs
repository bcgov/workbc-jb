using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UnavailableLocationJobsOnReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Locations set RegionId = 0 where LocationId = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
