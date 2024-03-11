using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.Data.Model.Ssot;

namespace WorkBC.Data;

public class SsotApi
{
    private readonly string _ssotApiBaseUrl;
    private readonly HttpClient _httpClient;

    public SsotApi(IConfiguration configuration, HttpClient client)
    {
        _httpClient = client;
        _ssotApiBaseUrl = configuration.GetConnectionString("SsotApiServer");
    }

    public async Task<List<CareerProfile>> GetCareerProfiles(List<int> careerProfileIds)
    {
        try
        {
            using (_httpClient)
            {
                var commaList = String.Join(",", careerProfileIds);
                var endpoint = $"/career_profile?CareerProfileId=in.({commaList})";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CareerProfile>>(jsonString);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return new List<CareerProfile>();
        
    }
    
    public async Task<int> GetCareerProfileIdByNoc(string nocCode)
    {
        try
        {
            using (_httpClient)
            {
                var endpoint = $"/career_profile?Noccode=eq.{nocCode}&limit=1";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CareerProfile>>(jsonString)[0].CareerProfileId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return 0;

    }
    
    public async Task<List<IndustryProfile>> GetIndustryProfiles(List<int> industryProfileIds)
    {
        try
        {
            using (_httpClient)
            {
                var commaList = String.Join(",", industryProfileIds);
                var endpoint = $"/industry_profile?IndustryProfileId=in.({commaList})";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<IndustryProfile>>(jsonString);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return new List<IndustryProfile>();
        
    }
    
    public async Task<int> GetIndustryProfileIdByNaics(string naics)
    {
        try
        {
            using (_httpClient)
            {
                var endpoint = $"/industry_profile?naics_id=eq.{naics}&limit=1";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<IndustryProfile>>(jsonString)[0].IndustryProfileId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return 0;

    }
    
}