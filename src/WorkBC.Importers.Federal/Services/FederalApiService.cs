using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Serilog;
using WorkBC.Importers.Federal.Models;

namespace WorkBC.Importers.Federal.Services
{
    public class FederalApiService
    {
       private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public FederalApiService(HttpClient httpClient, string baseUrl, ILogger logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        /// <summary>
        ///     Get XML data from URL response
        /// </summary>
        private async Task<XmlDocument> CallApiTwiceIfNecessary(string url)
        {
            var xmlData = new XmlDocument();
            
            // first attempt
            var (isSuccess, responseFromServer) = GetApiResponse(url).Result;
            
            // second attempt, if necessary
            if (!isSuccess)
            {
                Console.Write("-WAIT_5_SECONDS-");
                await Task.Delay(5000); // wait 5 seconds and try again
                (isSuccess, responseFromServer) = GetApiResponse(url).Result;
            }

            if (!isSuccess) return null;
            
            xmlData.LoadXml(responseFromServer);
            return xmlData;
        }

        private async Task<(bool, string)> GetApiResponse(string url)
        {
            try
            {
                //Read the web response from URL
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                //Return successful response
                return (true, await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                //Return unsuccessful response
                _logger.Error("URL: " + url);
                _logger.Error("STACKTRACE: " + ex);
                _logger.Error("RESPONSE: " + ex.Message);
                return (false, ex.Message);
            }
        }

        /// <summary>
        ///     Get a list of all the job Id's available on the federal website.
        /// </summary>
        public async Task<List<JobPosting>> GetAllJobPostingItems()
        {
            var lstJobPostings = new List<JobPosting>();

            try
            {
                //get all job posting ID's from URL
                const string province = "/en/bc";
                var url = _baseUrl + province + "?includevirtual=true";
                var xmlData = await CallApiTwiceIfNecessary(url);

                //loop through nodes and create a list of "JobPosting" objects
                if (xmlData != null)
                {
                    //Get the root element
                    XmlElement root = xmlData.DocumentElement;

                    //Number of jobs in this XML
                    var numberOfJobsFound =
                        Convert.ToInt32(root.SelectSingleNode("/SolrResponse/Header/numFound").InnerText);

                    if (numberOfJobsFound > 0)
                    {
                        //Find all documents
                        XmlNodeList nodes = root.SelectNodes("/SolrResponse/Documents/Document");

                        //loop through all documents
                        foreach (XmlNode node in nodes)
                        {
                            //create new JobPosting
                            var jp = new JobPosting
                            {
                                FileUpdateDate = Convert.ToDateTime(node["file_update_date"].InnerText),
                                Id = Convert.ToInt32(node["jobs_id"].InnerText)
                            };

                            //add to list to return
                            lstJobPostings.Add(jp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - GetAllJobPostingItems() : " + ex.Message);
            }

            //return list
            return lstJobPostings;
        }

        /// <summary>
        ///     Get XML for english and french job details from Federal Job API
        /// </summary>
        public async Task<(XmlDocument, XmlDocument)> GetEnglishAndFrenchJobDetails(long jobId)
        {
            var tasks = new Task<XmlDocument>[2];
            
            tasks[0] = CallApiTwiceIfNecessary($"{_baseUrl}/en/{jobId}.xml");
            tasks[1] = CallApiTwiceIfNecessary($"{_baseUrl}/fr/{jobId}.xml");
            
            await Task.WhenAll(tasks);
            return (tasks[0].Result, tasks[1].Result);
        }

        public static DateTime? GetXmlDisplayUntil(string xml)
        {
            //return value - can be null
            DateTime? dt = null;

            //Xml document used to read xml
            var xmlDoc = new XmlDocument();

            //Load XML data in object
            xmlDoc.LoadXml(xml);

            if (xmlDoc.ChildNodes.Count > 0)
            {
                //Get the root element
                XmlElement root = xmlDoc.DocumentElement;

                //Number of jobs in this XML
                //It should be 1
                var numberOfJobsFound =
                    Convert.ToInt32(root.SelectSingleNode("/SolrResponse/Header/numFound").InnerText);

                if (numberOfJobsFound == 1)
                {
                    //Read XML Node
                    XmlNode xmlJobNode = root.SelectSingleNode("/SolrResponse/Documents/Document");

                    //If we have a node
                    if (xmlJobNode != null)
                    {
                        //set date object
                        dt = Convert.ToDateTime(xmlJobNode["display_until"].InnerText);
                    }
                }
            }

            //return object
            return dt;
        }
    }
}