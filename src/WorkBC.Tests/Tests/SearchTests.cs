using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace WorkBC.Tests.Tests
{
    public class SearchTests : TestsBase
    {
        //Properties
        private readonly SearchTestHelpers _searchHelper;

        public SearchTests(ITestOutputHelper output) : base(output)
        {
            _searchHelper = new SearchTestHelpers(GeocodingService, Configuration);
        }

        [Theory(DisplayName = "Find a Federal and Wanted job in the index")]
        [InlineData(31382167)]
        [InlineData(4049109571)]
        public async Task FindJobById(long jobId)
        {
            List<Source> result = await _searchHelper.GetJobsById(jobId);

            Assert.True(result.Count > 0, $"Job ID {jobId} did not return any results");
        }

        [Theory(DisplayName = "Find job in different employment groups")]
        [InlineData(false, false, false, false, false, false, false, false, true)] //Youth
        [InlineData(false, false, false, false, false, false, false, true, false)] //Visible minority
        [InlineData(false, false, false, false, false, false, true, false, false)] //Veterans
        [InlineData(false, false, false, false, false, true, false, false, false)] //Students
        [InlineData(false, false, false, false, true, false, false, false, false)] //People with disabilities
        [InlineData(false, false, false, true, false, false, false, false, false)] //Newcomers
        [InlineData(false, false, true, false, false, false, false, false, false)] //Mature workers
        [InlineData(false, true, false, false, false, false, false, false, false)] //Indigenous people
        [InlineData(true, false, false, false, false, false, false, false, false)] //Apprentices
        public async Task FindJobsInEmploymentGroups(bool isApprentice, bool isIndigenous, bool isMatureWorker,
            bool isNewcomer, bool isPeopleWithDisabilities, bool isStudents, bool isVeterans, bool isVisibleMinority,
            bool isYouth)
        {
            List<Source> result = await _searchHelper.GetJobsWithEmploymentGroup(isApprentice, isIndigenous,
                isMatureWorker, isNewcomer, isPeopleWithDisabilities, isStudents, isVeterans, isVisibleMinority,
                isYouth);

            Assert.True(result.Count > 0, "Job for employment group did not return any results");
        }

        [Theory(DisplayName = "Find jobs based on language preference on more filters")]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task FindJobByLanguagePreference(bool englishOnly, bool englishAndFrench)
        {
            List<Source> result = await _searchHelper.GetJobsByLanguagePreference(englishOnly, englishAndFrench);

            Assert.True(result.Count > 0,
                "Jobs for " + (englishOnly ? "English only" : "English and French") + " did not return any results");
        }

        [Theory(DisplayName = "Find jobs based on a NOC code")]
        [InlineData("6322")]
        public async Task FindJobByNocCode(string nocCode)
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobNocField(nocCode));

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            //We have 2 jobs with this NOC code in the fixtures
            Assert.True(result.Count == 3,
                $"Job for NOC code {nocCode} did not return 3 results, but {result.Count} results");
        }

        [Theory(DisplayName = "Find jobs based on a NOC code 2021 from a federal source")]
        [InlineData("21234")]
        public async Task FindJobByNocCode2021Federal(string nocCode2021)
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobNocField2021(nocCode2021));

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            //We have 1 jobs with this NOC code in the fixtures
            Assert.True(result.Count == 1,
                $"Job for NOC code {nocCode2021} returned {result.Count} results");
        }

        //[Theory(DisplayName = "Find jobs based on a NOC code 2021 derived from a TalentNeuron source")]
        //[InlineData("11202")]
        //[InlineData("74102")]
        //[InlineData("52120")]
        //[InlineData("11100")]
        //public async Task FindJobByNocCode2021Wanted(string nocCode2021)
        //{
        //    //Create an instance with the filters required
        //    var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobNocField2021(nocCode2021));

        //    //return results
        //    List<Source> result = await QueryElasticSearch(esq);

        //    //We have 1 jobs with this NOC code in the fixtures
        //    Assert.True(result.Count == 1,
        //        $"Job for 2021 NOC code {nocCode2021} returned {result.Count} results");
        //}

        [Theory(DisplayName = "Find jobs based on job sources")]
        [InlineData("1")] //National Job Bank/WorkBC
        [InlineData("2")] //Other job posting websites (e.g. Monster, Workopolis, Indeed)
        [InlineData("3")] //BC federal government
        [InlineData("4")] //BC municipal government (Not currently available, made a copy of another job and changed the type)
        [InlineData("5")] //BC provincial government
        public async Task FindJobsByJobSource(string sourceId)
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobSource(sourceId));

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            Assert.True(result.Count > 0, $"Job for Job Source code {sourceId} did not return results");
        }

        [Theory(DisplayName = "Search jobs by all text fields")]
        [InlineData("Enumerator")]
        [InlineData("GPS")]
        [InlineData("Bilingual Absolute")]
        public async Task SearchJobByAllFields(string searchText)
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsWithAll(searchText));

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            //There should be results
            Assert.True(result.Count > 0, $"Jobs for \"{searchText}\" across all fields did not return results");
        }

        [Theory(DisplayName = "Find jobs based on city or postal code")]
        [InlineData("", "", "V3R 8X2", 10)] //10km radius
        [InlineData("", "", "V3R8X2", 10)] //10km radius
        [InlineData("", "North Vancouver", "", -1)] //exact match
        [InlineData("", "North Vancouver", "", 15)] //15 Km radius

        //Not sure how to test this at this stage.
        //Can't find examples of these to test with
        //[InlineData("Cariboo", "Summit Lake", "")]
        //[InlineData("Kootenay", "Summit Lake", "")]
        public async Task FindJobByPostalOrCity(string region, string city, string postal, int radius)
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsByCityOrPostal(region, city, postal, radius));

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            Assert.True(result.Count > 0, "Jobs for (" + city + "," + region + ") did not return any results");
        }

        [Fact(DisplayName = "Find jobs based on multiple cities")]
        public async Task FindJobByMultipleCities()
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsByMultipleCities());

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            Assert.True(result.Count > 0, "Jobs for list of cities did not return any results");
        }

        [Fact(DisplayName = "Find jobs based on multiple postal codes")]
        public async Task FindJobByMultiplePostalCodes()
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsByMultiplePostalCodes());

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            Assert.True(result.Count > 0, "Jobs for list of cities did not return any results");
        }

        [Fact(DisplayName = "Find a job in Surrey")]
        public async Task FindJobWithByCity()
        {
            List<Source> result = await _searchHelper.GetJobsWithCity("Surrey");

            Assert.True(result.Count > 0, "Jobs in Surrey did not return any results");
        }

        [Fact(DisplayName = "Search job with by job title 'long haul truck driver'")]
        public async Task SearchJobByTitle()
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsWithTitle());

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            //There should be results
            Assert.True(result.Count == 1, "Jobs for job title 'long haul truck driver' did not return results");
        }

        [Fact(DisplayName = "Search job with employer name 'Persia Food Products Inc.'")]
        public async Task SearchJobForEmployer()
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsWithEmployer());

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            //There should be results
            Assert.True(result.Count == 1, "Jobs for employer 'Persia Food Products Inc.' did not return results");
        }

        [Fact(DisplayName = "Exclude placement agencies for jobs. Job 30696242 should NOT be in the list of jobs")]
        public async Task SearchJobsExcludingPlacementAgencyJobs()
        {
            //Create an instance with the filters required
            var esq = new JobSearchQuery(GeocodingService, Configuration, GetFiltersForJobsExcludingPlacementAgencies());

            //return results
            List<Source> result = await QueryElasticSearch(esq);

            //ensure that specific job is not in result set
            Assert.DoesNotContain(result, j => j.JobId == 30696242);

            //There should be results
            Assert.True(result.Count > 0, "Jobs excluding agency jobs did not return results");
        }
        
        [Theory(DisplayName = "When both salary string and salary numeric fields populated, calculate SalarySummary")]
        [InlineData(39879152, "$2,000.00 monthly + 20% commission per sale")]
        [InlineData(40098629, "$2,000.00 monthly + 3% commission per sale")]
        [InlineData(40104984, "$2,000.00 monthly + 3% commission per sale")]
        [InlineData(40207035, "$27.50 hourly")]
        [InlineData(40387120, "$46.50 hourly")]
        [InlineData(6111648176, "$82,795 annually")]
        public async Task TestSalarySummaryCalculated(long jobId, string expected)
        {
            List<Source> result = await _searchHelper.GetJobsById(jobId);
            Assert.Equivalent( expected, result[0].SalarySummary);
        }

        #region Filter setup

        private JobSearchFilters GetFiltersForJobNocField(string nocCode)
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchNocField = nocCode
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobNocField2021(string nocCode2021)
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchNoc2021Field = nocCode2021
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobSource(string sourceId)
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchJobSource = sourceId
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsExcludingPlacementAgencies()
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchExcludePlacementAgencyJobs = true
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsWithEmployer()
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchInField = "Employer",
                Keyword = "Persia Food Products Inc."
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsWithTitle()
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchInField = "Title",
                Keyword = "long haul truck driver"
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsWithAll(string keyword)
        {
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchInField = "All",
                Keyword = keyword
            };

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsByCityOrPostal(string region, string city, string postal,
            int radius)
        {
            var lf = new LocationField
            {
                City = city,
                Postal = postal,
                Region = region
            };

            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchInField = "All",
                SearchLocationDistance = radius
            };

            filters.SearchLocations.Add(lf);

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsByMultipleCities()
        {
            var surrey = new LocationField
            {
                City = "Surrey",
                Postal = string.Empty,
                Region = string.Empty
            };

            var chilliwack = new LocationField
            {
                City = "Chilliwack",
                Postal = string.Empty,
                Region = string.Empty
            };

            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchInField = "All",
                SearchLocationDistance = -1 //exact match
            };

            filters.SearchLocations.Add(surrey);
            filters.SearchLocations.Add(chilliwack);

            return filters;
        }

        private JobSearchFilters GetFiltersForJobsByMultiplePostalCodes()
        {
            var lfPostal1 = new LocationField
            {
                City = string.Empty,
                Postal = "V3R8X2",
                Region = string.Empty
            };

            var lfPostal2 = new LocationField
            {
                City = string.Empty,
                Postal = "V7M1R5",
                Region = string.Empty
            };

            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchInField = "All",
                SearchLocationDistance = -1 //exact match
            };

            filters.SearchLocations.Add(lfPostal1);
            filters.SearchLocations.Add(lfPostal2);

            return filters;
        }

        #endregion
    }
}