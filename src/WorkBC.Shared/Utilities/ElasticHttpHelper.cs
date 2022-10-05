using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WorkBC.Shared.Utilities
{
    public class ElasticHttpHelper
    {
        private string _username;
        private string _password;

        public ElasticHttpHelper(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public ElasticHttpHelper(IConfiguration configuration)
        {
            _username = configuration["IndexSettings:ElasticUser"];
            _password = configuration["IndexSettings:ElasticPassword"];
        }

        /// <summary>
        /// POST data to ElasticSearch
        /// </summary>
        public async Task<string> PostToElasticSearch(string json, string url, string action = "POST")
        {
            string responseFromServer;
            StringContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var handler = new HttpClientHandler
                {
                    // ElasticSearch is on localhost:9200.  Dotnet Core will try to use the proxy unless 
                    // we add this line.  
                    Proxy = new WebProxy
                    {
                        BypassProxyOnLocal = true
                    }
                };

                // add credentials for basic authentication
                if (!string.IsNullOrEmpty(_username))
                {
                    handler.Credentials = new NetworkCredential(_username, _password);
                }

                using (var httpClient = new HttpClient(handler))
                {
                    HttpResponseMessage result;
                    

                    if (action == "DELETE")
                    {
                        result = await httpClient.DeleteAsync(url);
                    }
                    else if (action == "PUT")
                    {
                        result = await httpClient.PutAsync(url, jsonContent);
                    }
                    else if (action == "POST")
                    {
                        result = await httpClient.PostAsync(url, jsonContent);
                    }
                    else
                    {
                        throw new NotImplementedException($"ElasticHttpHelper.PostToElasticSearch() does not support the request method {action}");
                    }

                    if (result.IsSuccessStatusCode)
                    {
                        //read the response from the server
                        responseFromServer = result.ReasonPhrase;
                    }
                    else if (result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //unauthorized
                        Console.Write("[401]");
                        throw new UnauthorizedAccessException("Unable to connect to ElasticSearch Basic Auth. Please ensure the correct username and password are specified in appsettings.json");
                    } 
                    else 
                    {
                        //record not found in elastic search
                        Console.Write("[404]");
                        responseFromServer = "[404]";
                    }
                }
            }
            catch (WebException ex) when (ex.InnerException?.InnerException is SocketException)
            {
                // rethrow if SocketException
                throw;
                // note: The message should be "Only one usage of each socket address (protocol/network address/port) is normally permitted"
            }
            catch (Exception ex)
            {
                if (action.ToLower().Equals("delete"))
                {
                    //this will be index not found (404)
                    //Error|404|JobId
                    responseFromServer = $"[ERROR|404|{url}]";
                }
                else
                {
                    Console.WriteLine("ERROR: COULD NOT POST DATA TO ELASTIC SEARCH. PostToElasticSearch() " + ex.ToString());
                    responseFromServer = "ERROR";
                }
            }

            return responseFromServer;
        }

        public async Task<string> QueryElasticSearch(string json, string url)
        {
            StringContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            var handler = new HttpClientHandler
            {
                // ElasticSearch is on localhost:9200.  Dotnet Core will try to use the proxy unless 
                // we add this line.  
                Proxy = new WebProxy
                {
                    BypassProxyOnLocal = true
                }
            };

            // add credentials for basic authentication
            if (!string.IsNullOrEmpty(_username))
            {
                handler.Credentials = new NetworkCredential(_username, _password);
            }

            using (var httpClient = new HttpClient(handler))
            {
                HttpResponseMessage result;
                
                result = await httpClient.PostAsync(url, jsonContent);

                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception($"Elasticsearch returned a {result.StatusCode} status code");
                }

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}