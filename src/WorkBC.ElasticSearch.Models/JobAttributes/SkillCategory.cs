using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class SkillCategory
    {
        internal SkillCategory()
        {
            // no public constructor
        }

        [JsonProperty("Category", NullValueHandling = NullValueHandling.Ignore)]
        public Category Category { get; set; }

        [JsonProperty("SkillCount", NullValueHandling = NullValueHandling.Ignore)]
        public int SkillCount { get; set; }

        [JsonProperty("Skills", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Skills { get; set; }
    }
}