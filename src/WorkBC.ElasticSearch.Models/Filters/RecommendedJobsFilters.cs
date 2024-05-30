using System.Collections.Generic;

namespace WorkBC.ElasticSearch.Models.Filters
{
    public partial class RecommendedJobsFilters
    {
        // On/off settings for employment groups
        public bool FilterIsApprentice { get; set; }
        public bool FilterIsVeterans { get; set; }
        public bool FilterIsIndigenous { get; set; }
        public bool FilterIsMatureWorkers { get; set; }
        public bool FilterIsNewcomers { get; set; }
        public bool FilterIsPeopleWithDisabilities { get; set; }
        public bool FilterIsStudents { get; set; }
        public bool FilterIsVisibleMinority { get; set; }
        public bool FilterIsYouth { get; set; }

        // On/off settings for other filters
        public bool FilterSavedJobNocs { get; set; }
        public bool FilterSavedJobTitles { get; set; }
        public bool FilterSavedJobEmployers { get; set; }
        public bool FilterJobSeekerCity { get; set; }

        // Job seeker's filter data (scraped from saved jobs)
        public Dictionary<string, int> Titles { get; set; }
        public Dictionary<short, int> NocCodes { get; set; }
        public Dictionary<int, int> NocCodes2021 { get; set; }
        public Dictionary<string, int> Employers { get; set; }

        // Job seeker's filter data (scraped from user profile)
        public string City { get; set; }

        // List of the job seeker's saved jobs (to be excluded from recommendations, because recommendations are based on saved jobs)
        public string[] IgnoreJobIdList { get; set; }

        public bool IsApprentice { get; set; }
        public bool IsVeterans { get; set; }
        public bool IsIndigenous { get; set; }
        public bool IsMatureWorkers { get; set; }
        public bool IsNewcomers { get; set; }
        public bool IsPeopleWithDisabilities { get; set; }
        public bool IsStudents { get; set; }
        public bool IsVisibleMinority { get; set; }
        public bool IsYouth { get; set; }
    }

    public partial class RecommendedJobsFilters : IPageableFilters
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int SortOrder { get; set; }

    }
}