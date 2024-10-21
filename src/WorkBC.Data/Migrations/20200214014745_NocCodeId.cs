using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class NocCodeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "NocCode", table: "Jobs", "NocCodeId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NocCodes",
                table: "NocCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NocCodes",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4)",
                oldMaxLength: 4);

            migrationBuilder.AddColumn<short>(
                name: "Id",
                table: "NocCodes",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "NocCodeId",
                table: "Jobs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_NocCodes_NocCodeId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NocCodes",
                table: "NocCodes");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_NocCodeId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NocCodes");

            migrationBuilder.DropColumn(
                name: "NocCodeId",
                table: "Jobs");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NocCodes",
                type: "varchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "NocCode",
                table: "Jobs",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NocCodes",
                table: "NocCodes",
                column: "Code");
        }
    }
}
