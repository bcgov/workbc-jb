using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.ElasticAttributes
{
    public class Hits
    {
        internal Hits()
        {
            // no public constructor
        }

        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }

        [JsonProperty("max_score")]
        public object MaxScore { get; set; }

        [JsonProperty("hits", NullValueHandling = NullValueHandling.Ignore)]
        public List<Hit> HitsHits { get; set; }
    }
}