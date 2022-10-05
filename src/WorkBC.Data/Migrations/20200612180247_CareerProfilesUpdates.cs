using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CareerProfilesUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"Update SystemSettings set [Value] = 'Discover over 500 career options and learn about the duties, salary, education, job prospects and much more.' WHERE [Name] = 'jbAccount.careerProfiles.callToAction1BodyText'");
            migrationBuilder.Sql(
                @"Update SystemSettings set [Value] = '/Labour-Market-Industry/Labour-Market-Outlook.aspx' WHERE [Name] = 'jbAccount.careerProfiles.callToAction1BodyText'");
            migrationBuilder.Sql(
                @"Update SystemSettings set [Value] = 'Explore the Labour Market Outlook' WHERE [Name] = 'jbAccount.careerProfiles.callToAction1BodyText'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}