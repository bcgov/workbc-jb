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

    public async Task<List<SavedCareerProfile>> GetSavedCareerProfiles(Dictionary<int, int> savedCareerProfilesDictionary)
    {
        try
        {
            using (_httpClient)
            {
                var commaList = String.Join(",", savedCareerProfilesDictionary.Select(s => s.Key).ToList());
                var endpoint = $"/career_profile?CareerProfileId=in.({commaList})";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var careerProfiles = JsonConvert.DeserializeObject<List<CareerProfile>>(jsonString);
                    
                    var savedCareerProfiles = new List<SavedCareerProfile>();

                    if (careerProfiles.Count > 0)
                    {
                        foreach (var careerProfile in careerProfiles)
                        {
                            savedCareerProfiles.Add(new SavedCareerProfile()
                            {
                                Id = savedCareerProfilesDictionary[careerProfile.CareerProfileId],
                                Title = careerProfile.NameEnglish,
                                NocCode = careerProfile.Noccode
                            });
                        }
                    }

                    return savedCareerProfiles;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return new List<SavedCareerProfile>();
        
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
    
    public async Task<List<SavedIndustryProfile>> GetSavedIndustryProfiles(Dictionary<int, int> savedIndustryProfilesDict)
    {
        try
        {
            using (_httpClient)
            {
                var commaList = String.Join(",", savedIndustryProfilesDict.Keys.ToList());
                var endpoint = $"/industry_profile?IndustryProfileId=in.({commaList})";
                var response = await _httpClient.GetAsync(_ssotApiBaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var industryProfiles = JsonConvert.DeserializeObject<List<IndustryProfile>>(jsonString);

                    var savedIndustryProfiles = new List<SavedIndustryProfile>();

                    if (industryProfiles.Count > 0)
                    {
                        foreach (var industryProfile in industryProfiles)
                        {
                            savedIndustryProfiles.Add(new SavedIndustryProfile()
                            {
                                Id = savedIndustryProfilesDict[industryProfile.IndustryProfileId],
                                Title = industryProfile.PageTitle
                            });
                        }
                    }

                    return savedIndustryProfiles;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return new List<SavedIndustryProfile>();
        
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
        /*

         SELECT IndustryProfileId FROM edm_industryprofiles
         WHERE NaicsId == naics
         LIMIT 1;

        */

    }
    
}