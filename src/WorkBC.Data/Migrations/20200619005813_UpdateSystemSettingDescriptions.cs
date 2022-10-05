using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateSystemSettingDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update SystemSettings set [Description] = 'Registration confirmation email template / HTML version. {0} = firstName, {1} = lastName, {2} = email, {3} = linkUrl, {4} = emailSubject.' where [name] = 'email.registration.bodyHtml'");
            migrationBuilder.Sql("update SystemSettings set [Description] = 'Registration confirmation email template / plain text version. {0} = firstName, {1} = lastName, {2} = email, {3} = linkUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.' where [name] = 'email.registration.bodyText'");
            migrationBuilder.Sql("update SystemSettings set [Description] = 'Job alert email template / plain text version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.' where [name] = 'email.jobAlert.bodyText'");
            migrationBuilder.Sql("update SystemSettings set [Description] = 'Password reset email template / plain text version. {0} = firstName, {1} = lastName, {2} = linkUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.' where [name] = 'email.passwordReset.bodyText'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
