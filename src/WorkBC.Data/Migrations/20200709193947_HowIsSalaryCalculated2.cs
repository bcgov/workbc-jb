using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class HowIsSalaryCalculated2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Where do I find my Job Alerts if email notifications are set to never?',
[DefaultValue] = 'Where do I find my Job Alerts if email notifications are set to never?'
WHERE [Name] = 'jbAccount.jobAlerts.noEmailHelpQuestion'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
