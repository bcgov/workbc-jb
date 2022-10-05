using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class Location
    {
        internal Location()
        {
            // no public constructor
        }

        [JsonProperty("Lat", NullValueHandling = NullValueHandling.Ignore)]
        public string Lat { get; set; }

        [JsonProperty("Lon", NullValueHandling = NullValueHandling.Ignore)]
        public string Lon { get; set; }
    }
}