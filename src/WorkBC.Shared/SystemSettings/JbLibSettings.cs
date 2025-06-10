using System;
using System.Collections.Generic;

namespace WorkBC.Shared.SystemSettings
{
    public class JbLibSettings : BaseSettings
    {
        public ErrorsClass Errors = new ErrorsClass();
        public FiltersComponentContent Filters = new FiltersComponentContent();
        public SettingsClass Settings = new SettingsClass();
        public TooltipsClass Tooltips = new TooltipsClass();

        public JbLibSettings(Dictionary<string, string> settings)
        {
            Settings.DefaultSearchRadius = GetDefaultSearchRadius(settings);
            Settings.MinimumWage = GetMinimumWage(settings);

            Tooltips.NocCode = settings["shared.tooltips.nocCode"];
            Tooltips.UnknownSalaries = settings["shared.tooltips.unknownSalaries"];
            Tooltips.JobSource = settings["shared.tooltips.jobSource"];
            Tooltips.OnSite = settings["shared.tooltips.onSite"];
            Tooltips.Hybrid = settings["shared.tooltips.hybrid"];
            Tooltips.Travelling = settings["shared.tooltips.travelling"];
            Tooltips.Virtual = settings["shared.tooltips.virtual"];


            SetFilters(settings);

            Errors.JobAlertTitleRequired = GetSetting(settings, "shared.errors.jobAlertTitleRequired");
            Errors.JobAlertTitleDuplicate = GetSetting(settings, "shared.errors.jobAlertTitleDuplicate");
        }

        private void SetFilters(Dictionary<string, string> settings)
        {
            Filters.HowIsSalaryCalculatedBody = settings["shared.filters.howIsSalaryCalculatedBody"];
            Filters.HowIsSalaryCalculatedTitle = settings["shared.filters.howIsSalaryCalculatedTitle"];
            Filters.DatePostedTitle = GetSetting(settings, "shared.filters.datePostedTitle");
            Filters.EducationNote = GetSetting(settings, "shared.filters.educationNote");
            Filters.EducationTitle = GetSetting(settings, "shared.filters.educationTitle");
            Filters.IndustryTitle = GetSetting(settings, "shared.filters.industryTitle");
            Filters.JobTypeTitle = GetSetting(settings, "shared.filters.jobTypeTitle");
            Filters.LocationRadiusNote = GetSetting(settings, "shared.filters.locationRadiusNote");
            Filters.LocationRegionSearchLabel = GetSetting(settings, "shared.filters.locationRegionSearchLabel");
            Filters.LocationSearchLabel = GetSetting(settings, "shared.filters.locationSearchLabel");
            Filters.LocationTitle = GetSetting(settings, "shared.filters.locationTitle");
            Filters.MoreFiltersTitle = GetSetting(settings, "shared.filters.moreFiltersTitle");
            Filters.SalaryTitle = GetSetting(settings, "shared.filters.salaryTitle");
            Filters.KeywordInputPlaceholder = GetSetting(settings, "shared.filters.keywordInputPlaceholder");
        }

        /// <summary>
        ///     Gets the defaultSearchRadius setting or defaults to 15 if it's invalid
        /// </summary>
        private static int GetDefaultSearchRadius(Dictionary<string, string> settings)
        {
            int.TryParse(settings["shared.settings.defaultSearchRadius"], out int defaultSearchRadius);

            switch (defaultSearchRadius)
            {
                case 10:
                case 15:
                case 25:
                case 50:
                case 75:
                case 100:
                    return defaultSearchRadius;
                default:
                    return 15;
            }
        }

        private static decimal GetMinimumWage(Dictionary<string, string> settings)
        {
            Decimal.TryParse(settings["shared.settings.minimumWage"], out decimal minimumWage);
            return minimumWage;

        }
        public class SettingsClass
        {
            public int DefaultSearchRadius { get; set; }
            public bool IsProduction { get; set; }

            public decimal MinimumWage { get; set; }
        }

        public class ErrorsClass
        {
            public string JobAlertTitleRequired { get; set; }
            public string JobAlertTitleDuplicate { get; set; }
        }

        public class TooltipsClass
        {
            public string NocCode { get; set; }
            public string UnknownSalaries { get; set; }
            public string JobSource { get; set; }
            public string OnSite { get; set; }
            public string Hybrid { get; set; }
            public string Travelling { get; set; }
            public string Virtual { get; set; }
        }

        public class FiltersComponentContent
        {
            public string HowIsSalaryCalculatedTitle { get; set; }
            public string HowIsSalaryCalculatedBody { get; set; }
            public string LocationRadiusNote { get; set; }
            public string EducationNote { get; set; }
            public string LocationTitle { get; set; }
            public string LocationSearchLabel { get; set; }
            public string LocationRegionSearchLabel { get; set; }
            public string JobTypeTitle { get; set; }
            public string SalaryTitle { get; set; }
            public string IndustryTitle { get; set; }
            public string EducationTitle { get; set; }
            public string DatePostedTitle { get; set; }
            public string MoreFiltersTitle { get; set; }
            public string KeywordInputPlaceholder { get; set; }
        }
    }
}