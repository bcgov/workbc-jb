using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class EmailsToSystemSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM SystemSettings where [name] = 'email.jobAlert.body' OR [name] = 'email.jobAlert.bodyText'");

            string passwordResetBodyHtml =
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n <head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n  <title>{3}</title>\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>\r\n</head>\r\n<body style=\"margin: 0; padding: 0;\">\r\n <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n  <tr>\r\n   <td>\r\n    Hello {0} {1},<br><br>\r\n    You have requested a password reset from your account. In order to complete the process and choose a new password for your account, follow the link below:\r\n    <br><br>\r\n    <a href=\"{2}\" target=\"_blank\">Reset the password on my account</a><br><br>\r\n    Sincerely,<br><br>\r\n    The WorkBC Team<br>\r\n    <a href=\"https://www.workbc.ca\" target=\"_blank\">https://www.workbc.ca</a><br><br>\r\n    Please disregard this message if you received it in error.\r\n   </td>\r\n  </tr>\r\n </table>\r\n</body>\r\n</html>\r\n";

            string passwordResetBodyText = "Hello {0} {1},\r\n\r\nYou have requested a password reset from your account.\r\n\r\nPlease click the following WorkBC link to complete the password reset process:\r\n\r\n{2}\r\n\r\nSincerely,\r\n\r\nThe WorkBC Team\r\nhttps://www.workbc.ca\r\n\r\nPlease disregard this message if you received it in error.";

            migrationBuilder.Sql(
                $"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.passwordReset.bodyHtml','{passwordResetBodyHtml}','Password reset email template / HTML version',5,1,GETDATE())");

            migrationBuilder.Sql(
                $"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.passwordReset.bodyText','{passwordResetBodyText}','Password reset email template / plain text version',2,1,GETDATE())");
            
            string registerBodyHtml = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n <head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n  <title>{3}</title>\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>\r\n</head>\r\n<body style=\"margin: 0; padding: 0;\">\r\n <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n  <tr>\r\n   <td>\r\nHello {0},<br><br>\r\nThank you for registering on WorkBC! Your account username is: {1}.\r\n<br><br>\r\nTo verify your account please click this link, or copy and paste the URL into your browser:<br>\r\n<a href=\"{2}\" target=\"_blank\">{2}</a>\r\n<br><br>\r\nIf you received this message without signing up please disregard.\r\n   </td>\r\n  </tr>\r\n </table>\r\n</body>\r\n</html>";
            string registerBodyText = "Hello {0},\r\n\r\nThank you for registering on WorkBC! Your account username is: {1}.\r\n\r\nTo verify your account please click this link, or copy and paste the URL into your browser: \r\n\r\n{2}\r\n\r\nIf you received this message without signing up please disregard.";
            
            migrationBuilder.Sql(
                 $"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.registration.bodyHtml','{registerBodyHtml}','Registration confirmation email template / HTML version',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                 $"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.registration.bodyText','{registerBodyText}','Registration confirmation email template / plain text version',2,1,GETDATE())");
            
            string jobAlertEmailHtml = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n <head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n  <title>{4}</title>\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>\r\n</head>\r\n<body style=\"margin: 0; padding: 0;\">\r\n <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n  <tr>\r\n   <td>\r\nHello {0},<br>\r\n<br>\r\nYour {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:<br>\r\n<br>\r\n{3}<br>\r\n<br>\r\nYour job alerts will only be sent when new job opportunities are available. You can edit or delete your job alert by signing in to your account on WorkBC.<br>\r\n<br>\r\nPlease disregard this message if you received it in error.\r\n   </td>\r\n  </tr>\r\n </table>\r\n</body>\r\n</html>";
            string jobAlertEmailText = "Hello {0},\r\n\r\nYour {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:\r\n\r\n{3}\r\n\r\nYour job alerts will only be sent when new job opportunities are available. You can edit or delete your job alert by signing in to your account on WorkBC.\r\n\r\nPlease disregard this message if you received it in error.";
            
            migrationBuilder.Sql(
                $"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.jobAlert.bodyHtml','{jobAlertEmailHtml}','Job alert email template / HTML version',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                $"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.jobAlert.bodyText','{jobAlertEmailText}','Job alert email template / plain text version',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                "INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.jobAlert.subject','Job Alert for {0} - NEW Job(s) Available!','Job alert email subject',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                "INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.registration.subject','Your WorkBC Account Requires Activation','Registration confirmation email subject',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                "INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('email.passwordReset.subject','WorkBC Password Reset','Password reset email subject',1,1,GETDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

