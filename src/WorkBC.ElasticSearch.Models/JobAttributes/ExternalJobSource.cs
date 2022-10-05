using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class ExternalJobSource
    {
        internal ExternalJobSource()
        {
            // no public constructor
        }

        [JsonProperty("Source", NullValueHandling = NullValueHandling.Ignore)]
        public List<ExternalSource> Source { get; set; }
    }
}