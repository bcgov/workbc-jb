using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameIndustryAndCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameTable("LocationLookups", newName: "Cities");

            migrationBuilder.RenameTable("NaicsCodes", newName: "IndustryCodes");

            migrationBuilder.RenameColumn("City", "Cities", "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
