using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameIndustry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("IndustryCodes",newName: "Industries");
            migrationBuilder.RenameColumn("NaicsId", "Jobs", "IndustryId");
            migrationBuilder.RenameColumn("NaicsId", "JobVersions", "IndustryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
