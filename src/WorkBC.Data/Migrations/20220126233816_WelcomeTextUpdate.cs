using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class WelcomeTextUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE [dbo].[SystemSettings]
                                   SET [Value] = '<p>Your Account Dashboard has been improved to help you easily manage your saved jobs, recommendations, alerts and saved career and industry profiles. You can also manage your Personal Settings by selecting Manage Account.</p>

<p>Take note of the following changes:</p>

<ol>
<li>Set your Job Alerts to recommend job postings based on your preferences. You can access them from your Account Profile and receive them via email.</li>
<li>Save your job searches and access them later from the Job Alerts section.</li>
<li>The Saved Employers and Apprentice Match features are no longer available.</li>
<li>Visit the new Recommended Resources below to help you with your job search and career exploration journey.</li>
</ol>'
                                      ,[ModifiedByAdminUserId] = 1
                                      ,[DateUpdated] = GetDate()
                                      ,[DefaultValue] = '<p>Your Account Dashboard has been improved to help you easily manage your saved jobs, recommendations, alerts and saved career and industry profiles. You can also manage your Personal Settings by selecting Manage Account.</p>

<p>Take note of the following changes:</p>

<ol>
<li>Set your Job Alerts to recommend job postings based on your preferences. You can access them from your Account Profile and receive them via email.</li>
<li>Save your job searches and access them later from the Job Alerts section.</li>
<li>The Saved Employers and Apprentice Match features are no longer available.</li>
<li>Visit the new Recommended Resources below to help you with your job search and career exploration journey.</li>
</ol>'
                                   WHERE [Name] = 'jbAccount.dashboard.newAccountMessageBody'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
