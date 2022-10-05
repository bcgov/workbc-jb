using WorkBC.ElasticSearch.Models.JobAttributes;

namespace WorkBC.Web.Models.Search
{
    //This will be used to render data on screen, add any properties here that will be needed
    public class SearchResultsModel
    {
        public Source[] Result { get; set; }
        public long Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public JobDetailsPageLabels TextHeaders { get; set; }
    }
}