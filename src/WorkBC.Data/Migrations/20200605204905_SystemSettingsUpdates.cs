using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update SystemSettings set value = 'Please enter a valid email address.' where name = 'jbAccount.errors.invalidEmail'");

            migrationBuilder.Sql(
                "update SystemSettings set value = 'WorkBC.ca Password Reset' where name = 'email.passwordReset.subject'");

            migrationBuilder.Sql(
                @"UPDATE SYSTEMSETTINGS SET [Value] = '<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
 <head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  <title>{3}</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
</head>
<body style=""margin: 0; padding: 0;"">
 <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
  <tr>
   <td>
    Hello {0},<br><br>
    You''ve requested a password reset for your WorkBC.ca account. Follow the link below to choose a new password and complete the process.
    <br><br>
    <a href=""{2}"" target=""_blank"">Reset password</a><br><br>
    Thank you,<br><br>
    The WorkBC.ca Team<br>
    <a href=""https://www.workbc.ca"" target=""_blank"">www.workbc.ca</a>
   </td>
  </tr>
 </table>
</body>
</html>' WHERE [NAME] = 'email.passwordReset.bodyHtml'");

            migrationBuilder.Sql(@"UPDATE SYSTEMSETTINGS SET [Value] = 'Hello {0},

You''ve requested a password reset for your WorkBC.ca account. Follow the link below to choose a new password and complete the process.

{2}

Thank you,

The WorkBC.ca Team
https://www.workbc.ca' WHERE [NAME] = 'email.passwordReset.bodyText'");


            migrationBuilder.Sql("UPDATE SystemSettings SET [Value] = 'To reset your password, enter the email address you use to log into your account.' where [name] = 'jbAccount.login.forgotPasswordIntroText'");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}