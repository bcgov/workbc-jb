using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.ElasticAttributes
{
    public class Total
    {
        internal Total()
        {
            // no public constructor
        }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        [JsonProperty("relation", NullValueHandling = NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }
}