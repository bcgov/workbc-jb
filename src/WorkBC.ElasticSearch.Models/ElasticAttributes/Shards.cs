using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.ElasticAttributes
{
    public class Shards
    {
        internal Shards()
        {
            // no public constructor
        }

        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Total { get; set; }

        [JsonProperty("successful", NullValueHandling = NullValueHandling.Ignore)]
        public long? Successful { get; set; }

        [JsonProperty("skipped", NullValueHandling = NullValueHandling.Ignore)]
        public long? Skipped { get; set; }

        [JsonProperty("failed", NullValueHandling = NullValueHandling.Ignore)]
        public long? Failed { get; set; }
    }
}