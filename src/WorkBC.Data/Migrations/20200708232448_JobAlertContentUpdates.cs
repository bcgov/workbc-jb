using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobAlertContentUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Please specify a title for this alert.',
[DefaultValue] = 'Please specify a title for this alert.'
WHERE [Name] = 'shared.errors.jobAlertTitleRequired'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Where do I find my Job Alerts if email notifications are set to Never?',
[DefaultValue] = 'Where do I find my Job Alerts if email notifications are set to Never?'
WHERE [Name] = 'jbAccount.jobAlerts.noEmailHelpQuestion'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<p class=""pl-2"">Go to Job Alerts in your Account Profile to view jobs.</p>',
[DefaultValue] = '<p class=""pl-2"">Go to Job Alerts in your Account Profile to view jobs.</p>'
WHERE [Name] = 'jbAccount.jobAlerts.noEmailHelpAnswer'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
