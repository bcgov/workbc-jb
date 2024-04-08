using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class AddNocCodes2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_NocCode_NocCodeId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVersions_NocCode_NocCodeId",
                table: "JobVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NocCode",
                table: "NocCode");

            migrationBuilder.RenameTable(
                name: "NocCode",
                newName: "NocCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NocCodes",
                table: "NocCodes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "NocCodes2021",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FrenchTitle = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NocCodes2021", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_NocCodes_NocCodeId",
                table: "Jobs",
                column: "NocCodeId",
                principalTable: "NocCodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobVersions_NocCodes_NocCodeId",
                table: "JobVersions",
                column: "NocCodeId",
                principalTable: "NocCodes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_NocCodes_NocCodeId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVersions_NocCodes_NocCodeId",
                table: "JobVersions");

            migrationBuilder.DropTable(
                name: "NocCodes2021");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NocCodes",
                table: "NocCodes");

            migrationBuilder.RenameTable(
                name: "NocCodes",
                newName: "NocCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NocCode",
                table: "NocCode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_NocCode_NocCodeId",
                table: "Jobs",
                column: "NocCodeId",
                principalTable: "NocCode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobVersions_NocCode_NocCodeId",
                table: "JobVersions",
                column: "NocCodeId",
                principalTable: "NocCode",
                principalColumn: "Id");
        }
    }
}
