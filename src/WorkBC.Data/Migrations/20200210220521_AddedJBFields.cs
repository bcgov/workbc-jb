using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddedJBFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "JbAccount",
                table: "ConfigurationSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "JbSearch",
                table: "ConfigurationSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JbAccount",
                table: "ConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "JbSearch",
                table: "ConfigurationSettings");
        }
    }
}
