using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateForgotPasswordEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
    You''ve requested a password reset for your WorkBC account. Follow the link below to choose a new password and complete the process.
    <br><br>
    <a href=""{2}"" target=""_blank"">Reset password</a><br><br>
    Sincerely,<br><br>
    The WorkBC Team<br>
    <a href=""https://www.workbc.ca"" target=""_blank"">www.workbc.ca</a><br><br>
    Please disregard this message if you received it in error.
   </td>
  </tr>
 </table>
</body>
</html>' WHERE [NAME] = 'email.passwordReset.bodyHtml'");

            migrationBuilder.Sql(@"UPDATE SYSTEMSETTINGS SET [Value] = 'Hello {0},

You''ve requested a password reset for your WorkBC account. Follow the link below to choose a new password and complete the process.

{2}

Sincerely,

The WorkBC Team
https://www.workbc.ca

Please disregard this message if you received it in error.' WHERE [NAME] = 'email.passwordReset.bodyText'");


            migrationBuilder.Sql(@"UPDATE SYSTEMSETTINGS SET [VALUE] = 
'<h3 class=""mb-3"">Invalid Link</h3>
<p>This password reset link has now expired. Please <a href=""#/login""><u>click here</u></a> to login.</p>' WHERE [Name] = 'jbAccount.errors.forgotPasswordInvalidToken'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}