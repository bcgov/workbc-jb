using System;
using System.Collections.Generic;
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

    public async Task<List<CareerProfile>> GetNocsByCareerProfileIds(List<int> careerProfileIds)
    {
        try
        {
            using (_httpClient)
            {
                var commaList = String.Join(",", careerProfileIds);
                var endpoint = $"/career_profile?Id=in.({commaList})";
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
        
        /*
         
        The API needs a view that returns the following:  

        SELECT p.careerprofileid, n.nameenglish, n.noccode FROM edm_careerprofile p
        INNER JOIN edm_noc n ON p.noc_id = n.noc_id
        WHERE p.careerprofileid IN (careerProfileIds)
        ORDER BY n.NameEnglish

         */
        
    }
    
    public async Task<int> GetCareerProfileIdByNoc(string nocCode)
    {
        try
        {
            using (_httpClient)
            {
                var endpoint = $"/career_profile?NocCode=eq.{nocCode}&limit=1";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CareerProfile>>(jsonString)[0].Id;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return 0;
        
        /*
         
        The API needs a view that returns the following: 
        
        SELECT p.careerprofileid, n.nameenglish, n.noccode from edm_careerprofile p
        INNER JOIN edm_noc n on p.noc_id = n.noc_id
        WHERE n.noccode = '0632'
        LIMIT 1;
         
        */
        return 13;
    }
    
    public async Task<List<IndustryProfile>> GetNocsByIndustryProfileIds(List<int> industryProfileIds)
    {
        // call out to the /industry_profile endpoint on the SSOT API
        
        try
        {
            using (_httpClient)
            {
                var commaList = String.Join(",", industryProfileIds);
                var endpoint = $"/industry_profile?Id=in.({commaList})";
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
        
        /*
        
        SELECT industryprofileid, pagetitle FROM edm_industryprofiles p
        WHERE IndustryProfileId IN (industryProfileIds) 
        ORDER BY p.PageTitle
         
        */
        
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
                    return JsonConvert.DeserializeObject<List<IndustryProfile>>(jsonString)[0].Id;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return 0;
        /*

         SELECT IndustryProfileId FROM edm_industryprofiles
         WHERE NaicsId == naics
         LIMIT 1;

        */

    }
    
}