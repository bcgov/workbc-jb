using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveSurvey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from systemsettings where [name] = 'jbAccount.settings.surveyEnabled'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
