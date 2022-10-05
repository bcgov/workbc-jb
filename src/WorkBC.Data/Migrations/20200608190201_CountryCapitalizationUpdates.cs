using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CountryCapitalizationUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Countries set [name] = REPLACE([name],' And ',' and ')");
            migrationBuilder.Sql("update Countries set [name] = REPLACE([name],' Of ',' of ')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
