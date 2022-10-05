using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace WorkBC.Data.Migrations
{
    public partial class DropCityAndRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("City");
            migrationBuilder.DropTable("Region");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
