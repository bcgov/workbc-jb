using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CountryUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update countries set [Name] = Replace([Name],' Of', ' of') where [name] like '% Of'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
