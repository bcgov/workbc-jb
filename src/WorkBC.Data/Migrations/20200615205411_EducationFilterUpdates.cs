using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class EducationFilterUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set [value] = 'Education' where [name] = 'shared.filters.educationTitle'");
            migrationBuilder.Sql(@"update SystemSettings set [value] = '<i>Note: You may see fewer results than you were expecting because an algorithm is used to get minimum education levels for job postings that are external to WorkBC.ca. If you see this, try removing the filter.</i>' where [name] = 'shared.filters.educationNote'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
