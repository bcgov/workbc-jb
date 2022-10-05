using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateDatePostedTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set value = 'Date Posted' where name = 'shared.filters.datePostedTitle'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
