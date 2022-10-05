namespace WorkBC.ElasticSearch.Search.Boosts
{
    public static class RecommendedJobsBoost
    {
        public const decimal Apprentice = 0.25m;
        public const decimal Veteran = 0.25m;
        public const decimal Aboriginal = 0.25m;
        public const decimal Mature = 0.25m;
        public const decimal Newcomer = 0.25m;
        public const decimal Disability = 0.25m;
        public const decimal Student = 0.25m;
        public const decimal Minority = 0.25m;
        public const decimal Youth = 0.25m;

        public const decimal Employers = 1m;
        public const decimal Titles = 1m;
        public const decimal NocCodes = 1m;

        // e.g. Employers boost = 1 and EmployerCountBonus = 0.01m and the user has 2 saved jobs for this employer:  
        // - employer boost becomes 1 + (2 * 0.01) = 1.02
        public const decimal EmployerCountBonus = 0.01m; 
        public const decimal NocCountBonus = 0.01m;
        public const decimal TitleCountBonus = 0.01m;

        public const decimal City = 1m;

        // other variables used by the RecommendedJobsService
        public const int MinimumShouldMatch = 1; // Elasticsearch "minimum_should_match"
        public const int MaxSavedJobs = 200;  // Reduce this number if there are performance issues
    }
}