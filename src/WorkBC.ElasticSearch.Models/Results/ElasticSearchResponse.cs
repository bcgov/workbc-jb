using Newtonsoft.Json;
using WorkBC.ElasticSearch.Models.ElasticAttributes;

namespace WorkBC.ElasticSearch.Models.Results
{
    //All classes below are used to read the data received from Elastic Search unless otherwise commented.
    //When adding new fields to Elastic Search results it needs to be added here 
    public class ElasticSearchResponse
    {
        [JsonProperty("_scroll_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ScrollId { get; set; }

        [JsonProperty("took", NullValueHandling = NullValueHandling.Ignore)]
        public long? Took { get; set; }

        [JsonProperty("timed_out", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TimedOut { get; set; }

        [JsonProperty("_shards", NullValueHandling = NullValueHandling.Ignore)]
        public Shards Shards { get; set; }

        [JsonProperty("hits", NullValueHandling = NullValueHandling.Ignore)]
        public Hits Hits { get; set; }

        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int Count { get; set; }
    }
}
