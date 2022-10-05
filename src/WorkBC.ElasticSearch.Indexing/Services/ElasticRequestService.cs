using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Nito.AsyncEx.Synchronous;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class ElasticRequestService
    {
        private string _username;
        private string _password;

        public ElasticRequestService(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public ElasticRequestService(IConfiguration configuration)
        {
            _username = configuration["IndexSettings:ElasticUser"];
            _password = configuration["IndexSettings:ElasticPassword"];
        }

        public bool Send(string url, string json, string method)
        {
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

                    if (method == "DELETE")
                    {
                        result = httpClient.DeleteAsync(url).WaitAndUnwrapException();
                    } 
                    else if  (method == "PUT")
                    {
                        result = httpClient.PutAsync(url, jsonContent).WaitAndUnwrapException();
                    }
                    else if (method == "POST")
                    {
                        result = httpClient.PostAsync(url, jsonContent).WaitAndUnwrapException();
                    } 
                    else
                    {
                        throw new NotImplementedException($"ElasticRequestService.Send() does not support the request method {method}");
                    }

                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine();
                if (method == "DELETE")
                {
                    Console.WriteLine("Index was not removed (it probably doesn't exist)");
                }
                else
                {
                    Console.WriteLine("Error sending request to ElasticSearch server. Reason: " + ex.Message);
                    Console.WriteLine("Index: " + url);
                    Console.WriteLine("Method: " + method);
                }

                Console.WriteLine();

                return false;
            }
        }
    }
}