using System;
using System.Collections.Generic;
using System.ComponentModel;
using WorkBC.ElasticSearch.Models.JobAttributes;

namespace WorkBC.ElasticSearch.Models.Filters
{
    public class JobSearchFilters : IPageableFilters
    {
        public DateField StartDate { get; set; }

        public DateField EndDate { get; set; }

        /// <example>1</example>
        public int Page { get; set; }

        /// <example>20</example>
        public int PageSize { get; set; }

        /// <example></example>
        public string Keyword { get; set; }

        /// <example>all</example>
        public string SearchInField { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeFullTime { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypePartTime { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeLeadingToFullTime { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypePermanent { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeTemporary { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeCasual { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeSeasonal { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeDay { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeEarly { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeEvening { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeFlexible { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeMorning { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeNight { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeOnCall { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeOvertime { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeShift { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeTbd { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeWeekend { get; set; }

        /// <summary>
        ///     ANY_DATE = "0", TODAY = "1", PAST_THREE_DAYS = "2", DATE_RANGE = "3"
        /// </summary>
        /// <example>0</example>
        public string SearchDateSelection { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeOnSite { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeHybrid { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeTravelling { get; set; }

        [DefaultValue(false)]
        public bool SearchJobTypeVirtual { get; set; }

        /// <example>["University", "College or apprenticeship", "Secondary school or job-specific training", "No education"]</example>
        public List<string> SearchJobEducationLevel { get; set; }

        /// <example>4</example>
        public int SalaryType { get; set; }

        /// <example>-1</example>
        public int SearchLocationDistance { get; set; }

        public List<LocationField> SearchLocations { get; set; }

        /// <example>
        ///     ["As per collective agreement", "Bonus", "Commission", "Dental benefits", "Disability benefits", "Gratuities",
        ///     "Group insurance benefits", "Life insurance benefits", "Medical benefits", "Mileage paid", "Pension plan benefits",
        ///     "Piece work", "RESP benefits", "RRSP benefits", "Vision care benefits", "Other benefits"]
        /// </example>
        public List<string> SearchSalaryConditions { get; set; }

        /// <example>11</example>
        public int SortOrder { get; set; }

        [DefaultValue(false)]
        public bool SalaryBracket1 { get; set; }

        [DefaultValue(false)]
        public bool SalaryBracket2 { get; set; }

        [DefaultValue(false)]
        public bool SalaryBracket3 { get; set; }

        [DefaultValue(false)]
        public bool SalaryBracket4 { get; set; }

        [DefaultValue(false)]
        public bool SalaryBracket5 { get; set; }

        [DefaultValue(false)]
        public bool SalaryBracket6 { get; set; }
        
        /// <example></example>
        public string SalaryMin { get; set; }

        /// <example></example>
        public string SalaryMax { get; set; }

        /// <example>
        ///     ["37", "40", "1", "36", "23", "34", "42", "29", "35", "28", "32", "24", "21", "44", "46", "31", "39", "30",
        ///     "45", "43", "26", "27", "22", "41", "25" ]
        /// </example>
        public List<int> SearchIndustry { get; set; }

        [DefaultValue(false)]
        public bool SearchIsApprentice { get; set; }

        [DefaultValue(false)]
        public bool SearchIsVeterans { get; set; }

        [DefaultValue(false)]
        public bool SearchIsIndigenous { get; set; }

        [DefaultValue(false)]
        public bool SearchIsMatureWorkers { get; set; }

        [DefaultValue(false)]
        public bool SearchIsNewcomers { get; set; }

        [DefaultValue(false)]
        public bool SearchIsPeopleWithDisabilities { get; set; }

        [DefaultValue(false)]
        public bool SearchIsStudents { get; set; }

        [DefaultValue(false)]
        public bool SearchIsVisibleMinority { get; set; }

        [DefaultValue(false)]
        public bool SearchIsYouth { get; set; }

        [DefaultValue(true)]
        public bool SearchIsPostingsInEnglish { get; set; }

        [DefaultValue(false)]
        public bool SearchIsPostingsInEnglishAndFrench { get; set; }

        /// <example></example>
        public string SearchNocField { get; set; }

        /// <example>0</example>
        public string SearchJobSource { get; set; }

        [DefaultValue(false)]
        public bool SearchExcludePlacementAgencyJobs { get; set; }
    }

    public class DateField
    {
        public DateField()
        {
            Year = 0;
            Month = 0;
            Day = 0;
            Hour = 0;
            Minute = 0;
            Second = 0;
            Millisecond = 0;
        }

        public DateField(DateTime date)
        {
            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
            Hour = date.Hour;
            Minute = date.Minute;
            Second = date.Second;
            Millisecond = date.Millisecond;
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }

        // returns a YYYY-MM-DD string
        public override string ToString()
        {
            return $"{Year}-{Month:00}-{Day:00}T{Hour:00}:{Minute:00}:{Second:00}.{Millisecond:000}";
        }
    }

    public class LocationField
    {
        private string _postal;

        /// <example>Surrey</example>
        public string City { get; set; }

        /// <example></example>
        public string Region { get; set; }

        /// <example></example>
        public string Postal
        {
            // uppercase postal codes and remove spaces
            get => _postal?.ToUpper().Replace(" ", "");
            set => _postal = value;
        }
    }
}
