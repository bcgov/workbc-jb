using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // updates to system settings that were edited manually by the content team on the test environment.  
            // this will ensure that these settings are migrated to prod.

            //jbAccount.dashboard.newAccountMessageBody

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<ul>
<li>Access Recommended Resources to assist you with your job search or career exploration journey.</li>
<li>Create (or re-create) your Job Alerts to help you find jobs that match your preferences.</li>
<li>Save your favourite postings to generate Recommended Jobs, which you can access from your Account Profile or by email.</li>
</ul>',
[DefaultValue] = '<ul>
<li>Access Recommended Resources to assist you with your job search or career exploration journey.</li>
<li>Create (or re-create) your Job Alerts to help you find jobs that match your preferences.</li>
<li>Save your favourite postings to generate Recommended Jobs, which you can access from your Account Profile or by email.</li>
</ul>'
WHERE [Name] = 'jbAccount.dashboard.newAccountMessageBody'");

            //jbAccount.dashboard.newAccountMessageTitle
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Your Account Profile has changed!',
[DefaultValue] = 'Your Account Profile has changed!'
WHERE [Name] = 'jbAccount.dashboard.newAccountMessageTitle'");

            //jbAccount.errors.emptyPassword
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Please enter your password.',
[DefaultValue] = 'Please enter your password.'
WHERE [Name] = 'jbAccount.errors.emptyPassword'");

            //jbAccount.errors.termsOfUseRequired
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'You must agree to the WorkBC Terms of Use before continuing.',
[DefaultValue] = 'You must agree to the WorkBC Terms of Use before continuing.'
WHERE [Name] = 'jbAccount.errors.termsOfUseRequired'");

            //shared.errors.jobAlertTitleDuplicate
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'The title already exists.',
[DefaultValue] = 'The title already exists.'
WHERE [Name] = 'shared.errors.jobAlertTitleDuplicate'");

            //shared.filters.educationNote
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<i>Note: You may see fewer results than you were expecting because an algorithm is used to get minimum education levels for job postings that are external to WorkBC.ca. If you see this, try removing the filter.</i>',
[DefaultValue] = '<i>Note: You may see fewer results than you were expecting because an algorithm is used to get minimum education levels for job postings that are external to WorkBC.ca. If you see this, try removing the filter.</i>'
WHERE [Name] = 'shared.filters.educationNote'");

            //shared.tooltips.unknownSalaries
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'Use this filter to include jobs that do not report a salary.',
[DefaultValue] = 'Use this filter to include jobs that do not report a salary.'
WHERE [Name] = 'shared.tooltips.unknownSalaries'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
