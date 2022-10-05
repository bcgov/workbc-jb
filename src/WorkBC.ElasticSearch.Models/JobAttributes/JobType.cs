using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class JobType
    {
        internal JobType()
        {
            // no public constructor
        }

        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Description { get; set; }
    }
}