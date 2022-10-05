using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateIndustryCareerProfileCalloutUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings 
                set value = '/Labour-Market-Industry/Industry-and-Sector-Information/Industry-and-Sector-Outlooks.aspx'
                where name in ('jbAccount.industryProfiles.callToAction2LinkUrl',
                'jbAccount.careerProfiles.callToAction2LinkUrl')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
