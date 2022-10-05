using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class removeyouthtooltip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM SystemSettings WHERE [Name] = 'shared.tooltips.youth'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
