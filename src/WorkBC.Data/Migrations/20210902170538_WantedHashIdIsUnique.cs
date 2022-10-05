using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class WantedHashIdIsUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ImportedJobsWanted");

            migrationBuilder.DropIndex(
                name: "IX_ImportedJobsWanted_HashId",
                table: "ImportedJobsWanted");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedJobsWanted_HashId",
                table: "ImportedJobsWanted",
                column: "HashId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportedJobsWanted_HashId",
                table: "ImportedJobsWanted");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedJobsWanted_HashId",
                table: "ImportedJobsWanted",
                column: "HashId");
        }
    }
}
