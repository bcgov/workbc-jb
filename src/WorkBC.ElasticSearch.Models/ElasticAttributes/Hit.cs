using System.Collections.Generic;
using Newtonsoft.Json;
using WorkBC.ElasticSearch.Models.Helpers;
using WorkBC.ElasticSearch.Models.JobAttributes;

namespace WorkBC.ElasticSearch.Models.ElasticAttributes
{
    public class Hit
    {
        internal Hit()
        {
            // no public constructor
        }

        [JsonProperty("_index", NullValueHandling = NullValueHandling.Ignore)]
        public string Index { get; set; }

        [JsonProperty("_type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringToLongConverter))]
        public long? Id { get; set; }

        [JsonProperty("_score")]
        public decimal? Score { get; set; }

        [JsonProperty("_source", NullValueHandling = NullValueHandling.Ignore)]
        public Source Source { get; set; }

        [JsonProperty("sort", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Sort { get; set; }
    }
}