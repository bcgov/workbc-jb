using WorkBC.ElasticSearch.Models.JobAttributes;

namespace WorkBC.Web.Models
{
    public class RecommendedJobsResultModel
    {
        public Source[] Result { get; set; }
        public long Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string City { get; set; }
        public dynamic JobSeekerFlags { get; set; }
    }
}