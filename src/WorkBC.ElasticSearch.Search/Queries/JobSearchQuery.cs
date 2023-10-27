using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.Helpers;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Utilities;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Services;
using WorkBC.Shared.Utilities;
using Boost = WorkBC.ElasticSearch.Search.Boosts.JobSearchBoost;
using Location = WorkBC.ElasticSearch.Models.JobAttributes.Location;
using SalaryType = WorkBC.ElasticSearch.Models.Helpers.SalaryRangeHelper.SalaryType;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public class JobSearchQuery : PageableJobsQueryBase
    {
        private readonly IConfiguration _configuration;
        private readonly JobSearchFilters _filters;

        #region Constructor

        public JobSearchQuery(IGeocodingService geocodingService, IConfiguration configuration, JobSearchFilters filters) : base(filters)
        {
            //Set default values
            DateSearchType = DateType.NONE;
            SalarySearchType = SalaryType.NONE;
            _configuration = configuration;
            _filters = filters;
            _geocodingService = geocodingService;
        }

        #endregion

        #region Properties

        //Enums
        public enum DateType
        {
            ANY_DATE = 0,
            TODAY = 1,
            PAST_THREE_DAYS = 2,
            DATE_RANGE = 3,
            NONE = 4
        }

        public enum LocationDistanceKm
        {
            EXACT_MATCH = -1,
            NONE = 0,
            TEN = 10,
            FIFTEEN = 15,
            TWENTY_FIVE = 25,
            FIFTY = 50,
            SEVENTY_FIVE = 75,
            HUNDERD = 100
        }

        //Date properties
        public DateType DateSearchType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        //salary properties
        public List<KeyValuePair<string, string>> SalaryRanges { get; set; }
        public SalaryType SalarySearchType { get; set; }

        //Keyword search
        public string Keyword { get; set; }

        //Industries
        public List<int> SearchIndustry { get; set; }

        //Search fields
        public string SearchInField { get; set; }
        public bool SearchJobTypeFullTime { get; set; }
        public bool SearchJobTypePartTime { get; set; }
        public bool SearchJobTypeLeadingToFullTime { get; set; }
        public bool SearchJobTypePermanent { get; set; }
        public bool SearchJobTypeTemporary { get; set; }
        public bool SearchJobTypeCasual { get; set; }
        public bool SearchJobTypeSeasonal { get; set; }
        public bool SearchJobTypeDay { get; set; }
        public bool SearchJobTypeEarly { get; set; }
        public bool SearchJobTypeEvening { get; set; }
        public bool SearchJobTypeFlexible { get; set; }
        public bool SearchJobTypeMorning { get; set; }
        public bool SearchJobTypeNight { get; set; }
        public bool SearchJobTypeOnCall { get; set; }
        public bool SearchJobTypeOvertime { get; set; }
        public bool SearchJobTypeShift { get; set; }
        public bool SearchJobTypeTbd { get; set; }
        public bool SearchJobTypeWeekend { get; set; }
        public bool SearchJobTypeOnSite { get; set; }
        public bool SearchJobTypeHybrid { get; set; }
        public bool SearchJobTypeTravelling { get; set; }
        public bool SearchJobTypeVirtual { get; set; }
        public List<string> SearchJobEducationLevel { get; set; }

        //location
        public LocationDistanceKm LocationDistance { get; set; }

        public List<LocationField> Locations { get; set; }

        //More filters
        //Employment groups
        public bool SearchIsApprentice { get; set; }
        public bool SearchIsVeterans { get; set; }
        public bool SearchIsIndigenous { get; set; }
        public bool SearchIsMatureWorkers { get; set; }
        public bool SearchIsNewcomers { get; set; }
        public bool SearchIsPeopleWithDisabilities { get; set; }
        public bool SearchIsStudents { get; set; }
        public bool SearchIsVisibleMinority { get; set; }
        public bool SearchIsYouth { get; set; }

        //Job posting language
        public bool SearchIsPostingsInEnglish { get; set; }
        public bool SearchIsPostingsInEnglishAndFrench { get; set; }

        //Job source (more filters)
        public string SearchJobSource { get; set; }

        //Noc code field (more filters)
        public string SearchNocField { get; set; }

        //(More filters)
        public bool SearchExcludePlacementAgencies { get; set; }

        //Salary benefits
        public List<string> SearchSalaryConditions { get; set; }

        //Services
        private readonly IGeocodingService _geocodingService;

        #endregion

        #region Methods

        /// <summary>
        ///     Build JSON string that will be used to query Elastic Search
        /// </summary>
        public async Task<string> ToJson(IConfiguration configuration, string jsonFileName)
        {
            string json = ResourceFileHelper.ReadFile(jsonFileName);
            var filterGroups = new List<string>();
            GeocodedLocationCache geoLocation = null;

            #region Date search

            string jsonDateFilter;

            switch (DateSearchType)
            {
                case DateType.DATE_RANGE:
                    jsonDateFilter = "{ \"range\": { \"DatePosted\": { \"gte\": \"" + StartDate + "\", \"lte\": \"" + EndDate + "\", \"time_zone\": \"America/Vancouver\" } } }";
                    filterGroups.Add(jsonDateFilter);
                    break;
                case DateType.PAST_THREE_DAYS:
                    jsonDateFilter = "{ \"range\": { \"DatePosted\": { \"gte\": \"now-3d/d\", \"lte\": \"now\", \"time_zone\": \"America/Vancouver\" } } }";
                    filterGroups.Add(jsonDateFilter);
                    break;
                case DateType.TODAY:
                    jsonDateFilter = "{ \"range\": { \"DatePosted\": { \"gte\": \"now/d\", \"lt\": \"now+1d/d\", \"time_zone\": \"America/Vancouver\" } } }";
                    filterGroups.Add(jsonDateFilter);
                    break;
            }

            #endregion

            #region Salary search

            if (_filters.SearchSalaryUnknown || (SalarySearchType != SalaryType.NONE && SalaryRanges.Count > 0))
            {
                var jsonSalaryFilter = "";

                var k = 0;
                foreach (KeyValuePair<string, string> salaryRange in SalaryRanges)
                {
                    if (k > 0)
                    {
                        jsonSalaryFilter += ",";
                    }

                    //try and parse the string values to numeric values to prevent errors in ElasticSearch when parsing the query
                    decimal.TryParse(salaryRange.Key, out decimal minSalary);
                    decimal.TryParse(salaryRange.Value, out decimal maxSalary);

                    if (maxSalary > 0)
                    {
                        //option 1 - 5 where we have ranges with definate values
                        jsonSalaryFilter += "{ \"range\": { \"Salary\": { \"gte\": " + minSalary + ", \"lte\": " + maxSalary + " } } }";
                    }
                    else
                    {
                        //If the user used "unlimited" the max salary will not have a valid numeric value, 
                        //so then we only use the min salary value
                        jsonSalaryFilter += "{ \"range\": { \"Salary\": { \"gte\": " + minSalary + " } } }";
                    }

                    k++;
                }

                if (_filters.SearchSalaryUnknown)
                {
                    if (k > 0)
                    {
                        jsonSalaryFilter += ",";
                    }

                    // You can't search for null so we search in SalarySort.Descending instead.  
                    // The indexer sets this field to -99999999 for jobs with no salary.
                    jsonSalaryFilter += "{ \"range\": { \"SalarySort.Descending\": { \"lte\": -99999999 } } }";
                }

                filterGroups.Add(jsonSalaryFilter);
            }

            if (SearchSalaryConditions != null && SearchSalaryConditions.Count > 0)
            {
                var jsonBenefits = string.Empty;

                foreach (string condition in SearchSalaryConditions)
                {
                    jsonBenefits += "{ \"term\": { \"SalaryConditions.Description.keyword\": \"" + condition + "\" } },";
                }

                //remove last comma
                jsonBenefits = jsonBenefits.Substring(0, jsonBenefits.Length - 1);

                filterGroups.Add(jsonBenefits);
            }

            #endregion

            #region Keywords search

            if (!string.IsNullOrEmpty(Keyword))
            {
                string queryString = KeywordParsing.BuildSimpleQueryString(Keyword);

                if (!string.IsNullOrEmpty(queryString))
                {
                    string fields;

                    switch (SearchInField.ToLower())
                    {
                        case "employer":
                            fields = "\"EmployerName\"";
                            break;
                        case "jobid":
                            fields = "\"JobId\"";
                            break;
                        case "title":
                            fields = "\"Title\"";
                            break;
                        default:
                            var fieldsBoostTemplate =
                                "\"EmployerName^{0}\",\"JobId^{1}\",\"Title^{2}\",\"AllSkills^{3}\",\"JobDescription^{4}\",\"City^{5}\"";
                            fields = string.Format(fieldsBoostTemplate,
                                Boost.EmployerName,
                                Boost.JobId,
                                Boost.Title,
                                Boost.AllSkills,
                                Boost.JobDescription,
                                Boost.City);
                            break;
                    }

                    // json escape quotation marks
                    queryString = queryString.Replace(@"""", @"\""");

                    string jsonKeywords =
                        $"{{ \"simple_query_string\": {{ \"query\": \"{queryString}\", \"fields\": [{fields}], \"default_operator\": \"AND\", \"quote_field_suffix\": \".exact\" }} }}";

                    filterGroups.Add(jsonKeywords);
                }
            }

            #endregion

            #region Job Type

            //Hours of Work (Full-time, Part-time, Part-time leading to full-time)
            if (SearchJobTypeFullTime || SearchJobTypePartTime || SearchJobTypeLeadingToFullTime)
            {
                var hoursOfWork = string.Empty;

                if (SearchJobTypeFullTime)
                {
                    hoursOfWork += "{ \"term\": { \"HoursOfWork.Description.keyword\": \"Full-time\" } },";
                }

                if (SearchJobTypePartTime)
                {
                    hoursOfWork += "{ \"term\": { \"HoursOfWork.Description.keyword\": \"Part-time\" } },";
                }

                if (SearchJobTypeLeadingToFullTime)
                {
                    hoursOfWork += "{ \"term\": { \"HoursOfWork.Description.keyword\": \"Part-time leading to full-time\" } },";
                }

                //remove last comma
                hoursOfWork = hoursOfWork.Substring(0, hoursOfWork.Length - 1);
                filterGroups.Add(hoursOfWork);
            }

            //Period of Employment (Temporary, Permanent, Casual, Seasonal)
            if (SearchJobTypeTemporary || SearchJobTypePermanent || SearchJobTypeSeasonal || SearchJobTypeCasual)
            {
                var periodOfEmployment = string.Empty;

                if (SearchJobTypeTemporary)
                {
                    periodOfEmployment += "{ \"term\": { \"PeriodOfEmployment.Description.keyword\": \"Temporary\" } },";
                }

                if (SearchJobTypePermanent)
                {
                    periodOfEmployment += "{ \"term\": { \"PeriodOfEmployment.Description.keyword\": \"Permanent\" } },";
                }

                if (SearchJobTypeCasual)
                {
                    periodOfEmployment += "{ \"term\": { \"PeriodOfEmployment.Description.keyword\": \"Casual\" } },";
                }

                if (SearchJobTypeSeasonal)
                {
                    periodOfEmployment += "{ \"term\": { \"PeriodOfEmployment.Description.keyword\": \"Seasonal\" } },";
                }

                //remove last comma
                periodOfEmployment = periodOfEmployment.Substring(0, periodOfEmployment.Length - 1);
                filterGroups.Add(periodOfEmployment);
            }

            //Employment Terms (Day, Early, Evening, Flexible, Morning, Night, On Call, Overtime, Shift, To be determined, Weekend)
            if (SearchJobTypeDay || SearchJobTypeEarly || SearchJobTypeEvening || SearchJobTypeFlexible || SearchJobTypeMorning || SearchJobTypeShift ||
                SearchJobTypeNight || SearchJobTypeOnCall || SearchJobTypeOvertime || SearchJobTypeTbd || SearchJobTypeWeekend)
            {
                var employmentTerms = string.Empty;

                if (SearchJobTypeDay)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Day\" } },";
                }

                if (SearchJobTypeEarly)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Early morning\" } },";
                }

                if (SearchJobTypeEvening)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Evening\" } },";
                }

                if (SearchJobTypeFlexible)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Flexible hours\" } },";
                }

                if (SearchJobTypeMorning)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Morning\" } },";
                }

                if (SearchJobTypeNight)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Night\" } },";
                }

                if (SearchJobTypeOnCall)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"On call\" } },";
                }

                if (SearchJobTypeOvertime)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Overtime\" } },";
                }

                if (SearchJobTypeShift)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Shift\" } },";
                }

                if (SearchJobTypeTbd)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"To be determined\" } },";
                }

                if (SearchJobTypeWeekend)
                {
                    employmentTerms += "{ \"term\": { \"EmploymentTerms.Description.keyword\": \"Weekend\" } },";
                }

                //remove last comma
                employmentTerms = employmentTerms.Substring(0, employmentTerms.Length - 1);
                filterGroups.Add(employmentTerms);
            }

            #endregion

            #region WorkplaceType 
            //Workplace Type (On-site only, On-site or remote work, Work location varies, Virtual job)
            if (SearchJobTypeOnSite || SearchJobTypeHybrid || SearchJobTypeTravelling || SearchJobTypeVirtual)
            {
                var workplaceType = string.Empty;

                if (SearchJobTypeOnSite)
                {
                    workplaceType += "{ \"term\": { \"WorkplaceType.Id\": 0 } },";
                }

                if (SearchJobTypeHybrid)
                {
                    workplaceType += "{ \"term\": { \"WorkplaceType.Id\": 100000 } },";
                }

                if (SearchJobTypeTravelling)
                {
                    workplaceType += "{ \"term\": { \"WorkplaceType.Id\": 100001 } },";
                }

                if (SearchJobTypeVirtual)
                {
                    workplaceType += "{ \"term\": { \"WorkplaceType.Id\": 15141 } },";
                }

                //remove last comma
                workplaceType = workplaceType.Substring(0, workplaceType.Length - 1);
                filterGroups.Add(workplaceType);
            }



            #endregion

            #region Education

            //ElasticSearch "term" would do an EXACT match.

            if (SearchJobEducationLevel != null && SearchJobEducationLevel.Count > 0)
            {
                var jsonEducationLevel = "";

                for (var k = 0; k < SearchJobEducationLevel.Count; k++)
                {
                    if (k > 0)
                    {
                        jsonEducationLevel += ",";
                    }

                    jsonEducationLevel += "{ \"term\": { \"EduLevel.keyword\": \"" + SearchJobEducationLevel[k] + "\" } }";
                }

                filterGroups.Add(jsonEducationLevel);
            }

            #endregion

            #region City / Postal Code (Location)

            if (Locations.Count > 0 && Locations[0] != null)
            {
                var postal = string.Empty;
                List<string> locationFilters = new List<string>();

                //Only one location selected (keyword with postal/city) OR
                //Location filter with only one option
                if (Locations.Count == 1)
                {
                    string geoStr;

                    if (!string.IsNullOrEmpty(Locations[0].Postal))
                    {
                        //POSTAL CODE

                        if (LocationDistance == LocationDistanceKm.EXACT_MATCH)
                        {
                            //Exact match
                            locationFilters.Add("{ \"term\": { \"PostalCode.keyword\": \"" + Locations[0].Postal + "\" } },");
                        }
                        else
                        {
                            string postalCodeKey = $"{Locations[0].Postal}, CANADA";

                            //Get location Long and Lat 
                            //Try and load from Cache else use Google Maps
                            geoLocation = await _geocodingService.GetLocation(postalCodeKey);

                            if (geoLocation != null)
                            {
                                //Need to apply distance parameter
                                geoStr = "{ \"geo_distance\" : { \"distance\" : \"" + (int)LocationDistance + "km\", \"LocationGeo\" : { \"lat\" : " + geoLocation.Latitude + ",\"lon\" : " + geoLocation.Longitude + " } } }";

                                //Update Elastic Search Query
                                locationFilters.Add(geoStr);
                            }
                            else
                            {
                                // invalid location.  Use a lat/lon in the middle of the pacific ocean.
                                locationFilters.Add("{ \"geo_distance\" : { \"distance\" : \"1km\", \"LocationGeo\" : { \"lat\" : 0,\"lon\" : 180 } } }");
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(Locations[0].City) || !string.IsNullOrEmpty(Locations[0].Region))
                    {
                        //CITY
                        string cityName = string.IsNullOrEmpty(Locations[0].City) ? string.Empty : Locations[0].City.Trim();
                        string regionName = string.IsNullOrEmpty(Locations[0].Region) ? string.Empty : Locations[0].Region.Trim();

                        
                        if (regionName != string.Empty)
                        {
                            string jsonRegion = "{ \"term\": { \"Region.keyword\": \"" + regionName + "\" } },";
                            locationFilters.Add(jsonRegion);
                        } 
                        else if (LocationDistance == LocationDistanceKm.EXACT_MATCH)
                        {
                            //Exact match
                            if (cityName != string.Empty)
                            {
                                string jsonCity = "{ \"term\": { \"City.normalize\": \"" + cityName.ToLower() + "\" } }";
                                locationFilters.Add(jsonCity);
                            }
                        }
                        else
                        {
                            //Apply radius for city
                            if (cityName != string.Empty)
                            {
                                string cityNameKey = $"{cityName}, BC, CANADA";

                                //Get location Long and Lat 
                                //Try and load from Cache else use Google Maps
                                geoLocation = await _geocodingService.GetLocation(cityNameKey);

                                if (geoLocation != null)
                                {
                                    //Need to apply distance parameter
                                    geoStr = "{ \"geo_distance\" : { \"distance\" : \"" + (int)LocationDistance + "km\", \"LocationGeo\" : { \"lat\" : " + geoLocation.Latitude + ",\"lon\" : " + geoLocation.Longitude + " } } }";

                                    //Update Elastic Search Query
                                    locationFilters.Add(geoStr);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Multiple locations selected
                    //MUST be exact match because of multiple locations

                    var jsonLocations = string.Empty;

                    foreach (LocationField location in Locations)
                    {
                        if (!string.IsNullOrEmpty(location.Postal))
                        {
                            jsonLocations += "{ \"term\": { \"PostalCode.keyword\": \"" + location.Postal + "\" } },";
                        }

                        if (!string.IsNullOrEmpty(location.City))
                        {
                            jsonLocations += "{ \"term\": { \"City.normalize\": \"" + location.City.ToLower() + "\" } },";
                        }

                        if (!string.IsNullOrEmpty(location.Region))
                        {
                            jsonLocations += "{ \"term\": { \"Region.keyword\": \"" + location.Region + "\" } },";
                        }
                    }

                    if (!string.IsNullOrEmpty(jsonLocations))
                    {
                        //remove last comma 
                        if (jsonLocations.Length > 0)
                        {
                            jsonLocations = jsonLocations.Substring(0, jsonLocations.Length - 1);
                        }

                        //apply filter
                        locationFilters.Add(jsonLocations);
                    }
                }

                if (locationFilters.Any())
                {
                    // include all virtual jobs when there is a location filter
                    filterGroups.AddRange(
                        locationFilters
                            .Select(loc => "{\"term\":{\"WorkplaceType.Id\":{\"value\":15141,\"boost\":0}}}," + loc)
                    );
                }
            }

            #endregion

            #region Industry filter

            if (SearchIndustry != null && SearchIndustry.Count > 0)
            {
                var jsonIndustry = "";

                for (var k = 0; k < SearchIndustry.Count; k++)
                {
                    if (k > 0)
                    {
                        jsonIndustry += ",";
                    }

                    jsonIndustry += "{ \"term\": { \"NaicsId\": " + SearchIndustry[k] + " } }";
                }

                filterGroups.Add(jsonIndustry);
            }

            #endregion

            #region More filters

            //Employment groups
            if (SearchIsApprentice || SearchIsIndigenous || SearchIsMatureWorkers ||
                SearchIsNewcomers || SearchIsPeopleWithDisabilities || SearchIsStudents ||
                SearchIsVeterans || SearchIsVisibleMinority || SearchIsYouth)
            {
                var jsonEmploymentGroups = string.Empty;

                if (SearchIsApprentice)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsApprentice\": true } },";
                }

                if (SearchIsIndigenous)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsAboriginal\": true } },";
                }

                if (SearchIsMatureWorkers)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsMatureWorker\": true } },";
                }

                if (SearchIsNewcomers)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsNewcomer\": true } },";
                }

                if (SearchIsPeopleWithDisabilities)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsDisability\": true } },";
                }

                if (SearchIsStudents)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsStudent\": true } },";
                }

                if (SearchIsVeterans)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsVeteran\": true } },";
                }

                if (SearchIsVisibleMinority)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsVismin\": true } },";
                }

                if (SearchIsYouth)
                {
                    jsonEmploymentGroups += "{ \"term\": { \"IsYouth\": true } },";
                }

                //remove last comma
                if (jsonEmploymentGroups.Length > 0)
                {
                    jsonEmploymentGroups = jsonEmploymentGroups.Substring(0, jsonEmploymentGroups.Length - 1);
                    filterGroups.Add(jsonEmploymentGroups);
                }
            }

            //Job posting language
            if (SearchIsPostingsInEnglishAndFrench)
            {
                //Only Federal jobs
                var jsonPostingLanguage = "{ \"term\": { \"IsFederalJob\": true } }";
                filterGroups.Add(jsonPostingLanguage);
            }

            //NOC field
            if (!string.IsNullOrEmpty(SearchNocField))
            {
                string nocJson = "{ \"term\": { \"Noc\": \"" + SearchNocField + "\" } }";
                filterGroups.Add(nocJson);
            }

            //Job source
            if (!string.IsNullOrEmpty(SearchJobSource) && SearchJobSource != "0")
            {
                var jsonJobSource = string.Empty;
                switch (SearchJobSource)
                {
                    case "1":
                        //National Job Bank/WorkBC
                        //These are the jobs provided through the federal XML feed
                        jsonJobSource = "{ \"term\": { \"IsFederalJob\": true } }";
                        break;
                    case "2":
                        //Other job posting websites (e.g. Monster, Workopolis, Indeed)
                        //These are External jobs only and are provided through the external API feed.)
                        jsonJobSource = "{ \"term\": { \"IsFederalJob\": false } }";
                        break;
                    case "3":
                        // federal government
                        //These are an option on the Federal Job Bank and should be available through the federal XML feed.
                        jsonJobSource = "{ \"term\": { \"EmployerTypeId\": 2 } }";
                        break;
                    case "4":
                        // municipal government
                        jsonJobSource = "{ \"term\": {\"EmployerTypeId\": {\"value\": \"4\"}}},{\"nested\": {\"path\": \"ExternalSource\",\"query\": {\"bool\": {\"should\": [{\"match_phrase\": {\"ExternalSource.Source.Source\": \"CivicInfoBC\"}},{\"match_phrase\": {\"ExternalSource.Source.Source\": \"CivicJobs.ca\"}}]}}}}";
                        break;
                    case "5":
                        //BC provincial government
                        //These are jobs posted through the Provincial Government. This option may have to be removed if there is no identifier for provincial govt jobs from the federal XML or the external Job Posting API.
                        jsonJobSource = "{ \"term\": { \"EmployerTypeId\": 3 } }";
                        break;
                }

                if (jsonJobSource != string.Empty)
                {
                    filterGroups.Add(jsonJobSource);
                }
            }


            if (SearchExcludePlacementAgencies)
            {
                //Exclude placement agency
                var jsonExcludePlacementAgency = string.Empty;

                //Where employer type id is NOT 1
                jsonExcludePlacementAgency = "{ \"term\": { \"EmployerTypeId\": 1 } }";

                json = json.Replace("##QUERY_MUST_NOT##", jsonExcludePlacementAgency + ", ##QUERY_MUST_NOT##"); //add filters
            }

            #endregion

            #region Sorting and Pagination

            // geoLocation gets populated when we are applying the location filter above
            json = base.SetPagingAndSorting(json, geoLocation);

            #endregion

            //if one of the filter group options was selected update the query
            if (filterGroups.Any())
            {
                var filterGroupEntry = "{ \"bool\": { \"should\" : [ ##ENTRY## ] } }";
                var groupFilter = "";

                foreach (string filterGroup in filterGroups)
                {
                    groupFilter += filterGroupEntry.Replace("##ENTRY##", filterGroup) + ",";
                }

                //remove trailing comma
                groupFilter = groupFilter.EndsWith(",") ? groupFilter.Substring(0, groupFilter.Length - 1) : groupFilter;
                json = json.Replace("##QUERY_MUST##", groupFilter);
            }

            #region Cleanup

            //clean all placeholders
            json = json.Replace("##SORT##", string.Empty);
            json = json.Replace("##QUERY_MUST##", string.Empty);
            json = json.Replace("##QUERY_MUST_NOT##", string.Empty);

            //Remove unnecessary comma in arrays that result in invalid JSON
            json = RemoveExtraCommas(json);

            #endregion

            #region Return query

            return json;

            #endregion
        }

        /// <summary>
        ///     Get results by querying Elastic Search
        /// </summary>
        public async Task<ElasticSearchResponse> GetSearchResults(string jsonFileName = "jobsearch_main.json", string index = "")
        {
            // Create a request to the elastic search server 
            var server = _configuration["ConnectionStrings:ElasticSearchServer"];
            if (index != General.FrenchIndex)
            {
                index = _configuration["IndexSettings:DefaultIndex"];
            }

            string docType = General.ElasticDocType;

            string url = $"{server}/{index}/{docType}/_search";

            // Apply filter logic 
            SetSortFilters();
            SetFilters();

            string json = await ToJson(_configuration, jsonFileName);

            string jsonResult = await new ElasticHttpHelper(_configuration).QueryElasticSearch(json, url);
            return JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResult);
        }

        /// <summary>
        ///     Set the filter properties based on the values received in the constructor
        /// </summary>
        private void SetFilters()
        {
            //default page size
            RequestedPageSize = _filters.PageSize;
            PageSize = _filters.PageSize;
            PageNumber = _filters.Page;

            //Keyword
            Keyword = _filters.Keyword;

            //Date type
            switch (_filters.SearchDateSelection)
            {
                case "0":
                    //No Date
                    DateSearchType = DateType.NONE;
                    break;
                case "1":
                    //Today
                    DateSearchType = DateType.TODAY;
                    break;
                case "2":
                    //Past 3 days
                    DateSearchType = DateType.PAST_THREE_DAYS;
                    break;
                case "3":
                    //custom date
                    DateSearchType = DateType.DATE_RANGE;

                    //To and From date
                    StartDate = _filters.StartDate != null && _filters.StartDate.Year > 0
                        ? _filters.StartDate.ToString()
                        : "1970-01-01";

                    EndDate = _filters.EndDate != null && _filters.EndDate.Year > 0
                        ? _filters.EndDate.ToString()
                        : DateTime.MaxValue.ToString("yyyy-MM-dd");

                    break;
            }

            //Salary
            SalaryRanges = new List<KeyValuePair<string, string>>();
            if (_filters.SalaryBracket1)
            {
                //Under $40,000
                SalaryRanges.Add(SalaryRangeHelper.GetAnnualRange((SalaryType) _filters.SalaryType, 1));
            }

            if (_filters.SalaryBracket2)
            {
                //$40,000 - $59,999
                SalaryRanges.Add(SalaryRangeHelper.GetAnnualRange((SalaryType) _filters.SalaryType, 2));
            }

            if (_filters.SalaryBracket3)
            {
                //$60,000 - $79,999
                SalaryRanges.Add(SalaryRangeHelper.GetAnnualRange((SalaryType) _filters.SalaryType, 3));
            }

            if (_filters.SalaryBracket4)
            {
                //$80,000 - $99,999
                SalaryRanges.Add(SalaryRangeHelper.GetAnnualRange((SalaryType) _filters.SalaryType, 4));
            }

            if (_filters.SalaryBracket5)
            {
                //$100,000+
                //$100,000 - $100,000,000
                SalaryRanges.Add(SalaryRangeHelper.GetAnnualRange((SalaryType) _filters.SalaryType, 5));
            }

            if (_filters.SalaryBracket6)
            {
                if (!string.IsNullOrEmpty(_filters.SalaryMin))
                {
                    //Custom salary range
                    decimal.TryParse(_filters.SalaryMin, out decimal salaryMin);
                    decimal.TryParse(_filters.SalaryMax, out decimal salaryMax);

                    if (_filters.SalaryType == (int) SalaryType.HOURLY)
                    {
                        salaryMin = salaryMin * 2080;
                        salaryMax = salaryMax * 2080;
                    }
                    else if (_filters.SalaryType == (int) SalaryType.WEEKLY)
                    {
                        salaryMin = salaryMin * 52;
                        salaryMax = salaryMax * 52;
                    }
                    else if (_filters.SalaryType == (int) SalaryType.BI_WEEKLY)
                    {
                        salaryMin = salaryMin * 26;
                        salaryMax = salaryMax * 26;
                    }

                    if (_filters.SalaryType == (int) SalaryType.MONTHLY)
                    {
                        salaryMin = salaryMin * 12;
                        salaryMax = salaryMax * 12;
                    }

                    SalaryRanges.Add(new KeyValuePair<string, string>(
                        $"{decimal.Round(salaryMin, 0)}",
                        $"{decimal.Round(salaryMax, 0)}")
                    );
                }
            }

            switch (_filters.SalaryType)
            {
                case 0:
                    SalarySearchType = SalaryType.HOURLY;
                    break;
                case 1:
                    SalarySearchType = SalaryType.WEEKLY;
                    break;
                case 2:
                    SalarySearchType = SalaryType.BI_WEEKLY;
                    break;
                case 3:
                    SalarySearchType = SalaryType.MONTHLY;
                    break;
                case 4:
                    SalarySearchType = SalaryType.ANNUALLY;
                    break;
                default:
                    SalarySearchType = SalaryType.NONE;
                    break;
            }

            //Industries
            SearchIndustry = _filters.SearchIndustry;

            //More filters
            SearchIsApprentice = _filters.SearchIsApprentice;
            SearchIsIndigenous = _filters.SearchIsIndigenous;
            SearchIsMatureWorkers = _filters.SearchIsMatureWorkers;
            SearchIsNewcomers = _filters.SearchIsNewcomers;
            SearchIsPeopleWithDisabilities = _filters.SearchIsPeopleWithDisabilities;
            SearchIsStudents = _filters.SearchIsStudents;
            SearchIsVeterans = _filters.SearchIsVeterans;
            SearchIsVisibleMinority = _filters.SearchIsVisibleMinority;
            SearchIsYouth = _filters.SearchIsYouth;
            SearchIsPostingsInEnglish = _filters.SearchIsPostingsInEnglish;
            SearchIsPostingsInEnglishAndFrench = _filters.SearchIsPostingsInEnglishAndFrench;
            SearchNocField = _filters.SearchNocField;
            SearchJobSource = _filters.SearchJobSource;
            SearchExcludePlacementAgencies = _filters.SearchExcludePlacementAgencyJobs;

            //Basic filters
            SearchInField = _filters.SearchInField;
            SearchJobTypeCasual = _filters.SearchJobTypeCasual;
            SearchJobTypeDay = _filters.SearchJobTypeDay;
            SearchJobTypeEarly = _filters.SearchJobTypeEarly;
            SearchJobTypeEvening = _filters.SearchJobTypeEvening;
            SearchJobTypeFlexible = _filters.SearchJobTypeFlexible;
            SearchJobTypeFullTime = _filters.SearchJobTypeFullTime;
            SearchJobTypeLeadingToFullTime = _filters.SearchJobTypeLeadingToFullTime;
            SearchJobTypeMorning = _filters.SearchJobTypeMorning;
            SearchJobTypeNight = _filters.SearchJobTypeNight;
            SearchJobTypeOnCall = _filters.SearchJobTypeOnCall;
            SearchJobTypeOvertime = _filters.SearchJobTypeOvertime;
            SearchJobTypePermanent = _filters.SearchJobTypePermanent;
            SearchJobTypePartTime = _filters.SearchJobTypePartTime;
            SearchJobTypeSeasonal = _filters.SearchJobTypeSeasonal;
            SearchJobTypeShift = _filters.SearchJobTypeShift;
            SearchJobTypeTbd = _filters.SearchJobTypeTbd;
            SearchJobTypeTemporary = _filters.SearchJobTypeTemporary;
            SearchJobTypeWeekend = _filters.SearchJobTypeWeekend;
            SearchJobTypeOnSite = _filters.SearchJobTypeOnSite;
            SearchJobTypeHybrid = _filters.SearchJobTypeHybrid;
            SearchJobTypeTravelling = _filters.SearchJobTypeTravelling;
            SearchJobTypeVirtual = _filters.SearchJobTypeVirtual;
            SearchJobEducationLevel = _filters.SearchJobEducationLevel;
            LocationDistance = (LocationDistanceKm) _filters.SearchLocationDistance;
            Locations = _filters.SearchLocations ?? new List<LocationField>();
            SearchSalaryConditions = _filters.SearchSalaryConditions;
        }

        /// <summary>
        ///     Get a list of locations that we can pin on Google Maps
        /// </summary>
        public async Task<List<GoogleMapsPinLocation>> GetGoogleMapResults()
        {
            //Set "Size" in query
            //Current max locations set to 5000
            ElasticSearchResponse esResults = await GetSearchResults("jobsearch_googlemap.json");

            if (esResults != null)
            {
                Source[] jobs = esResults.Hits.HitsHits.Select(hit => hit.Source).ToArray();
                return GetMapPins<GoogleMapsPinLocation>(jobs);
            }

            return new List<GoogleMapsPinLocation>();
        }

        public static List<T> GetMapPins<T>(Source[] jobs, Func<Source, string> contentFunction = null)
        {
            // get the name of the city that appears most frequently in the search results
            string mostFrequentCity = jobs
                .Select(j => j.City)
                .GroupBy(c => c)
                .OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key)
                .FirstOrDefault();

            // get the name of the region that appears most frequently in the search results
            string mostFrequentRegion = jobs
                .Select(j => j.Region is { Length: 1 } ? j.Region[0] : string.Empty)
                .GroupBy(r => r)
                .OrderByDescending(grp => grp.Count())
                .Where(grp => grp.Key != string.Empty)
                .Select(grp => grp.Key)
                .FirstOrDefault();

            var results = new List<T>();

            foreach (Source job in jobs)
            {
                int bestIndex = -1;

                if (job.Location.Length <= 1)
                {
                    bestIndex = 0;
                }
                else
                {
                    // If the job has multiple locations, and one of the locations is in the most
                    // frequent city then we want to pick that one. 
                    // The default zoom/pan of the map is based on the plotted pins. Jobs with multiple
                    // cities can sometimes cause pins to be added in areas completely outside the 
                    // geographic area where the user was searching. 
                    string[] cities;
                    if (job.City == null || job.City.StartsWith("Virtual") || !job.City.Contains(','))
                    {
                        cities = new[] { job.City ?? "" };
                    }
                    else
                    {
                        cities = job.City.Split(',').Select(c => c.Trim()).ToArray();
                    }

                    for (var j = 0; j < cities.Length; j++)
                    {
                        if (cities[j] == mostFrequentCity)
                        {
                            bestIndex = j;
                        }
                    }
                }

                // if we didn't find a best city index, then check the regions next
                if (bestIndex == -1 && job.Region is { Length: > 1 })
                {
                    for (var j = 0; j < job.Region.Length; j++)
                    {
                        if (job.Region[j] == mostFrequentRegion)
                        {
                            bestIndex = j;
                        }
                    }
                }

                for (var i = 0; i < job.Location.Length; i++)
                {
                    // if the job had multiple locations but none of the locations were in the most
                    // frequent city or region then we'll just plot all the locations.
                    if (i == bestIndex || bestIndex == -1)
                    {
                        Location geo = job.Location[i];

                        if (typeof(T) == typeof(GoogleMapsPinLocation))
                        {
                            results.Add((T)(object)new GoogleMapsPinLocation
                            {
                                JobId = job.JobId.ToString(),
                                Latitude = geo.Lat,
                                Longitude = geo.Lon
                            });
                        }
                        else if (typeof(T) == typeof(CareerCompassJobMapLocation))
                        {
                            results.Add((T)(object)new CareerCompassJobMapLocation
                            {
                                Lat = double.Parse(geo.Lat),
                                Lng = double.Parse(geo.Lon),
                                Bounds = true,
                                // ReSharper disable once PossibleNullReferenceException
                                Content = contentFunction(job)
                            });
                        }
                    }
                }
            }

            // jobs with multiple locations can cause more than 5000 map pins.  Truncate the list to
            // 5000 when this happens.
            return results.Count > 5000 ? results.Take(5000).ToList() : results;
        }

        #endregion
    }
}