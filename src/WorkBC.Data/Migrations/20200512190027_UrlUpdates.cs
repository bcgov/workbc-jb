using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UrlUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update systemSettings set [Value] = Replace([Value], '#/account/', '#/') where [value] like '%#/account%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}