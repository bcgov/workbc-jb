using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class Category
    {
        internal Category()
        {
            // no public constructor
        }

        [JsonProperty("Id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("Key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
    }
}