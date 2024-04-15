using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class AddFKNoCCode2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new columns to dbo.Jobs and dbo.JobVersions tables.
            migrationBuilder.AddColumn<string>(
               name: "NocCodeId2021",
               table: "Jobs",
               type: "nvarchar(5)",
               maxLength: 5,
               nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NocCodeId2021",
                table: "JobVersions",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NocCodeId2021",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "NocCodeId2021",
                table: "JobVersions");
        }
    }
}
