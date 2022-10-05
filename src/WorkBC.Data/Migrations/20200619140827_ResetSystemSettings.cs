using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ResetSystemSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM [dbo].[SystemSettings]
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.jobAlert.bodyHtml', N'<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
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
Hello {0},<br>
<br>
Your {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:<br>
<br>
{3}<br>
<br>
Your job alerts will only be sent when new job opportunities are available. You can edit or delete your job alert by signing in to your account on WorkBC.<br>
<br>
Please disregard this message if you received it in error.
   </td>
  </tr>
 </table>
</body>
</html>', N'Job alert email template / HTML version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl, {4} = emailSubject.', 5, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.jobAlert.bodyText', N'Hello {0},

Your {1} job alert for {2} is ready for viewing. Please click the link below, or copy and paste the URL into your browser:

{3}

Your job alerts will only be sent when new job opportunities are available. You can edit or delete your job alert by signing in to your account on WorkBC.

Please disregard this message if you received it in error.', N'Job alert email template / plain text version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.', 2, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.jobAlert.subject', N'Job Alert for {0} - NEW Job(s) Available!', N'Job alert email subject. {0} = jobAlertTitle.', 1, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.passwordReset.bodyHtml', N'<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
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
</html>', N'Password reset email template / HTML version. {0} = firstName, {1} = lastName, {2} = linkUrl, {3} = emailSubject.', 5, 1, CAST(N'2020-06-19T06:54:37.6966667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.passwordReset.bodyText', N'Hello {0},

You''ve requested a password reset for your WorkBC.ca account. Follow the link below to choose a new password and complete the process.

{2}

Thank you,

The WorkBC.ca Team
https://www.workbc.ca', N'Password reset email template / plain text version. {0} = firstName, {1} = lastName, {2} = linkUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.', 2, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.passwordReset.subject', N'WorkBC.ca Password Reset', N'Password reset email subject', 1, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.registration.bodyHtml', N'<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
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
</html>', N'Registration confirmation email template / HTML version. {0} = firstName, {1} = lastName, {2} = email, {3} = linkUrl, {4} = emailSubject.', 5, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.registration.bodyText', N'Hello {0} {1},

Thank you for registering on WorkBC.ca! Your account username is: {2}.

You will need to activate your account before you can login by clicking on the link below:

{3}

Thank you,

The WorkBC.ca Team
https://www.workbc.ca', N'Registration confirmation email template / plain text version. {0} = firstName, {1} = lastName, {2} = email, {3} = linkUrl. The Job Board sends multi-part emails. Only a very small number of devices require these plain text emails, but including them helps to keep our messages from being flagged as spam.', 2, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'email.registration.subject', N'Your WorkBC.ca Account Requires Activation', N'Registration confirmation email subject', 1, 1, CAST(N'2020-06-19T06:54:37.7000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.careerProfiles.callToAction1BodyText', N'Discover over 500 career options and learn about the duties, salary, education, job prospects and much more.', N'Description for the 1st call-to-action on the saved career profiles page', 2, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.careerProfiles.callToAction1LinkText', N'Search Career Profiles', N'Link text for the 1st call-to-action on the saved career profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.careerProfiles.callToAction1LinkUrl', N'/Jobs-Careers/Explore-Careers.aspx', N'Link URL for the 1st call-to-action on the saved career profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.careerProfiles.callToAction2BodyText', N'Learn about the kinds of jobs and skills that will be most in demand for the next 10 years.', N'Description for the 2nd call-to-action on the saved career profiles page', 2, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.careerProfiles.callToAction2LinkText', N'Explore the Labour Market Outlook', N'Link text for the 2nd call-to-action on the saved career profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.careerProfiles.callToAction2LinkUrl', N'/Labour-Market-Industry/Labour-Market-Outlook.aspx', N'Link URL for the 2nd call-to-action on the saved career profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.accountDescription', N'Keep your account up-to-date. Access and manage your personal settings.', N'Text for the Manage Account widget on the account dashboard page', 2, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.careersDescription', N'Explore and save your favourite career and industry profiles to help you decide on the right career path for you.', N'Text for the Careers & Industries widget on the account dashboard page', 2, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.introText', N'Manage and personalize the job search process in your account. 
<br>
Find job opportunities that match your skills and experience, and learn about careers and industries in B.C.', N'Text shown at the top of the account dashboard page', 5, 1, CAST(N'2020-06-19T06:54:41.0000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.jobsDescription', N'Stay organized by saving your favourite jobs and employers. You can view and manage then without having to run a search.', N'Text for the Jobs widget on the account dashboard page', 2, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.newAccountMessageBody', N'<p>Here is what''s new with the website:</p>
<ol>
<li>We have made improvements to your Job Alerts. Your Job Alerts will continue to recommend job postings to you based on your selected preferences, but you can now access them from your Dashboard menu and receive them via email.</li>
<li>If you are a returning user, your Saved Searches have been merged into the enhanced Job Alerts section of your account.</li>
</ol>', N'The New Account Message will appear at the top of the account dashboard the first time a user logs into the Job Board or when a user logs in from a new browser. It will continue to appear until the user dismisses the message. This field can contain HTML or plain text.', 5, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.newAccountMessageTitle', N'Welcome to WorkBC''s job board!', N'This is a mandatory component of the New Account Message. It appears in bold at the top of the message. It can contain HTML or plain text.', 5, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.notification1Body', N'', N'Notification #1 appears at the top of the account dashboard and below the New Account Message. If Notification #1 is enabled, the message will appear in a specific browser until the user dismisses the message. It will re-appear if the message is changed or the user logs in from a different browser. This field can contain HTML or plain text.', 5, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.notification1Enabled', N'0', N'Toggle to turn Notification #1 on or off', 4, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.notification1Title', N'', N'This is a mandatory component of Notification #1. It appears in bold at the top of the message. It can contain HTML or plain text.', 5, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.notification2Body', N'', N'Notification #2 appears at the top of the account dashboard and below Notification #1. If Notification #1 is enabled, the message will appear in a specific browser until the user dismisses the message. It will re-appear if the message is changed or the user logs in from a different browser. This field can contain HTML or plain text.', 5, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.notification2Enabled', N'0', N'Toggle to turn Notification #2 on or off', 4, 1, CAST(N'2020-06-19T06:54:38.7466667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.notification2Title', N'', N'This is a mandatory component of Notification #2. It appears in bold at the top of the message. It can contain HTML or plain text.', 5, 1, CAST(N'2020-06-19T06:54:38.7433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource1Body', N'The occupations that are expected to have above-average opportunities are highlighted in this section.', N'Description for the 1st recommended resource at the bottom of the account dashboard page', 2, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource1Title', N'View high demand occupations', N'Title for the 1st recommended resource at the bottom of the account dashboard page', 1, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource1Url', N'/Labour-Market-Industry/High-Opportunity-Occupations.aspx', N'URL for the 1st recommended resource at the bottom of the account dashboard page', 1, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource2Body', N'WorkBC Employment Services Centres support you in finding a job and keeping it.', N'Description for the 2nd recommended resource at the bottom of the account dashboard page', 2, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource2Title', N'Visit a WorkBC Centre', N'Title for the 2nd recommended resource at the bottom of the account dashboard page', 1, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource2Url', N'/Employment-Services/WorkBC-Centres.aspx', N'URL for the 2nd recommended resource at the bottom of the account dashboard page', 1, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource3Body', N'Find labour market details on each of the seven regions in B.C., as well as the employment outlooks for the years to come.', N'Description for the 3rd recommended resource at the bottom of the account dashboard page', 2, 1, CAST(N'2020-06-19T06:54:41.0066667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource3Title', N'Explore Regional Profiles', N'Title for the 3rd recommended resource at the bottom of the account dashboard page', 1, 1, CAST(N'2020-06-19T06:54:41.0033333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.dashboard.resource3Url', N'/Labour-Market-Industry/Regional-Profiles.aspx', N'URL for the 3rd recommended resource at the bottom of the account dashboard page', 1, 1, CAST(N'2020-06-19T06:54:41.0066667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.emptyPassword', N'Please enter password', N'Error message shown on the login page when the password field is blank', 1, 1, CAST(N'2020-06-19T06:54:40.8566667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.forgotPasswordEmailNotFound', N'<strong>Ooops! The email address could not be found.</strong>
<div>Try again or create a new account.</div>', N'Error message for password reset shown when a user account does not exist for the email address entered', 5, 1, CAST(N'2020-06-19T06:54:40.7966667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.forgotPasswordInvalidToken', N'<h3 class=""mb-3"">Invalid Link</h3>
<p>This password reset link has now expired. Please <a href=""#/login""><u>click here</u></a> to login.</p>', N'Error message for an invalid password reset token', 5, 1, CAST(N'2020-06-19T06:54:40.7966667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.invalidEmail', N'Please enter a valid email address.', N'Error message shown on the login and password reset screens when an invalid email address is entered', 1, 1, CAST(N'2020-06-19T06:54:40.7933333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.loginFailed', N'The password you entered is incorrect', N'Error message shown on the login page for incorrect password', 1, 1, CAST(N'2020-06-19T06:54:40.7933333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes', N'<h4 class=""mt-4"">There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
<li>Clear all your filters</li>
<li>Add the filters back one at a time</li>
<li>Click the Apply Filter button after every selection</li>
</ul>', N'Error message shown on the recommended jobs page if zero results are returned and the user has checked multiple filter checkboxes', 5, 1, CAST(N'2020-06-19T06:54:40.7966667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.recommendedJobsNoResultsOneCheckbox', N'<h4 class=""mt-4"">There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
<li>Clear all your filters</li>
<li >Select a different filter</li>
</ul>', N'Error message shown on the recommended jobs page if zero results are returned and the user has only checked one filter checkboxes', 5, 1, CAST(N'2020-06-19T06:54:40.7966667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.errors.termsOfUseRequired', N'You must agree to the WorkBC Terms of Use before continuing', N'Error message shown on the registration page if the user does not accept the terms of use', 1, 1, CAST(N'2020-06-19T06:54:40.7966667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.industryProfiles.callToAction1BodyText', N'See data for B.C.''s major industries, including employment trends, earning potential and more.', N'Description for the 1st call-to-action on the saved industry profiles page', 2, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.industryProfiles.callToAction1LinkText', N'View Industry Profiles', N'Link text for the 1st call-to-action on the saved industry profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.industryProfiles.callToAction1LinkUrl', N'/Labour-Market-Industry/Industry-and-Sector-Information/Industry-Profiles.aspx', N'Link URL for the 1st call-to-action on the saved industry profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.industryProfiles.callToAction2BodyText', N'Use the sub-industry outlooks to identify trends and opportunities, and help you make career decisions.', N'Description for the 2nd call-to-action on the saved industry profiles page', 2, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.industryProfiles.callToAction2LinkText', N'Explore Industry and Sector Outlooks', N'Link text for the 2nd call-to-action on the saved industry profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.industryProfiles.callToAction2LinkUrl', N'/Labour-Market-Industry/Industry-and-Sector-Information/Industry-and-Sector-Outlooks.aspx', N'Link URL for the 2nd call-to-action on the saved industry profiles page', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.jobAlerts.noEmailHelpAnswer', N'<p class=""""pl-2"""">
    If you''ve turned your email notifications off, you will need to log in to your account and navigate to Job Alerts to view jobs that match an alert you''ve created. Once you''re logged in, you can access Job Alerts in two ways:
</p>
<ul>
    <li>Click on <strong>Job Alerts</strong> under the <em>Jobs</em> section in your Account Dashboard</li>
    <li> From the top navigation in your profile, hover your mouse over the <em>Jobs</em> menu and click on <strong>Job Alerts</strong></li>
 </ul>', N'Explanatory text on the create/edit job alert page for users who select an email alert frequency of ""Never""', 5, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.jobAlerts.noEmailHelpQuestion', N' Where do I find my Job Alerts if email notifications are turned off? ', N'Title for the explanatory text on the create/edit job alert page for users who select an email alert frequency of ""Never""', 1, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.login.forgotPasswordConfirmationBody', N'<p><strong>We''ve sent your password reset instructions to {0}.</strong></p>
<p><i>If you cannot find the email, try checking your junk folder. If you use a spam or security filter, change your email settings to allow messages from noreply@gov.bc.ca.</i></p>
<p><i>For help, contact the WorkBC.ca call centre at: <span class=""text-nowrap"">250-952-6914</span> or toll free at: <span class=""text-nowrap"">1-877-952-6914.</span></i></p>', N'The body of the message displayed after a user resets their password. Placeholder {0} is for the user''s email address.', 5, 1, CAST(N'2020-06-19T06:54:42.7266667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.login.forgotPasswordConfirmationTitle', N'Please check your email.', N'The title of the message displayed after a user resets their password.', 1, 1, CAST(N'2020-06-19T06:54:42.7266667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.login.forgotPasswordIntroText', N'To reset your password, enter the email address you use to log into your account.', N'Introductory text on the reset password dialog', 1, 1, CAST(N'2020-06-19T06:54:41.0233333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.recommendedJobs.filterIntroText ', N'You can also filter your recommended jobs by adjusting the reason for the recommendations below.', N'Explanatory text at the top of the recommended jobs filter', 1, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.recommendedJobs.introText', N'<p>Welcome to your recommended jobs! Jobs are suggested to you if they:</p>
<ul>
    <li>have the same job title as one of your saved jobs</li>
    <li>have the same <a href=''http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16'' target=''_blank''>NOC</a> code as one of your saved jobs</li>
    <li>have the same employer one of your saved jobs</li>
    <li>are in the same city as you specified in your <em>Personal Settings</em>, or if</li>
    <li>an employer has chosen to encourage applicants from a group you have self-identified as in your <em>Personal Settings</em>. </li>
</ul>
<p>To make sure the job recommendations that come up are the most meaningful to you, we have not included jobs that you have already saved in your profile.</p>', N'Introductory text that appears at the top of the recommended jobs page', 5, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.recommendedJobs.introTextNoRecommendedJobs', N'<p>You do not have any recommended jobs yet. Jobs are suggested to you if they:</p>
<ul>
    <li>have the same job title as one of your saved jobs</li>
    <li>have the same <a href=''http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16'' target=''_blank''>NOC</a> code as one of your saved jobs</li>
    <li>have the same employer one of your saved jobs</li>
    <li>are in the same city as you specified in your <em>Personal Settings</em>, or if</li>
    <li>an employer has chosen to encourage applicants from a group you have self-identified as in your <em>Personal Settings</em>. </li>
</ul>
<p class=''no-recommended-jobs''> To increase the number of recommendations, you can either <a href=''#/saved-jobs''>save a job</a>, <a href=''#/personal-settings#location''>change your city</a> or <a href=''#/personal-settings''>add group/s that you self-identify as</a> in your <em>Personal Settings</em>. </p>', N'Introductory text that appears at the top of the recommended jobs page for users with no recommended jobs', 5, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.registration.activationBody', N'You are now being redirected to log in or you can <a href=""#/login"">click here</a>.', N'The body of the message shown after a user clicks the email confirmation link to activate their account.', 5, 1, CAST(N'2020-06-19T06:54:41.0000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.registration.activationTitle', N'You have successfully activated your account!', N'The title of the message shown after a user clicks the email confirmation link to activate their account', 1, 1, CAST(N'2020-06-19T06:54:41.0000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.registration.confirmationBody', N'<p>
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
</p>', N'The body of the message displayed after a user completes the registration form.  Placeholder {0} is for the user''s email address.', 5, 1, CAST(N'2020-06-19T06:54:41.0000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.registration.confirmationTitle ', N'Thanks for registering!', N'The title of the message shown after a user completes the registration form', 1, 1, CAST(N'2020-06-19T06:54:41.0000000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.registration.termsOfUseText', N'<h4><strong>Introduction</strong></h4>
<p>The WorkBC website is provided by the Province of British Columbia (the ""Province"").</p>
<p>The WorkBC Job Board is British Columbia''s most comprehensive job board, where job seekers can search for and find jobs. You can apply to jobs directly with employers, receive job alerts when new positions are posted, manage your favourite jobs and more. The WorkBC Job Board service is available 24 hours per day, seven days a week, free of charge.</p>
<p>These Terms of Use set out the terms and conditions under which the WorkBC Job Board service is made available to you, the job seeker (""you"" or ""your""), to view job postings.</p>
<div style=""padding: 16px; border: 1px solid black; margin-bottom: 25px"">
    <p><strong>Disclaimer:</strong></p>
    <p style=""margin-bottom:4px""><strong>It is important that you carefully read and understand the Terms of Use. </strong>By clicking ""I Agree"" during the registration process and by accessing, using and/or viewing job postings on the WorkBC Job Board, you are confirming that you have read and understood the Terms of Use. You are also confirming your initial and ongoing acceptance and agreement to be bound by these Terms of Use.</p>
</div>
<h4><strong>How the Work BC Job Board service works and collection, use and disclosure of personal information</strong></h4>
<p>The job board is designed to display both job postings submitted to the Government of Canada''s National Job Bank by employers and other current online job postings that may be available. A job seeker may search any of these job postings and use the contact information provided by employers in those job postings to respond directly to employers for jobs that are of interest.</p>
<p>You may use the job board either as an anonymous job seeker (""anonymous user"") or, if you would like to take advantage of additional services, you may set up a user account and become a registered job seeker (""registered user""). In either case, you will be bound by thee Terms of Use, to the extent applicable.</p>
<p>Any personal information collected from you will be collected, used and disclosed in accordance with the <em>British Columbia Freedom of Information and Protection of Privacy Act </em>(""FOIPPA"") and for the purpose of providing you with access to job search resources. The authority to collect personal information comes from Section 26(c) of the FOIPPA. If you have any questions about the collection of your personal information, please contact: Manager, WorkBC Operations, <span class=""text-nowrap"">1-877-952-6914,</span> P.O. Box 9189 Stn Prov Govt, Victoria, B.C. V8W 9E6.</p>
<h5><strong>Anonymous user</strong></h5>
<p>You may use the job board to search current job postings without providing any personal information. However, as an Anonymous User, you will need to re-enter any job search parameters and preferences each time you access the WorkBC Job Board.</p>
<h5><strong>Registered user</strong></h5>
<p>To become a registered user, you must provide the following to create and access your account:</p>
<ul>
    <li>First and last name</li>
    <li>Location (country, province and city)</li>
    <li>Email address</li>
    <li>Password</li>
    <li>Security question and answer</li>
</ul>
<p>You must also supply a password to allow you to access your account. Upon request, your email address may be used to provide you with notices regarding available job postings that meet your search parameters and preferences. Your preferences will be stored within your user account and can only be accessed by you. Your email address may also be used if you forget your login information. As well, you may be notified by email if there are any significant changes made to the WorkBC Job Board that impact you.</p>
<p>If you are accessing your account or the WorkBC Job Board from outside Canada, you hereby consent to your personal information being accessed from outside of Canada. You also understand and agree that any non-personal information provided by you through your use of the job board may be used, in aggregate form only, for program review, analysis and reporting, and for statistical research purposes.</p>
<h5><strong>Voluntary participation</strong></h5>
<p>Participation is voluntary. If you do not provide your personal information, you will not be able to create an account on the WorkBC Job Board. You may still use the WorkBC Job Board as an anonymous user to search for jobs, but you will not be able to access features associated with having an account (i.e. create job alerts).</p>
<h4><strong>Job seeker obligations</strong></h4>
<p>You agree to not use the WorkBC Job Board service for any purpose other than to search available employment opportunities and, if desired, to respond to those employment opportunities, both solely for your personal purposes. You must not copy, scrape, link to, frame or use any data, materials or other information provided on the WorkBC Job Board for any other purpose whatsoever.</p>
<h4><strong>Amendments</strong></h4>
<p>The Province reserves the right to cease providing the WorkBC Job Board service at any time. The Province also reserves the right to modify these Terms of Use at any time without notice being provided directly to you.</p>
<p>You understand and agree that it is your responsibility to consult the Terms of Use on a regular basis to become aware of any modifications that have been made. If you do not agree to be bound by such modified terms, your sole remedy is to cease using the service.</p>
<h4><strong>Job postings</strong></h4>
<p>The job postings on the WorkBC Job Board come from two sources:</p>
<ol>
    <li>Jobs postings submitted to the Government of Canada''s National Job Bank by employers</li>
    <li>Other current online job postings that may be available</li>
</ol>
<p>The Province does not generally monitor or filter the content of the WorkBC Job Board job postings. However, it is the Province''s policy that any job posting that falls within any of the following categories must not be posted or may be removed:</p>
<ul>
    <li>Volunteer opportunities</li>
    <li>Job postings where there are no available openings at the time of posting</li>
    <li>Job postings containing inappropriate, offensive or illegal content, such as defamatory statements, inflammatory or discriminatory content or hiring practices based on gender, sexual orientation, age, religious affiliations, disability or ethnicity</li>
    <li>Any commercial solicitation and/or advertising other than for specific employment opportunities</li>
    <li>Job postings that require job seekers to participate in unpaid training as a condition of hiring</li>
    <li>Job postings for the recruitment of replacement workers during a labour dispute</li>
    <li>Job postings for employment opportunities based outside of British Columbia</li>
</ul>
<p>If you should encounter a job posting that falls into any of the above categories, please immediately contact the Province at <span class=""text-nowrap"">1-877-952-6914</span> and provide details of the offending posting so that it can be reviewed.</p>
<p>Notwithstanding the foregoing, the Province will not have any liability for any information posted by third parties, even if the Province has been advised of any offending content in such information.</p>
<p>Note: Job postings on the WorkBC Job Board have an initial maximum advertising period of 30 calendar days. However, the Province will not have any liability for the earlier or later termination of a job posting for any reason.</p>
<h4><strong>Condition of use</strong></h4>
<p>By accessing and using WorkBC Job Board you agree that your use of this site and the job board service is entirely at your own risk and that you will be liable for any failure to abide by these Terms of Use.</p>
<p>The Province has no obligation to provide, or continue to provide, the job board service to you and all functionalities of WorkBC Job Board are provided on an as available basis. Without limiting the generality of the foregoing and in addition to the <a href=""https://www2.gov.bc.ca/gov/content/home/disclaimer"" target=""_blank""><u>Warranty Disclaimer</u></a>, the Province makes no representations or warranties, express or implied, with respect to:</p>
<ul>
    <li>WorkBC Job Board being free of malware, including viruses or other harmful components</li>
    <li>How accurate, complete or current any information is available through the job board</li>
</ul>
<p>The Province is not responsible and assumes no liability with respect to any of the information provided to create an account with WorkBC Job Board or in relation to any job advertisement. The Province is not responsible for lost, intercepted, incomplete, illegible, misdirected or stolen messages or mail, unavailable connections, failed, incomplete, garbled or delayed transmissions, online failures, hardware, software or other technical malfunctions or disturbances.<br /> <br /> Although an individual must provide a valid business payroll account number and social insurance number in order to post a job advertisement on the Government of Canada''s National Job Bank (which are in turn displayed on the WorkBC Job Board), they may not be who they claim to be (especially in the case of job postings found on other sites and merely linked to by WorkBC Job Board). As such, you should exercise caution and only disclose personal information that is essential for replying to any job posting.</p>
<p>If any provision of these Terms of Use is declared by an arbitrator or a court of competent jurisdiction to be invalid, illegal or unenforceable, such provision shall be severed from these Terms of Use and all other provisions shall remain in full force and effect.</p>
<p>The laws in force in the Province of British Columbia will govern these Terms of Use and your use of the WorkBC Job Board. You agree that any action at law or in equity in any way arising from these Terms of Use or your use of the service will be resolved by arbitration under the <em>British Columbia Commercial Arbitration Act</em> and the place of arbitration will be Victoria, British Columbia.</p>
<p>WorkBC Job Board users agree to <em>not</em>:</p>
<ul>
    <li>Transmit, post, distribute, store or destroy material, including without limitation WorkBC Job Board content, in violation of any applicable law or regulation, including but not limited to laws or regulations governing the collection, processing, or transfer of personal information, or in breach of WorkBC''s privacy policy</li>
    <li>Take any action that imposes an unreasonable or disproportionately large load on the WorkBC Job Board''s infrastructure</li>
    <li>Use any device to navigate or search the WorkBC Job Board, other than the tools available on the Job Board, generally available third-party web browsers, or other tools approved by the WorkBC Job Board</li>
    <li>Use any data mining, robots or similar data gathering or extraction methods</li>
    <li>Violate or attempt to violate the security of the WorkBC Job Board, including attempting to probe, scan or test the vulnerability of a system or network or to breach security or authentication measures without proper authorization</li>
    <li>Share with a third-party any login credentials to the WorkBC Job Board</li>
    <li>Access data not intended for you or logging into a server or account which you are not authorized to access</li>
    <li>Post or submit to the WorkBC Job Board any incomplete, false or inaccurate biographical information or information which is not your own</li>
    <li>Solicit passwords or personally identifiable information from other users</li>
    <li>Delete or alter any material posted by any other person or entity</li>
    <li>Harass, incite harassment or advocate harassment of any group, company, or individual</li>
    <li>Attempt to interfere with service to any user, host or network, including, without limitation, via means of submitting a virus to the WorkBC Job Board, overloading, ""flooding,"" ""spamming,"" ""mailbombing"" or ""crashing""</li>
</ul>
<p>You are responsible for maintaining the confidentiality of your account, profile and passwords, as applicable. You may not share your password or other account access information with any other party, temporarily or permanently, and you shall be responsible for all uses of your registration and passwords, whether or not authorized by you. You agree to immediately notify the WorkBC Job Board of any unauthorized use of your account, profile or passwords.</p>
<h4><strong>Limitation of Liability and Indemnity</strong></h4>
<p>In addition to the Province''s general <a href=""https://www2.gov.bc.ca/gov/content/home/disclaimer"" target=""_blank""><u>Limitation of Liability</u></a>, under no circumstances will the Province be liable to any person or business entity for any direct, indirect, special, incidental, consequential or other damages based on: (i) any use or inability to use the Service, or (ii) any lack of availability or delay in using the Service, or the posting or removal of any job advertisement.</p>
<p>Any dispute at any time arising between you and an individual/employer posting a job shall be solely between you and them.</p>
<p>You agree to indemnify and hold harmless the Province and its successors, assigns, ministers, officers, employees and agents from any claim, action, demand, loss or damages, including lawyers'' fees, made or incurred by any third party in any way arising out of your use of the Service, including as between you and the individual/employer posting the job.</p>
<h4><strong>Termination of use</strong></h4>
<p>The Province may, without notice to you, temporarily or permanently terminate your access to WorkBC Job Board if:</p>
<ul>
    <li>You fail to comply with any of these terms and conditions</li>
    <li>If your user ID or password are compromised or insecure or are suspected of being compromised or insecure</li>
</ul>
<p>If at any time you no longer wish to agree to be bound by these Terms of Use or for any reason wish to terminate your use of the job board service, you must immediately cease accessing and using WorkBC Job Board. In addition, you must notify us so that we may delete your account.</p>
<p>The Province may at any time revoke your access to WorkBC Job Board if you use the Service otherwise than for legitimately searching and applying for employment opportunities for your own personal purposes only.</p>
<h4><strong>Security</strong></h4>
<p>You are responsible for keeping your WorkBC Job Board account login information, including password, secure. You must not share this information with any third party for any purpose.</p>
<h4><strong>Agreement to WorkBC Job Board''s Terms of Use</strong></h4>
<p>By clicking on ""I Agree"", you confirm that you have read, understood and agree to be bound by these Terms of Use for the WorkBC Job Board. By accessing and using the WorkBC Job Board you agree the job board service is used entirely at your own risk and accept that WorkBC Job Board is not liable for events outside of its control.</p>
<p>If you do not agree to be bound by these Terms of Use, you must not access this website or use the job board.</p>', N'HTML text for the terms of use', 5, 1, CAST(N'2020-06-19T06:54:37.6433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.registration.termsOfUseTitle', N'WorkBC Job Board''s Terms of Use', N'Title for the terms of use.', 1, 1, CAST(N'2020-06-19T06:54:37.6466667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.shared.passwordComplexity', N'Passwords must be at least six characters long, with at least one upper-case letter and one number.', N'Text explaining our password complexity rules', 1, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbAccount.shared.whyIdentify', N'<strong>Why identify?</strong>
<p>Some employers encourage applications from select groups. Choose the boxes that describe how you identify. You will receive an alert when matching jobs are posted.</p>', N'Explanatory text for the ""How do you identify?"" section of the registration and personal settings pages', 5, 1, CAST(N'2020-06-19T06:54:41.0200000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.errors.duplicateCity', N'The city <strong>{0}</strong> has already been added.', N'Error message shown when a user attempts to add the same city twice on the location filter', 5, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.errors.duplicatePostal', N'The postal code <strong>{0}</strong> has already been added.', N'Error message shown when a user attempts to add the same postal code twice on the location filter', 5, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.errors.invalidCity', N'The city <strong>{0}</strong> could not be found. Please ensure the spelling is correct.', N'Error message shown when a user enters an invalid city on the job search page', 5, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.errors.invalidPostalCode', N'The postal code <strong>{0}</strong> is invalid. Please ensure the format is correct.', N'Error message shown when a user enters an invalid postal code on the job search page', 5, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.errors.noSearchResults', N'<h4>There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
  <li>Check your spelling</li>
  <li>Try broader search terms</li>
  <li>Use different synonyms</li>
  <li>Replace abbreviations with the entire word</li>
</ul>', N'Error message shown when a search returns zero results', 5, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction1BodyText', N'Get the details you need on 500 jobs - including duties, education required, salary and employment outlook.', N'Description for the 1st call-to-action on the job detail page', 2, 1, CAST(N'2020-06-19T06:54:41.0066667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction1Intro', N'CAREER PROFILES', N'Introductory title for the 1st call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0066667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction1LinkText', N'View Career Profiles', N'Button text for the 1st call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0066667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction1LinkUrl', N'/Jobs-Careers/Explore-Careers.aspx', N'Link URL for the 1st call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction1Title', N'EXPLORE CAREERS', N'Main title for the 1st call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0066667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction2BodyText', N'Discover jobs that are expected to be in high demand over the next decade.', N'Description for the 2nd call-to-action on the job detail page', 2, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction2Intro', N'LABOUR MARKET INFORMATION', N'Introductory title for the 2nd call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction2LinkText', N'View High Demand Jobs', N'Button text for the 2nd call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction2LinkUrl', N'/Labour-Market-Industry/High-Demand-Occupations.aspx', N'Link URL for the 2nd call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.jobDetail.callToAction2Title', N'HIGH DEMAND JOBS', N'Main title for the 2nd call-to-action on the job detail page', 1, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'jbSearch.settings.newJobPeriodDays', N'3', N'A job is considered new if it was posted within this number of days.', 3, 1, CAST(N'2020-06-19T06:54:26.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.errors.jobAlertTitleDuplicate', N'The title already exists', N'Error message shown when a user tries to add a duplicate title to a job alert', 1, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.errors.jobAlertTitleRequired', N'Please specify a title for this alert', N'Error message for required job alert title', 1, 1, CAST(N'2020-06-19T06:54:40.6166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.datePostedTitle', N'Date Posted', N'Main title for the date posted filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.educationNote', N'<i>Note: You may see fewer results than you were expecting because an algorithm is used to get minimum education levels for job postings that are external to WorkBC.ca. If you see this, try removing the filter.</i>', N'Explanatory text that appears on the education filter', 5, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.educationTitle', N'Education', N'Main title for the education filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.howIsSalaryCalculatedBody', N'<p>When you select a value from the salary drop-down menu, that figure is automatically converted to the equivalent annual salary even when a pay period other than Annually is selected. </p>
<p>The conversion is calculated as follows:</p>
<ul>
    <li>Hourly - hourly rate is multiplied by 2080 (approximate number of work hours in a year)</li>
    <li>Weekly - weekly rate is multiplied by 52 (number of weeks in a year)</li>
    <li>Biweekly - biweekly rate is multiplied by 26 (half the number of weeks in a year)</li>
    <li>Monthly - monthly rate is multiplied by 12 (number of months in a year)</li>
</ul>
<p>Using this conversion, the search is able to return results matching your salary requirements regardless of whether you select Annually, Hourly, Weekly, Biweekly or Monthly as the pay period. The search will include all jobs posted with a salary equal to or higher than the amount selected.</p>
<p>Example: A user searches for salaries: $15 hourly. This will be converted by multiplying $15 by the conversion factor of 2080 (in this case, approximate number of work hours in a year)</p>
<p>Salary searches apply to jobs where employers publicly posted salary amounts (e.g., annual, monthly, biweekly, weekly or hourly). Note: Some employers do not specify a salary amount.</p>', N'Explanatory text that appears at the bottom of the salary filter', 5, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.howIsSalaryCalculatedTitle', N'How is salary calculated?', N'Title for the explanatory text at the bottom of the salary filter', 1, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.industryTitle', N'Industry', N'Main title for the industry filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.jobTypeTitle', N'Job type', N'Main title for the job type filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.keywordInputPlaceholder', N'Keyword(s)', N'Placeholder text for the main keyword search input', 1, 1, CAST(N'2020-06-19T06:54:42.7266667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.locationRadiusNote', N'<i>Note: You can select a search radius around a <strong>single city or postal code</strong>. The search radius doesn''t apply when you enter more than one location.</i>', N'Explanatory text that appears below the radius dropdown on the location filter', 5, 1, CAST(N'2020-06-19T06:54:41.0100000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.locationRegionSearchLabel', N'Narrow down by region', N'The label for the region checkboxes on the location filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.locationSearchLabel', N'Narrow down by city name or postal code', N'The label of the city or postal code search input on the location filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.locationTitle', N'Location', N'Main title for the location filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.moreFiltersTitle', N'More Filters', N'Main title for the more filters filter', 1, 1, CAST(N'2020-06-19T06:54:41.0166667' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.filters.salaryTitle', N'Salary and Benefits', N'Main title for the salary filter', 1, 1, CAST(N'2020-06-19T06:54:41.0133333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.settings.defaultSearchRadius', N'15', N'Default radius (km) for location searches. Valid values are 10, 15, 25, 50, 75 or 100. If you enter an invalid value then 15 will be used instead.', 3, 1, CAST(N'2020-06-19T06:54:40.6700000' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.tooltips.jobSource', N'WorkBC jobs are verified by the National Job Bank. External jobs are B.C. job postings from other job boards (For example Monster and Indeed).', N'Tooltip for Job Source in the more filters filter', 1, 1, CAST(N'2020-06-19T06:54:42.1433333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.tooltips.nocCode', N'The <a href=""http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16"" target=""_blank"" style=""text-decoration: underline;"">National Occupational Classification</a> system classifies all occupations in Canada.', N'Tooltip for 2016 NOC code in the more filters filter', 5, 1, CAST(N'2020-06-19T06:54:40.4933333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.tooltips.unknownSalaries', N'Use this filter to include jobs that do not report a salary', N'Tooltip for the unknown salaries option in the salary filter', 1, 1, CAST(N'2020-06-19T06:54:40.4933333' AS DateTime2))
GO
INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated]) VALUES (N'shared.tooltips.youth', N'Youth are people between 15 and 30 years of age.', N'Tooltip for the youth option in the more filters filter', 1, 1, CAST(N'2020-06-19T06:54:26.0200000' AS DateTime2))
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
