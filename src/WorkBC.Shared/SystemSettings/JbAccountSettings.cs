using System.Collections.Generic;
using WorkBC.Shared.Utilities;

namespace WorkBC.Shared.SystemSettings
{
    public class JbAccountSettings : BaseSettings
    {
        public CareerProfilesPageContent CareerProfiles = new CareerProfilesPageContent();
        public DashboardPageContent Dashboard = new DashboardPageContent();
        public ErrorsClass Errors = new ErrorsClass();
        public IndustryProfilesPageContent IndustryProfiles = new IndustryProfilesPageContent();
        public JobAlertsPageContent JobAlerts = new JobAlertsPageContent();
        public LoginPageContent Login = new LoginPageContent();
        public RecommendedJobsPageContent RecommendedJobs = new RecommendedJobsPageContent();
        public RegistrationPageContent Registration = new RegistrationPageContent();
        public SettingsClass Settings = new SettingsClass();
        public SharedContent Shared = new SharedContent();

        public JbAccountSettings()
        {
            // empty constructor for deserializing from cache
        }

        public JbAccountSettings(Dictionary<string, string> settings)
        {
            // load all the values from the database here

            Registration.TermsOfUseText = settings["jbAccount.registration.termsOfUseText"];
            Registration.TermsOfUseTitle = settings["jbAccount.registration.termsOfUseTitle"];
            Registration.ConfirmationTitle = GetSetting(settings, "jbAccount.registration.confirmationTitle");
            Registration.ConfirmationBody = GetSetting(settings, "jbAccount.registration.confirmationBody");
            Registration.ActivationTitle = GetSetting(settings, "jbAccount.registration.activationTitle");
            Registration.ActivationBody = GetSetting(settings, "jbAccount.registration.activationBody");

            SetDashboard(settings);

            Errors.InvalidEmail = GetSetting(settings, "jbAccount.errors.invalidEmail");
            Errors.EmptyPassword = GetSetting(settings, "jbAccount.errors.emptyPassword");
            Errors.LoginFailed = GetSetting(settings, "jbAccount.errors.loginFailed");
            Errors.TermsOfUseRequired = GetSetting(settings, "jbAccount.errors.termsOfUseRequired");
            Errors.ForgotPasswordEmailNotFound = GetSetting(settings, "jbAccount.errors.forgotPasswordEmailNotFound");
            Errors.RecommendedJobsNoResultsOneCheckbox =
                GetSetting(settings, "jbAccount.errors.recommendedJobsNoResultsOneCheckbox");
            Errors.RecommendedJobsNoResultsMultipleCheckboxes = GetSetting(settings,
                "jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes");
            Errors.ForgotPasswordInvalidToken = GetSetting(settings, "jbAccount.errors.forgotPasswordInvalidToken");

            Login.ForgotPasswordIntroText = GetSetting(settings, "jbAccount.login.forgotPasswordIntroText");
            Login.ForgotPasswordConfirmationTitle =
                GetSetting(settings, "jbAccount.login.forgotPasswordConfirmationTitle");
            Login.ForgotPasswordConfirmationBody =
                GetSetting(settings, "jbAccount.login.forgotPasswordConfirmationBody");

            JobAlerts.NoEmailHelpQuestion = GetSetting(settings, "jbAccount.jobAlerts.noEmailHelpQuestion");
            JobAlerts.NoEmailHelpAnswer = GetSetting(settings, "jbAccount.jobAlerts.noEmailHelpAnswer");

            Shared.WhyIdentify = GetSetting(settings, "jbAccount.shared.whyIdentify");
            Shared.PasswordComplexity = GetSetting(settings, "jbAccount.shared.passwordComplexity");

            RecommendedJobs.IntroText = GetSetting(settings, "jbAccount.recommendedJobs.introText");
            RecommendedJobs.IntroTextNoRecommendedJobs =
                GetSetting(settings, "jbAccount.recommendedJobs.introTextNoRecommendedJobs");
            RecommendedJobs.FilterIntroText = GetSetting(settings, "jbAccount.recommendedJobs.filterIntroText");

            CareerProfiles.CallToAction1BodyText =
                GetSetting(settings, "jbAccount.careerProfiles.callToAction1BodyText");
            CareerProfiles.CallToAction1LinkText =
                GetSetting(settings, "jbAccount.careerProfiles.callToAction1LinkText");
            CareerProfiles.CallToAction1LinkUrl = GetSetting(settings, "jbAccount.careerProfiles.callToAction1LinkUrl");
            CareerProfiles.CallToAction2BodyText =
                GetSetting(settings, "jbAccount.careerProfiles.callToAction2BodyText");
            CareerProfiles.CallToAction2LinkText =
                GetSetting(settings, "jbAccount.careerProfiles.callToAction2LinkText");
            CareerProfiles.CallToAction2LinkUrl = GetSetting(settings, "jbAccount.careerProfiles.callToAction2LinkUrl");

            IndustryProfiles.CallToAction1BodyText =
                GetSetting(settings, "jbAccount.industryProfiles.callToAction1BodyText");
            IndustryProfiles.CallToAction1LinkText =
                GetSetting(settings, "jbAccount.industryProfiles.callToAction1LinkText");
            IndustryProfiles.CallToAction1LinkUrl =
                GetSetting(settings, "jbAccount.industryProfiles.callToAction1LinkUrl");
            IndustryProfiles.CallToAction2BodyText =
                GetSetting(settings, "jbAccount.industryProfiles.callToAction2BodyText");
            IndustryProfiles.CallToAction2LinkText =
                GetSetting(settings, "jbAccount.industryProfiles.callToAction2LinkText");
            IndustryProfiles.CallToAction2LinkUrl =
                GetSetting(settings, "jbAccount.industryProfiles.callToAction2LinkUrl");
        }

        private void SetDashboard(Dictionary<string, string> settings)
        {
            Dashboard.NewAccountMessageTitle = settings["jbAccount.dashboard.newAccountMessageTitle"];
            Dashboard.NewAccountMessageBody = settings["jbAccount.dashboard.newAccountMessageBody"];

            Dashboard.Notification1Title = settings["jbAccount.dashboard.notification1Title"];
            Dashboard.Notification1Body = settings["jbAccount.dashboard.notification1Body"];
            Dashboard.Notification1Enabled = settings["jbAccount.dashboard.notification1Enabled"] == "1";

            Dashboard.Notification2Title = settings["jbAccount.dashboard.notification2Title"];
            Dashboard.Notification2Body = settings["jbAccount.dashboard.notification2Body"];
            Dashboard.Notification2Enabled = settings["jbAccount.dashboard.notification2Enabled"] == "1";

            Dashboard.IntroText = GetSetting(settings, "jbAccount.dashboard.introText");
            Dashboard.JobsDescription = GetSetting(settings, "jbAccount.dashboard.jobsDescription");
            Dashboard.CareersDescription = GetSetting(settings, "jbAccount.dashboard.careersDescription");
            Dashboard.AccountDescription = GetSetting(settings, "jbAccount.dashboard.accountDescription");

            Dashboard.Resource1Title = GetSetting(settings, "jbAccount.dashboard.resource1Title");
            Dashboard.Resource2Title = GetSetting(settings, "jbAccount.dashboard.resource2Title");
            Dashboard.Resource3Title = GetSetting(settings, "jbAccount.dashboard.resource3Title");

            Dashboard.Resource1Body = GetSetting(settings, "jbAccount.dashboard.resource1Body");
            Dashboard.Resource2Body = GetSetting(settings, "jbAccount.dashboard.resource2Body");
            Dashboard.Resource3Body = GetSetting(settings, "jbAccount.dashboard.resource3Body");

            Dashboard.Resource1Url = GetSetting(settings, "jbAccount.dashboard.resource1Url");
            Dashboard.Resource2Url = GetSetting(settings, "jbAccount.dashboard.resource2Url");
            Dashboard.Resource3Url = GetSetting(settings, "jbAccount.dashboard.resource3Url");
        }

        public class SettingsClass
        {
        }

        public class ErrorsClass
        {
            public string InvalidEmail { get; set; }
            public string EmptyPassword { get; set; }
            public string ForgotPasswordInvalidToken { get; set; }
            public string RecommendedJobsNoResultsOneCheckbox { get; set; }
            public string RecommendedJobsNoResultsMultipleCheckboxes { get; set; }
            public string LoginFailed { get; set; }
            public string TermsOfUseRequired { get; set; }
            public string ForgotPasswordEmailNotFound { get; set; }
        }

        public class LoginPageContent
        {
            public string ForgotPasswordIntroText { get; set; }
            public string ForgotPasswordConfirmationBody { get; set; }
            public string ForgotPasswordConfirmationTitle { get; set; }
        }

        public class JobAlertsPageContent
        {
            public string NoEmailHelpQuestion { get; set; }
            public string NoEmailHelpAnswer { get; set; }
        }

        public class RecommendedJobsPageContent
        {
            public string IntroText { get; set; }
            public string IntroTextNoRecommendedJobs { get; set; }
            public string FilterIntroText { get; set; }
        }

        public class CareerProfilesPageContent
        {
            public string CallToAction1BodyText { get; set; }
            public string CallToAction1LinkText { get; set; }
            public string CallToAction1LinkUrl { get; set; }
            public string CallToAction2BodyText { get; set; }
            public string CallToAction2LinkText { get; set; }
            public string CallToAction2LinkUrl { get; set; }
        }

        public class IndustryProfilesPageContent
        {
            public string CallToAction1BodyText { get; set; }
            public string CallToAction1LinkText { get; set; }
            public string CallToAction1LinkUrl { get; set; }
            public string CallToAction2BodyText { get; set; }
            public string CallToAction2LinkText { get; set; }
            public string CallToAction2LinkUrl { get; set; }
        }

        public class DashboardPageContent
        {
            public string IntroText { get; set; }

            public string JobsDescription { get; set; }
            public string CareersDescription { get; set; }
            public string AccountDescription { get; set; }

            public string NewAccountMessageTitle { get; set; }
            public string NewAccountMessageBody { get; set; }

            public string Notification1Title { get; set; }
            public string Notification1Body { get; set; }

            public string Notification1Hash => HashingHelper.Hash2Strings(Notification1Title, Notification1Body);

            public bool Notification1Enabled { get; set; }

            public string Notification2Title { get; set; }
            public string Notification2Body { get; set; }

            public string Notification2Hash => HashingHelper.Hash2Strings(Notification2Title, Notification2Body);

            public bool Notification2Enabled { get; set; }

            public string Resource1Title { get; set; }
            public string Resource1Body { get; set; }
            public string Resource1Url { get; set; }

            public string Resource2Title { get; set; }
            public string Resource2Body { get; set; }
            public string Resource2Url { get; set; }

            public string Resource3Title { get; set; }
            public string Resource3Body { get; set; }
            public string Resource3Url { get; set; }
        }

        public class RegistrationPageContent
        {
            public string TermsOfUseTitle { get; set; }
            public string TermsOfUseText { get; set; }
            public string ConfirmationTitle { get; set; }
            public string ConfirmationBody { get; set; }
            public string ActivationTitle { get; set; }
            public string ActivationBody { get; set; }
        }

        // for content that appears in multiple places
        public class SharedContent
        {
            public string WhyIdentify { get; set; }
            public string PasswordComplexity { get; set; }
        }
    }
}