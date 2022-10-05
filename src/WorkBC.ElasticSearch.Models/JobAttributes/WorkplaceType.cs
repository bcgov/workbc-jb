using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class WorkplaceType
    {
        internal WorkplaceType()
        {
            // no public constructor
        }

        [JsonProperty("Id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}