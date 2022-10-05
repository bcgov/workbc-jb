using System.Collections.Generic;

namespace WorkBC.Shared.SystemSettings
{
    public class JbSearchSettings : BaseSettings
    {
        public ErrorsClass Errors = new ErrorsClass();
        public JobDetailPageContent JobDetail = new JobDetailPageContent();
        public SettingsClass Settings = new SettingsClass();

        public JbSearchSettings(Dictionary<string, string> settings)
        {
            Errors.InvalidPostalCode = GetSetting(settings, "jbSearch.errors.invalidPostalCode");
            Errors.InvalidCity = GetSetting(settings, "jbSearch.errors.invalidCity");
            Errors.DuplicatePostal = GetSetting(settings, "jbSearch.errors.duplicatePostal");
            Errors.DuplicateCity = GetSetting(settings, "jbSearch.errors.duplicateCity");
            Errors.OutOfProvincePostal = GetSetting(settings, "jbSearch.errors.outOfProvincePostal");
            Errors.NoSearchResults = GetSetting(settings, "jbSearch.errors.noSearchResults");
            Errors.NoMapPins = GetSetting(settings, "jbSearch.errors.noMapPins");
            Errors.TooManyMapPins = GetSetting(settings, "jbSearch.errors.tooManyMapPins");
            Errors.CappedResultsWarning = GetSetting(settings, "jbSearch.errors.cappedResultsWarning");
            Errors.MissingMapPins = GetSetting(settings, "jbSearch.errors.missingMapPins");
            Errors.MissingOneMapPin = GetSetting(settings, "jbSearch.errors.missingOneMapPin");

            Settings.NewJobPeriodDays = int.Parse(settings["jbSearch.settings.newJobPeriodDays"]);

            SetJobDetail(settings);
        }

        private void SetJobDetail(Dictionary<string, string> settings)
        {
            JobDetail.CallToAction1Intro = GetSetting(settings, "jbSearch.jobDetail.callToAction1Intro");
            JobDetail.CallToAction1Title = GetSetting(settings, "jbSearch.jobDetail.callToAction1Title");
            JobDetail.CallToAction1BodyText = GetSetting(settings, "jbSearch.jobDetail.callToAction1BodyText");
            JobDetail.CallToAction1LinkText = GetSetting(settings, "jbSearch.jobDetail.callToAction1LinkText");
            JobDetail.CallToAction1LinkUrl = GetSetting(settings, "jbSearch.jobDetail.callToAction1LinkUrl");

            JobDetail.CallToAction2Intro = GetSetting(settings, "jbSearch.jobDetail.callToAction2Intro");
            JobDetail.CallToAction2Title = GetSetting(settings, "jbSearch.jobDetail.callToAction2Title");
            JobDetail.CallToAction2BodyText = GetSetting(settings, "jbSearch.jobDetail.callToAction2BodyText");
            JobDetail.CallToAction2LinkText = GetSetting(settings, "jbSearch.jobDetail.callToAction2LinkText");
            JobDetail.CallToAction2LinkUrl = GetSetting(settings, "jbSearch.jobDetail.callToAction2LinkUrl");
        }

        public class ErrorsClass
        {
            public string InvalidPostalCode { get; set; }
            public string InvalidCity { get; set; }
            public string DuplicatePostal { get; set; }
            public string DuplicateCity { get; set; }
            public string OutOfProvincePostal { get; set; }
            public string NoSearchResults { get; set; }
            public string NoMapPins { get; set; }
            public string TooManyMapPins { get; set; }
            public string CappedResultsWarning { get; set; }
            public string MissingMapPins { get; set; }
            public string MissingOneMapPin { get; set; }
        }

        public class SettingsClass
        {
            public int NewJobPeriodDays { get; set; }
        }

        public class JobDetailPageContent
        {
            public string CallToAction1Intro { get; set; }
            public string CallToAction1Title { get; set; }
            public string CallToAction1BodyText { get; set; }
            public string CallToAction1LinkText { get; set; }
            public string CallToAction1LinkUrl { get; set; }
            public string CallToAction2Intro { get; set; }
            public string CallToAction2Title { get; set; }
            public string CallToAction2BodyText { get; set; }
            public string CallToAction2LinkText { get; set; }
            public string CallToAction2LinkUrl { get; set; }
        }
    }
}