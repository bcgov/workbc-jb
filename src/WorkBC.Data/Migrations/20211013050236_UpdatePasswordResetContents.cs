using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdatePasswordResetContents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // updates to system settings to edit password reset contents

            // jbAccount.errors.forgotPasswordInvalidToken
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<div class=""mb-3"">Invalid Link</div>
<p>The password reset link has expired. If you would like to reset your password, you can <a href = ""#/login""><u>make a new request</u></a>.</p>' 
WHERE [Name] = 'jbAccount.errors.forgotPasswordInvalidToken'");

            // jbAccount.login.forgotPasswordConfirmationBody
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<p>We''ve sent your password reset instructions to {0}.</p><p>If you cannot find the email, try checking your junk folder.
If you use a spam or security filter, change your email settings to allow messages from noreply@gov.bc.ca.</p>
<p>For help, contact the WorkBC.ca call centre at: <span class=""text-nowrap"">250-952-6914</span> or toll free at: <span class=""text-nowrap"">1-877-952-6914.</span></p>' 
WHERE [Name] = 'jbAccount.login.forgotPasswordConfirmationBody'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
