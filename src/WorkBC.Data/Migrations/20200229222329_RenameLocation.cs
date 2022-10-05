using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("Cities", newName: "Locations");

            migrationBuilder.RenameColumn("Name", "Locations", "City");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
