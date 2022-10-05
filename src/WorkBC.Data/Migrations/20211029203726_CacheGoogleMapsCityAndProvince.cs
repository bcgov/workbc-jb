using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CacheGoogleMapsCityAndProvince : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Views",
                table: "JobViews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "GeocodedLocationCache",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "GeocodedLocationCache",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.Sql(@"DELETE FROM GeocodedLocationCache WHERE IsPermanent = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "GeocodedLocationCache");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "GeocodedLocationCache");

            migrationBuilder.AlterColumn<int>(
                name: "Views",
                table: "JobViews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
