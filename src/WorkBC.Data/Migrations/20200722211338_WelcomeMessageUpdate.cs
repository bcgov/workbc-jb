using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class WelcomeMessageUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<p>Your Account Profile has been improved to help you easily manage your saved jobs, recommendations and alerts, as well as your saved career and industry profiles. You can also manage your Personal Settings here.</p>
<p>Take note of the following features:</p>
<ol>
<li>Your Job Alerts will continue to recommend job postings based on your preferences. You can access them from your Account Profile and receive them via email.</li>
<li>If you are a returning user, your saved job searches have been merged into the enhanced Job Alerts section of your account.</li>
<li>You can find new Recommended Resources to help you with your job search or career exploration journey.</li>
</ol>',
[DefaultValue] = '<p>Your Account Profile has been improved to help you easily manage your saved jobs, recommendations and alerts, as well as your saved career and industry profiles. You can also manage your Personal Settings here.</p>
<p>Take note of the following features:</p>
<ol>
<li>Your Job Alerts will continue to recommend job postings based on your preferences. You can access them from your Account Profile and receive them via email.</li>
<li>If you are a returning user, your saved job searches have been merged into the enhanced Job Alerts section of your account.</li>
<li>You can find new Recommended Resources to help you with your job search or career exploration journey.</li>
</ol>'
WHERE [Name] = 'jbAccount.dashboard.newAccountMessageBody'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Welcome to your new Account Profile!',
[DefaultValue] = 'Welcome to your new Account Profile!'
WHERE [Name] = 'jbAccount.dashboard.newAccountMessageTitle'");

            migrationBuilder.Sql(
                "update Locations set City = 'Fort Nelson 2', [Label] = 'Fort Nelson 2' where LocationId = 727");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
