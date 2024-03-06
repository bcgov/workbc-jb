using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WorkBC.Data;

public class SsotApi
{
    private string SsotApiBaseUrl;
    private HttpClient HttpClient;

    public SsotApi(IConfiguration configuration, HttpClient client)
    {
        HttpClient = client;
        SsotApiBaseUrl = configuration.GetConnectionString("SsotApiServer");
    }

    public async Task<List<object>> GetNocsByCareerProfileIds(List<int> ids)
    {
        // call out to a new /career_profile endpoint on the SSOT API
        
        /*
         
        The API needs a view that returns the following:  

        SELECT p.careerprofileid, n.nameenglish, n.noccode FROM edm_careerprofile p
        INNER JOIN edm_noc n ON p.noc_id = n.noc_id

        WHERE p.careerprofileid IN (555,33, 390)
        ORDER BY n.NameEnglish

         */
        return new List<object>(
        );
    }
    
    public async Task<int> GetCareerProfileByNoc(string nocCode)
    {
        // call out to a /career_profile endpoint on the SSOT API
        
        /*
         
        The API needs a view that returns the following: 
        
        SELECT p.careerprofileid, n.nameenglish, n.noccode from edm_careerprofile p
        INNER JOIN edm_noc n on p.noc_id = n.noc_id
        WHERE n.noccode = '0632'
        LIMIT 1;
         
        */
        return 13;
    }
    
    public async Task<List<object>> GetNocsByIndustryProfileIds(List<int> ids)
    {
        // call out to a new /industry_profile endpoint on the SSOT API
        return new List<object>(
            
        );
    }
    
    public async Task<int> GetIndustryProfileByNoc(string nocCode)
    {
        // call out to a /industry_profile endpoint on the SSOT API
        return 13;
    }
    
}