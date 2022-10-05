namespace WorkBC.Shared.Constants
{
    public class General
    {
        public const string SystemSettingsTimestampCacheKey = "SETTINGS_DT";

        public const int CacheMinutes = 60;
        public const int DefaultWantedJobExpiryDays = 30;

        //Elastic search index names
        public const string EnglishIndex = "jobs_en";
        public const string FrenchIndex = "jobs_fr";

        //Elastic search index names
        public static string[] Indexes = {EnglishIndex, FrenchIndex};

        //Document type (default "_doc")
        public const string ElasticDocType = "_doc";
    }
}