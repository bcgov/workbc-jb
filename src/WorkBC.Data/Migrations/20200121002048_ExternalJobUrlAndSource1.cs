using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ExternalJobUrlAndSource1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalSourceName",
                table: "Jobs",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSourceUrl",
                table: "Jobs",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalSourceName",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ExternalSourceUrl",
                table: "Jobs");
        }
    }
}
