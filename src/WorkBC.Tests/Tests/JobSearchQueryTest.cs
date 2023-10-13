using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Search.Queries;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.Shared.Services;
using WorkBC.Tests.FakeServices;
using Xunit;

namespace WorkBC.Tests.Tests;

public class JobSearchQueryTest
{
    // properties
    protected readonly IConfiguration Configuration;
    protected readonly IGeocodingService FakeGeocodingService;
    protected readonly string JsonTemplate;
    protected JobSearchFilters Filters; 

    
    private readonly DateField _testDate = new DateField
    {
        Year = 2023,
        Month = 09,
        Day = 28
    };
    
    // constructor
    public JobSearchQueryTest()
    {
        FakeGeocodingService = new FakeGeocodingService(Configuration);
        var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
        var jsonPath = "/WorkBC.ElasticSearch.Search/Resources/jobsearch_main.json";
        JsonTemplate = File.ReadAllText(projectDirectory + jsonPath);
        Filters = new JobSearchFilters();
        Filters.Page = 1;
        Filters.PageSize = 20;
        
    }
    
    [Fact(DisplayName = "User can search jobs by posted date range that includes both the staring and ending dates")]
    public async Task CreateJobSearchFilterForSinglePostDateRange()
    {
        Filters.SearchDateSelection = "3";
        Filters.EndDate = _testDate;
        Filters.StartDate = _testDate;

        var expectedJson = @"{ ""DatePosted"": { ""gte"": ""2023-09-28T00:00:00.000"", ""lte"": ""2023-09-28T23:59:59.999"", ""time_zone"": ""America/Vancouver"" }";
        
        var query = new JobSearchQuery(FakeGeocodingService, Configuration, Filters);
        var results = await query.ToJson(Configuration, JsonTemplate);
        Assert.Contains(expectedJson, results);
    }
    
    [Fact(DisplayName = "If user doesn't supply a starting and ending dates for job search query, API assumes max values")]
    public async Task CreateJobSerchQueryUsingMaxValuesWithNoStartingAndEndingValues()
    {
        Filters.SearchDateSelection = "3";

        var expectedJson = @"{ ""DatePosted"": { ""gte"": ""1970-01-01"", ""lte"": ""9999-12-31"", ""time_zone"": ""America/Vancouver"" }";
        
        var query = new JobSearchQuery(FakeGeocodingService, Configuration, Filters);
        var results = await query.ToJson(Configuration, JsonTemplate);
        Assert.Contains(expectedJson, results);
    }
    
    [Fact(DisplayName = "User can search for jobs by today's posted date")]
    public async Task CreateJobSearchFilterForToday()
    {
        Filters.SearchDateSelection = "1";

        var expectedJson = @"{ ""DatePosted"": { ""gte"": ""now/d"", ""lt"": ""now+1d/d"", ""time_zone"": ""America/Vancouver"" }";
        
        var query = new JobSearchQuery(FakeGeocodingService, Configuration, Filters);
        var results = await query.ToJson(Configuration, JsonTemplate);
        Assert.Contains(expectedJson, results);
    }


    [Fact(DisplayName = "User cannot search for jobs using SearchDateSelection = 0")]
    public async Task CreateJobSearchFilterUsingSearchDateSelectionEqualZero()
    {
        Filters.SearchDateSelection = "0";

        var notExpectedJson = @"{ ""DatePosted"": { ";


        var query = new JobSearchQuery(FakeGeocodingService, Configuration, Filters);
        var results = await query.ToJson(Configuration, JsonTemplate);
        Assert.DoesNotContain(notExpectedJson, results);
    }
    
    [Fact(DisplayName = "User can search for jobs posted in the last 3 days")]
    public async Task CreateJobSearchFilterForPastThreeDays()
    {
        Filters.SearchDateSelection = "2";

        var expectedJson = @"{ ""DatePosted"": { ""gte"": ""now-3d/d"", ""lte"": ""now"", ""time_zone"": ""America/Vancouver"" }";
        
        var query = new JobSearchQuery(FakeGeocodingService, Configuration, Filters);
        var results = await query.ToJson(Configuration, JsonTemplate);
        Assert.Contains(expectedJson, results);
    }
   
}