using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FilterCopyUpates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set [value] = 'Location' where [name] = 'shared.filters.locationTitle'");
            migrationBuilder.Sql(@"update SystemSettings set [value] = 'Job type' where [name] = 'shared.filters.jobTypeTitle'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
