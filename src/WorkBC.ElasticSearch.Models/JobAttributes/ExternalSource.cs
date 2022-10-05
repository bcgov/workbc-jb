using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class ExternalSource
    {
        internal ExternalSource()
        {
            // no public constructor
        }

        [JsonProperty("Url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("Source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }
    }
}