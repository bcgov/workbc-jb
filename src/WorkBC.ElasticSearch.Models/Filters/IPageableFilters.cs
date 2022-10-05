namespace WorkBC.ElasticSearch.Models.Filters
{
    public interface IPageableFilters
    {
        int Page { get; set; }
        int PageSize { get; set; }
        int SortOrder { get; set; }
    }
}