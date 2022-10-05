using WorkBC.Shared.Constants;

namespace WorkBC.ElasticSearch.Indexing.Settings
{
    public class IndexSettings
    {
        private string _defaultIndex;

        //Document type (default "_doc")
        public string ElasticDocType = General.ElasticDocType;

        //Elastic search index names
        public static string[] Indexes
        {
            get
            {
                return new[]
                {
                    General.EnglishIndex,
                    General.FrenchIndex
                };
            }
        }

        // Default search index
        public string DefaultIndex
        {
            get => _defaultIndex ?? General.EnglishIndex;
            set => _defaultIndex = value;
        }

        public string ElasticUser { get; set; }
        public string ElasticPassword { get; set; }
    }
}