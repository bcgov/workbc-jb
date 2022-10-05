using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobDetailsCTAUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'JOBS AND CAREERS',
[DefaultValue] = 'JOBS AND CAREERS'
WHERE [Name] = 'jbSearch.jobDetail.callToAction1Intro'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'View 500 career options and get the details you need, from job duties to projected demand. Search for a career that interests you.',
[DefaultValue] = 'View 500 career options and get the details you need, from job duties to projected demand. Search for a career that interests you.'
WHERE [Name] = 'jbSearch.jobDetail.callToAction1BodyText'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Search careers',
[DefaultValue] = 'Search careers'
WHERE [Name] = 'jbSearch.jobDetail.callToAction1LinkText'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'LABOUR MARKET & INDUSTRY',
[DefaultValue] = 'LABOUR MARKET & INDUSTRY'
WHERE [Name] = 'jbSearch.jobDetail.callToAction2Intro'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'HIGH OPPORTUNITY OCCUPATIONS',
[DefaultValue] = 'HIGH OPPORTUNITY OCCUPATIONS'
WHERE [Name] = 'jbSearch.jobDetail.callToAction2Title'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Explore careers that are expected to have above-average opportunities over the next decade.',
[DefaultValue] = 'Explore careers that are expected to have above-average opportunities over the next decade.'
WHERE [Name] = 'jbSearch.jobDetail.callToAction2BodyText'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Find the opportunities',
[DefaultValue] = 'Find the opportunities'
WHERE [Name] = 'jbSearch.jobDetail.callToAction2LinkText'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
