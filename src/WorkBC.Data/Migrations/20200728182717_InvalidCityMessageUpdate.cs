using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class InvalidCityMessageUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'The city <strong>{0}</strong> could not be found. Please enter a city in British Columbia.',
[DefaultValue] = 'The city <strong>{0}</strong> could not be found. Please enter a city in British Columbia.'
WHERE [Name] = 'jbSearch.errors.invalidCity'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
