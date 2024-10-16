using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobAlertEmailUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM SystemSettings WHERE [Name] IN ('email.jobAlert.bodyHtml','email.jobAlert.bodyText','email.jobAlert.bodyHtml','shared.filters.educationTitle')

GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated], [DefaultValue]) VALUES (N'email.jobAlert.bodyHtml', N'<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/>
    <title>{4}</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
</head>
<body style=""margin: 0; padding: 0;"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
        <td>
        Hello {0},<br><br>

        Your {1} job alert for <strong>{2}</strong> is ready for viewing. Please
        click the link below, or copy and paste the URL into your browser:<br><br>

        {3}<br><br>

        Your job alerts will only be sent when new job opportunities are
        available. By
        <a href=""https://www.workbc.ca/Account.aspx#/login"" target=""_blank"">
            logging into your account</a>, you can:<br>

        <ul>
            <li>Edit a job alert if you’d like to make changes to it.</li>
            <li>Delete a job alert if you no longer wish to receive these emails.</li>
        </ul>
        Thank you,<br/><br/>
        The WorkBC.ca Team<br/>
        <a href=""https://www.workbc.ca"" target=""_blank"">www.workbc.ca</a>
        </td>
    </tr>
</table>
</body>
</html>', N'Job alert email template / HTML version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl, {4} = emailSubject.', 5, 1, CAST(N'2020-07-07T12:38:01.4238405' AS timestamp),
N'<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/>
    <title>{4}</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
</head>
<body style=""margin: 0; padding: 0;"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
        <td>
        Hello {0},<br><br>

        Your {1} job alert for <strong>{2}</strong> is ready for viewing. Please
        click the link below, or copy and paste the URL into your browser:<br><br>

        {3}<br><br>

        Your job alerts will only be sent when new job opportunities are
        available. By
        <a href=""https://www.workbc.ca/Account.aspx#/login"" target=""_blank"">
            logging into your account</a>, you can:<br>

        <ul>
            <li>Edit a job alert if you’d like to make changes to it.</li>
            <li>Delete a job alert if you no longer wish to receive these emails.</li>
        </ul>
        Thank you,<br/><br/>
        The WorkBC.ca Team<br/>
        <a href=""https://www.workbc.ca"" target=""_blank"">www.workbc.ca</a>
        </td>
    </tr>
</table>
</body>
</html>')
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated], [DefaultValue]) VALUES (N'email.jobAlert.bodyText',
N'Hello {0},

Your {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:

{3}

Your job alerts will only be sent when new job opportunities are available. By logging into your account, you can:
- Edit a job alert if you’d like to make changes to it.
- Delete a job alert if you no longer wish to receive these emails.

Thank you,

The WorkBC.ca Team
https://www.workbc.ca', N'Job alert email template / plain text version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.', 2, 1, CAST(N'2020-07-07T12:40:55.2847041' AS timestamp),
N'Hello {0},

Your {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:

{3}

Your job alerts will only be sent when new job opportunities are available. By logging into your account, you can:
- Edit a job alert if you’d like to make changes to it.
- Delete a job alert if you no longer wish to receive these emails.

Thank you,

The WorkBC.ca Team
https://www.workbc.ca')
GO

INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated], [DefaultValue]) VALUES (N'shared.filters.educationTitle', N'Education required', N'Main title for the education filter', 1, 1, CAST(N'2020-07-07T11:11:00.3220367' AS timestamp), N'Education')
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
