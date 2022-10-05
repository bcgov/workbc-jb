using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class WantedHashId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HashId",
                table: "ImportedJobsWanted",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ImportedJobsWanted_HashId",
                table: "ImportedJobsWanted",
                column: "HashId");

            migrationBuilder.Sql(
                @"UPDATE ImportedJobsWanted Set HashId = LEFT(SUBSTRING(SUBSTRING(JobPostEnglish, 22,17), PATINDEX('%[0-9.-]%', SUBSTRING(JobPostEnglish, 22,17)), 8000), PATINDEX('%[^0-9.-]%', SUBSTRING(SUBSTRING(JobPostEnglish, 22,17), PATINDEX('%[0-9.-]%', SUBSTRING(JobPostEnglish, 22,17)), 8000) + 'X') -1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportedJobsWanted_HashId",
                table: "ImportedJobsWanted");

            migrationBuilder.DropColumn(
                name: "HashId",
                table: "ImportedJobsWanted");
        }
    }
}
