using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FilterCopyUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Narrow down by city name or postal code:',
[DefaultValue] = 'Narrow down by city name or postal code:'
WHERE [Name] = 'shared.filters.locationSearchLabel'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Narrow down by region(s):',
[DefaultValue] = 'Narrow down by region(s):'
WHERE [Name] = 'shared.filters.locationRegionSearchLabel'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Education Required',
[DefaultValue] = 'Education Required'
WHERE [Name] = 'shared.filters.educationTitle'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Job Type',
[DefaultValue] = 'Job Type'
WHERE [Name] = 'shared.filters.jobTypeTitle'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}