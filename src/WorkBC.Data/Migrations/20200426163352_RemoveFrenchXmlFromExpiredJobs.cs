using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveFrenchXmlFromExpiredJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobPostFrench",
                table: "ExpiredJobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobPostFrench",
                table: "ExpiredJobs",
                type: "varchar",
                nullable: true);
        }
    }
}
