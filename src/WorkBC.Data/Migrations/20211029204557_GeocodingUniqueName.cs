using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class GeocodingUniqueName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM GeocodedLocationCache WHERE Latitude = '51.41284'");

            migrationBuilder.CreateIndex(
                name: "IX_GeocodedLocationCache_Name",
                table: "GeocodedLocationCache",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeocodedLocationCache_Name",
                table: "GeocodedLocationCache");
        }
    }
}
