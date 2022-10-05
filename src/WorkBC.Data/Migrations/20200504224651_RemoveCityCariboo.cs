using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveCityCariboo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Update Locations Set isHidden = 1 where LocationId = 350");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
