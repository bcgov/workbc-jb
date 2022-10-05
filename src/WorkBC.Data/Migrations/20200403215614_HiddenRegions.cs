using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class HiddenRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "IsHidden",
                "Regions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                "INSERT INTO Regions(Id,[Name],ListOrder,IsHidden) VALUES (-1,'Outside BC – within Canada',1001,1)");

            migrationBuilder.Sql(
                "INSERT INTO Regions(Id,[Name],ListOrder,IsHidden) VALUES (-2,'Outside Canada',1002,1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsHidden",
                "Regions");
        }
    }
}