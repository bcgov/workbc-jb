using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WorkBC.Shared.Utilities
{
    public class ElasticHttpHelper
    {
        private static readonly HttpClient _httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy
                {
                    BypassProxyOnLocal = true
                }
            };
            return new HttpClient(handler);
        }

        private readonly string _username;
        private readonly string _password;
        private readonly AuthenticationHeaderValue _authHeader;

        public ElasticHttpHelper(string username, string password)
        {
            _username = username;
            _password = password;
            _authHeader = BuildAuthHeader(username, password);
        }

        public ElasticHttpHelper(IConfiguration configuration)
            : this(configuration["IndexSettings:ElasticUser"],
                   configuration["IndexSettings:ElasticPassword"])
        {
        }

        private static AuthenticationHeaderValue BuildAuthHeader(string user, string pwd)
        {
            if (string.IsNullOrEmpty(user))
            {
                return null;
            }
            byte[] raw = Encoding.UTF8.GetBytes($"{user}:{pwd}");
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(raw));
        }

        /// <summary>
        /// POST data to ElasticSearch
        /// </summary>
        public async Task<string> PostToElasticSearch(string json, string url, string action = "POST")
        {
            string responseFromServer;

            try
            {
                using var request = new HttpRequestMessage(new HttpMethod(action), url);
                if (action == "POST" || action == "PUT")
                {
                    request.Content = new StringContent(json ?? string.Empty, Encoding.UTF8, "application/json");
                }
                else if (action != "DELETE")
                {
                    throw new NotImplementedException($"ElasticHttpHelper.PostToElasticSearch() does not support the request method {action}");
                }

                if (_authHeader != null)
                {
                    request.Headers.Authorization = _authHeader;
                }

                using HttpResponseMessage result = await _httpClient.SendAsync(request);

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
            catch (WebException ex) when (ex.InnerException?.InnerException is SocketException)
            {
                // rethrow if SocketException
                throw;
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
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            if (_authHeader != null)
            {
                request.Headers.Authorization = _authHeader;
            }

            using HttpResponseMessage result = await _httpClient.SendAsync(request);

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                var maskedPassword = new Regex("\\S").Replace(_password ?? string.Empty, "*");
                throw new Exception($"Elasticsearch returned an Unauthorized status code\n"
                   + $"url={url}\n" + $"user={_username}\n" + $"pwd={maskedPassword}");
            }

            if (!result.IsSuccessStatusCode)
            {
                var responseBody = await result.Content.ReadAsStringAsync();
                throw new Exception($"Elasticsearch returned a {result.StatusCode} status code\n"
                    + $"url={url}\n"
                    + $"response={responseBody}");
            }

            return await result.Content.ReadAsStringAsync();
        }
    }
}
