using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameExternalSourceName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "ExternalSourceName",
                "Jobs",
                "OriginalSource");

            migrationBuilder.Sql("update Jobs set OriginalSource = 'Federal Job Bank' where JobSourceId = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "OriginalSource",
                "Jobs",
                "ExternalSourceName");
        }
    }
}