using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class AddFKNoCCode2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new columns to dbo.Jobs and dbo.JobVersions tables.
            migrationBuilder.AddColumn<int>(
               name: "NocCodeId2021",
               table: "Jobs",
               type: "int",
               maxLength: 5,
               nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NocCodeId2021",
                table: "JobVersions",
                type: "int",
                maxLength: 5,
                nullable: true);

            //Create new foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_NocCodes2021_NocCodeId2021",
                table: "Jobs",
                column: "NocCodeId2021",
                principalTable: "NocCodes2021",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobVersions_NocCodes2021_NocCodeId2021",
                table: "JobVersions",
                column: "NocCodeId2021",
                principalTable: "NocCodes2021",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_NocCodes2021_NocCodeId2021",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVersions_NocCodes2021_NocCodeId2021",
                table: "JobVersions");

            migrationBuilder.DropColumn(
                name: "NocCodeId2021",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "NocCodeId2021",
                table: "JobVersions");


        }
    }
}
