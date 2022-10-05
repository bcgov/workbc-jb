using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AdminUserDisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AdminUsers",
                "DisplayName");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AdminUsers");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AdminUsers",
                oldMaxLength: 20,
                maxLength: 60);

            migrationBuilder.Sql("UPDATE AdminUsers SET DisplayName = 'XT:Olund, Michael AEST:IN' WHERE ID = 1");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
