using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateConfirmationEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string registerBodyHtml = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
 <head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  <title>{4}</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
</head>
<body style=""margin: 0; padding: 0;"">
 <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
  <tr>
   <td>
    Hello {0} {1},<br><br>
    Thank you for registering on WorkBC! Your account username is: {2}.
    <br><br>
    You will need to activate your account before you can login by clicking on the link below:<br><br>
    <a href=""{3}"" target=""_blank"">Activate my WorkBC Account</a><br><br>
    Sincerely,<br><br>
    The WorkBC Team<br>
    <a href=""https://www.workbc.ca"" target=""_blank"">https://www.workbc.ca</a><br><br>
    Please disregard this message if you received it in error.
    </td>
  </tr>
 </table>
</body>
</html>";

            string registerBodyText = @"Hello {0} {1},

Thank you for registering on WorkBC! Your account username is: {2}.

To verify your account please click this link, or copy and paste the URL into your browser: 

{3}

Sincerely,

The WorkBC Team
https://www.workbc.ca

Please disregard this message if you received it in error.";


            migrationBuilder.Sql(
                 $"UPDATE [dbo].[SystemSettings] SET [Value] = '{registerBodyHtml}' WHERE [Name] = 'email.registration.bodyHtml'");
            migrationBuilder.Sql(
                $"UPDATE [dbo].[SystemSettings] SET [Value] = '{registerBodyText}' WHERE [Name] = 'email.registration.bodyText'");

            // set appropriate defaults for notification
            migrationBuilder.Sql(
                "UPDATE [dbo].[SystemSettings] SET [Value] = '', Description = 'This is a mandatory component of Notification #1. It appears in bold at the top of the message. It can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification1Title'");
            migrationBuilder.Sql(
                "UPDATE [dbo].[SystemSettings] SET [Value] = '', Description = 'Notification #1 appears at the top of the account dashboard and below the New Account Message.  If Notification #1 is enabled, the message will appear in a specific browser until the user dismisses the message. It will re-appear if the message is changed or the user logs in from a different browser. This field can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification1Body'");
            migrationBuilder.Sql(
                "UPDATE [dbo].[SystemSettings] SET [Value] = '0', Description = 'Toggle to turn Notification #1 on or off' WHERE [Name] = 'jbAccount.dashboard.notification1Enabled'");

            migrationBuilder.Sql(
                "UPDATE [dbo].[SystemSettings] SET [Value] = '', Description = 'This is a mandatory component of Notification #2. It appears in bold at the top of the message. It can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification2Title'");
            migrationBuilder.Sql(
                "UPDATE [dbo].[SystemSettings] SET [Value] = '', Description = 'Notification #2 appears at the top of the account dashboard and below Notification #1.  If Notification #1 is enabled, the message will appear in a specific browser until the user dismisses the message. It will re-appear if the message is changed or the user logs in from a different browser. This field can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification2Body'");
            migrationBuilder.Sql(
                "UPDATE [dbo].[SystemSettings] SET [Value] = '0', Description = 'Toggle to turn Notification #2 on or off' WHERE [Name] = 'jbAccount.dashboard.notification2Enabled'");

            var welcomeTitle = @"Welcome to WorkBC''s job board!";

            var welcomeHtml = @"<p>Here is what''s new with the website:</p>
<ol>
<li>We have made improvements to your Job Alerts. Your Job Alerts will continue to recommend job postings to you based on your selected preferences, but you can now access them from your Dashboard menu and receive them via email.</li>
<li>If you are a returning user, your Saved Searches have been merged into the enhanced Job Alerts section of your account.</li>
</ol>";

            migrationBuilder.Sql(
                $"UPDATE [dbo].[SystemSettings] SET [Value] = '{welcomeTitle}', [Description] = 'This is a mandatory component of the New Account Message. It appears in bold at the top of the message. It can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.newAccountMessageTitle'");

            migrationBuilder.Sql(
                $"UPDATE [dbo].[SystemSettings] SET [Value] = '{welcomeHtml}', [Description] = 'The New Account Message will appear at the top of the account dashboard the first time a user logs into the Job Board or when a user logs in from a new browser. It will continue to appear until the user dismisses the message. This field can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.newAccountMessageBody'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
