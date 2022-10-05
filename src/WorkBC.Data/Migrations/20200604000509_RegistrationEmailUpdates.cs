using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RegistrationEmailUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update SystemSettings set [value] = 'Your WorkBC.ca Account Requires Activation' where [name] = 'email.registration.subject'");

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
    Thank you for registering on WorkBC.ca! Your account username is: {2}.
    <br><br>
    You will need to activate your account before you can login by clicking on the link below:<br><br>
    <a href=""{3}"" target=""_blank"">Activate my WorkBC.ca Account</a><br><br>
    Thank you,<br><br>
    The WorkBC.ca Team<br>
    <a href=""https://www.workbc.ca"" target=""_blank"">https://www.workbc.ca</a>
    </td>
  </tr>
 </table>
</body>
</html>";

            string registerBodyText = @"Hello {0} {1},

Thank you for registering on WorkBC.ca! Your account username is: {2}.

You will need to activate your account before you can login by clicking on the link below:

{3}

Thank you,

The WorkBC.ca Team
https://www.workbc.ca";

            migrationBuilder.Sql(
                $"UPDATE [dbo].[SystemSettings] SET [Value] = '{registerBodyHtml}' WHERE [Name] = 'email.registration.bodyHtml'");

            migrationBuilder.Sql(
                $"UPDATE [dbo].[SystemSettings] SET [Value] = '{registerBodyText}' WHERE [Name] = 'email.registration.bodyText'");

            migrationBuilder.Sql(
    @"UPDATE [dbo].[SystemSettings] SET [Value] = '<p>
    <strong>
        An activation email has been sent to {0}. Please follow the instructions in that email to activate your account.
    </strong>
</p>
<p>
    <i>
        If you have not received the activation email, try checking your junk folder. 
        If you use a spam or security filter for your emails, make sure to set it to allow messages from noreply@gov.bc.ca. 
        To have the activation email resent, use the button below:
    </i>
</p>'  WHERE [name] = 'jbAccount.registration.confirmationBody'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
