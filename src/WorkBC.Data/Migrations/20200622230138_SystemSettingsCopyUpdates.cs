using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsCopyUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'View High Opportunity Occupations'
                WHERE [name] = 'jbAccount.dashboard.resource1Title'");

            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'High Opportunity Occupations are those that are expected to experience higher demand and offer higher pay compared to other occupations.'
                WHERE [name] = 'jbAccount.dashboard.resource1Body'");

            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'Connect with WorkBC Centre staff to access employment services, including job search resources, skills assessment, training and work experience placement.'
                WHERE [name] = 'jbAccount.dashboard.resource2Body'");

            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'For each region you will find employment statistics, population data, 10-year labour market outlooks and more.'
                WHERE [name] = 'jbAccount.dashboard.resource3Body'");

            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'Manage and personalize the job search process in your account. Find job opportunities that match your skills and experience, and learn about careers and industries in B.C.'
                WHERE [name] = 'jbAccount.dashboard.introText'");

            migrationBuilder.Sql(@"update SystemSettings set [Value] = [DefaultValue]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
