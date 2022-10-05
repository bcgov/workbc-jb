using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FixSystemSettingsNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update SystemSettings set [name] = LTRIM(RTRIM([name]))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
