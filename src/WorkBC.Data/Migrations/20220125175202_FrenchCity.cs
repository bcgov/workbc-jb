using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FrenchCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from GeocodedLocationCache where IsPermanent = 0");

            migrationBuilder.AddColumn<string>(
                name: "FrenchCity",
                table: "GeocodedLocationCache",
                type: "varchar(80)",
                maxLength: 80,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrenchCity",
                table: "GeocodedLocationCache");
        }
    }
}
