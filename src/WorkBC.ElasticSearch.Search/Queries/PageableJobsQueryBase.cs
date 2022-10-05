using System.Text.RegularExpressions;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public abstract class PageableJobsQueryBase
    {
        public enum SortOrder
        {
            ASC = 0,
            DESC = 1
        }

        private readonly IPageableFilters _filters;

        protected PageableJobsQueryBase(IPageableFilters filters)
        {
            _filters = filters;
        }

        //Results properties
        public int RequestedPageSize { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public int Skip => PageNumber == 1 ? 0 : (PageNumber - 1) * RequestedPageSize;

        //Sort field
        public string SortField { get; set; }
        public SortOrder SortDirection { get; set; }


        /// <summary>
        ///     Set the paging and sorting fields in the elasticsearch query
        /// </summary>
        public string SetPagingAndSorting(string json, GeocodedLocationCache geoLocation = null)
        {
            var secondarySort = @"{""DatePosted"":""desc""},{""JobId.keyword"":""asc""}";

            if (geoLocation != null)
            {
                string geoSort = @"{""_geo_distance"":{""LocationGeo"":["
                                 + geoLocation.Longitude +
                                 "," +
                                 geoLocation.Latitude +
                                 @"],""order"":""asc"",""mode"":""min"",""distance_type"":""plane"",""ignore_unmapped"": true}}";
                secondarySort = geoSort + "," + secondarySort;
            }

            string jsonSort;

            if (!string.IsNullOrEmpty(SortField))
            {
                jsonSort = @"""sort"":[{""" + SortField + "\":\"" + SortDirection + "\"}," + secondarySort + "],";
            }
            else
            {
                jsonSort = @"""sort"":[{""_score"":""desc""}," + secondarySort + "],";
            }

            json = json.Replace("##SORT##", jsonSort);

            json = json.Replace("##PAGESIZE##", PageSize.ToString());
            json = json.Replace("##SKIP##", Skip.ToString());

            return json;
        }

        /// <summary>
        ///     Set the filter properties based on the values received in the constructor
        /// </summary>
        protected void SetSortFilters()
        {
            //default page size
            RequestedPageSize = _filters.PageSize;
            PageSize = _filters.PageSize;
            PageNumber = _filters.Page == 0 ? 1 : _filters.Page;

            //Sorting
            switch (_filters.SortOrder)
            {
                case 1:
                    SortField = "DatePosted";
                    SortDirection = SortOrder.DESC;
                    break;
                case 2:
                    SortField = "DatePosted";
                    SortDirection = SortOrder.ASC;
                    break;
                case 3:
                    SortField = "Title.normalize";
                    SortDirection = SortOrder.ASC;
                    break;
                case 4:
                    SortField = "Title.normalize";
                    SortDirection = SortOrder.DESC;
                    break;
                case 5:
                    SortField = "City.normalize";
                    SortDirection = SortOrder.ASC;
                    break;
                case 6:
                    SortField = "City.normalize";
                    SortDirection = SortOrder.DESC;
                    break;
                case 7:
                    SortField = "EmployerName.normalize";
                    SortDirection = SortOrder.ASC;
                    break;
                case 8:
                    SortField = "EmployerName.normalize";
                    SortDirection = SortOrder.DESC;
                    break;
                case 9:
                    SortField = "SalarySort.Ascending";
                    SortDirection = SortOrder.ASC;
                    break;
                case 10:
                    SortField = "SalarySort.Descending";
                    SortDirection = SortOrder.DESC;
                    break;
                case 11:
                    //Relevance
                    SortField = string.Empty;
                    SortDirection = SortOrder.ASC;
                    break;
                default:
                    //by default sort by date posted desc
                    SortField = "DatePosted";
                    SortDirection = SortOrder.DESC;
                    break;
            }
        }

        protected string RemoveExtraCommas(string json)
        {
            //Remove unnecessary comma in arrays that result in invalid JSON
            json = Regex.Replace(json, @"}\s*,+\s*]", "} ]");
            json = Regex.Replace(json, @"}\s*,+\s*}", "} }");
            return json;
        }
    }
}