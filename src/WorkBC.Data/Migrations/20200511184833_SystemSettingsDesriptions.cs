using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsDesriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Job alert email template / HTML version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl, {4} = emailSubject.' WHERE [Name] = 'email.jobAlert.bodyHtml'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Job alert email template / plain text version. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl.' WHERE [Name] = 'email.jobAlert.bodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Job alert email subject. {0} = jobAlertTitle.' WHERE [Name] = 'email.jobAlert.subject'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Password reset email template / HTML version. {0} = firstName, {1} = lastName, {2} = linkUrl, {3} = emailSubject.' WHERE [Name] = 'email.passwordReset.bodyHtml'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Password reset email template / plain text version. {0} = firstName, {1} = lastName, {2} = linkUrl.' WHERE [Name] = 'email.passwordReset.bodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Password reset email subject' WHERE [Name] = 'email.passwordReset.subject'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Registration confirmation email template / HTML version. {0} = firstName, {1} = linkUrl, {2} = emailSubject.' WHERE [Name] = 'email.registration.bodyHtml'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Registration confirmation email template / plain text version. {0} = firstName, {1} = linkUrl.' WHERE [Name] = 'email.registration.bodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Registration confirmation email subject' WHERE [Name] = 'email.registration.subject'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 1st call-to-action on the saved career profiles page' WHERE [Name] = 'jbAccount.careerProfiles.callToAction1BodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link text for the 1st call-to-action on the saved career profiles page' WHERE [Name] = 'jbAccount.careerProfiles.callToAction1LinkText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link URL for the 1st call-to-action on the saved career profiles page' WHERE [Name] = 'jbAccount.careerProfiles.callToAction1LinkUrl'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 2nd call-to-action on the saved career profiles page' WHERE [Name] = 'jbAccount.careerProfiles.callToAction2BodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link text for the 2nd call-to-action on the saved career profiles page' WHERE [Name] = 'jbAccount.careerProfiles.callToAction2LinkText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link URL for the 2nd call-to-action on the saved career profiles page' WHERE [Name] = 'jbAccount.careerProfiles.callToAction2LinkUrl'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Text for the Manage Account widget on the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.accountDescription'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Text for the Careers & Industries widget on the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.careersDescription'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Text shown at the top of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.introText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Text for the Jobs widget on the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.jobsDescription'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The New Account Message will appear at the top of the account dashboard the first time a user logs into the Job Board or when a user logs in from a new browser. It will continue to appear until the user dismisses the message. This field can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.newAccountMessageBody'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'This is a mandatory component of the New Account Message. It appears in bold at the top of the message. It can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.newAccountMessageTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Notification #1 appears at the top of the account dashboard and below the New Account Message. If Notification #1 is enabled, the message will appear in a specific browser until the user dismisses the message. It will re-appear if the message is changed or the user logs in from a different browser. This field can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification1Body'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Toggle to turn Notification #1 on or off' WHERE [Name] = 'jbAccount.dashboard.notification1Enabled'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'This is a mandatory component of Notification #1. It appears in bold at the top of the message. It can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification1Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Notification #2 appears at the top of the account dashboard and below Notification #1. If Notification #1 is enabled, the message will appear in a specific browser until the user dismisses the message. It will re-appear if the message is changed or the user logs in from a different browser. This field can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification2Body'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Toggle to turn Notification #2 on or off' WHERE [Name] = 'jbAccount.dashboard.notification2Enabled'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'This is a mandatory component of Notification #2. It appears in bold at the top of the message. It can contain HTML or plain text.' WHERE [Name] = 'jbAccount.dashboard.notification2Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 1st recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource1Body'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Title for the 1st recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource1Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'URL for the 1st recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource1Url'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 2nd recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource2Body'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Title for the 2nd recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource2Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'URL for the 2nd recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource2Url'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 3rd recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource3Body'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Title for the 3rd recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource3Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'URL for the 3rd recommended resource at the bottom of the account dashboard page' WHERE [Name] = 'jbAccount.dashboard.resource3Url'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown on the login page when the password field is blank' WHERE [Name] = 'jbAccount.errors.emptyPassword'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message for password reset shown when a user account does not exist for the email address entered' WHERE [Name] = 'jbAccount.errors.forgotPasswordEmailNotFound'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message for an invalid password reset token' WHERE [Name] = 'jbAccount.errors.forgotPasswordInvalidToken'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown on the login and password reset screens when an invalid email address is entered' WHERE [Name] = 'jbAccount.errors.invalidEmail'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown on the login page for incorrect password' WHERE [Name] = 'jbAccount.errors.loginFailed'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown on the recommended jobs page if zero results are returned and the user has checked multiple filter checkboxes' WHERE [Name] = 'jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown on the recommended jobs page if zero results are returned and the user has only checked one filter checkboxes' WHERE [Name] = 'jbAccount.errors.recommendedJobsNoResultsOneCheckbox'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown on the registration page if the user does not accept the terms of use' WHERE [Name] = 'jbAccount.errors.termsOfUseRequired'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 1st call-to-action on the saved industry profiles page' WHERE [Name] = 'jbAccount.industryProfiles.callToAction1BodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link text for the 1st call-to-action on the saved industry profiles page' WHERE [Name] = 'jbAccount.industryProfiles.callToAction1LinkText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link URL for the 1st call-to-action on the saved industry profiles page' WHERE [Name] = 'jbAccount.industryProfiles.callToAction1LinkUrl'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 2nd call-to-action on the saved industry profiles page' WHERE [Name] = 'jbAccount.industryProfiles.callToAction2BodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link text for the 2nd call-to-action on the saved industry profiles page' WHERE [Name] = 'jbAccount.industryProfiles.callToAction2LinkText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link URL for the 2nd call-to-action on the saved industry profiles page' WHERE [Name] = 'jbAccount.industryProfiles.callToAction2LinkUrl'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Explanatory text on the create/edit job alert page for users who select an email alert frequency of \"Never\"' WHERE [Name] = 'jbAccount.jobAlerts.noEmailHelpAnswer'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Title for the explanatory text on the create/edit job alert page for users who select an email alert frequency of \"Never\"' WHERE [Name] = 'jbAccount.jobAlerts.noEmailHelpQuestion'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Introductory text on the reset password dialog' WHERE [Name] = 'jbAccount.login.forgotPasswordIntroText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Explanatory text at the top of the recommended jobs filter' WHERE [Name] = 'jbAccount.recommendedJobs.filterIntroText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Introductory text that appears at the top of the recommended jobs page' WHERE [Name] = 'jbAccount.recommendedJobs.introText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Introductory text that appears at the top of the recommended jobs page for users with no recommended jobs' WHERE [Name] = 'jbAccount.recommendedJobs.introTextNoRecommendedJobs'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The body of the message shown after a user clicks the email confirmation link to activate their account.' WHERE [Name] = 'jbAccount.registration.activationBody'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The title of the message shown after a user clicks the email confirmation link to activate their account' WHERE [Name] = 'jbAccount.registration.activationTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The body message shown after a user completes the registration form. {0} = emailAddress.' WHERE [Name] = 'jbAccount.registration.confirmationBody'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The title of the message shown after a user completes the registration form' WHERE [Name] = 'jbAccount.registration.confirmationTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'HTML text for the terms of use' WHERE [Name] = 'jbAccount.registration.termsOfUseText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Title for the terms of use.' WHERE [Name] = 'jbAccount.registration.termsOfUseTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Text explaining our password complexity rules' WHERE [Name] = 'jbAccount.shared.passwordComplexity'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Explanatory text for the \"Do you identify as?\" section of the registration and personal settings pages' WHERE [Name] = 'jbAccount.shared.whyIdentify'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown when a user attempts to add the same city twice on the location filter' WHERE [Name] = 'jbSearch.errors.duplicateCity'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown when a user attempts to add the same postal code twice on the location filter' WHERE [Name] = 'jbSearch.errors.duplicatePostal'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown when a user enters an invalid city on the job search page' WHERE [Name] = 'jbSearch.errors.invalidCity'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown when a user enters an invalid postal code on the job search page' WHERE [Name] = 'jbSearch.errors.invalidPostalCode'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown when a search returns zero results' WHERE [Name] = 'jbSearch.errors.noSearchResults'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 1st call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction1BodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Introductory title for the 1st call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction1Intro'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Button text for the 1st call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction1LinkText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link URL for the 1st call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction1LinkUrl '");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the 1st call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction1Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Description for the 2nd call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction2BodyText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Introductory title for the 2nd call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction2Intro'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Button text for the 2nd call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction2LinkText'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Link URL for the 2nd call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction2LinkUrl'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the 2nd call-to-action on the job detail page' WHERE [Name] = 'jbSearch.jobDetail.callToAction2Title'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'A job is considered new if it was posted within this number of days.' WHERE [Name] = 'jbSearch.settings.newJobPeriodDays'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message shown when a user tries to add a duplicate title to a job alert' WHERE [Name] = 'shared.errors.jobAlertTitleDuplicate'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Error message for required job alert title' WHERE [Name] = 'shared.errors.jobAlertTitleRequired'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the date posted filter' WHERE [Name] = 'shared.filters.datePostedTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Explanatory text that appears on the education filter' WHERE [Name] = 'shared.filters.educationNote'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the education filter' WHERE [Name] = 'shared.filters.educationTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Explanatory text that appears at the bottom of the salary filter' WHERE [Name] = 'shared.filters.howIsSalaryCalculatedBody'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Title for the explanatory text at the bottom of the salary filter' WHERE [Name] = 'shared.filters.howIsSalaryCalculatedTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the industry filter' WHERE [Name] = 'shared.filters.industryTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the job type filter' WHERE [Name] = 'shared.filters.jobTypeTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Explanatory text that appears below the radius dropdown on the location filter' WHERE [Name] = 'shared.filters.locationRadiusNote'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The label for the region checkboxes on the location filter' WHERE [Name] = 'shared.filters.locationRegionSearchLabel'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'The label of the city or postal code search input on the location filter' WHERE [Name] = 'shared.filters.locationSearchLabel'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the location filter' WHERE [Name] = 'shared.filters.locationTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the more filters filter' WHERE [Name] = 'shared.filters.moreFiltersTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Main title for the salary filter' WHERE [Name] = 'shared.filters.salaryTitle'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Default radius (km) for location searches. Valid values are 10, 15, 25, 50, 75 or 100. If you enter an invalid value then 15 will be used instead.' WHERE [Name] = 'shared.settings.defaultSearchRadius'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Tooltip for 2016 NOC code in the more filters filter' WHERE [Name] = 'shared.tooltips.nocCode'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Tooltip for the unknown salaries option in the salary filter' WHERE [Name] = 'shared.tooltips.unknownSalaries'");
            migrationBuilder.Sql("update SystemSettings SET [Description] = 'Tooltip for the youth option in the more filters filter' WHERE [Name] = 'shared.tooltips.youth'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
