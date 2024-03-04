using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class XmlParsingServiceWanted : XmlParsingServiceBase
    {
        public XmlParsingServiceWanted(IConfiguration configuration) : base(configuration)
        {
        }

        public XmlParsingServiceWanted(List<Data.Model.JobBoard.Location> duplicateCities, Dictionary<string, string> uniqueCities, List<NocCode> nocCodes ): base(duplicateCities, uniqueCities, nocCodes)
        {
        }

        public ElasticSearchJob ConvertToElasticJob(string wantedXml)
        {
            var job = new ElasticSearchJob();

            try
            {
                //read XML to XmlDocument
                var jobXml = new XmlDocument();

                //Load job xml 
                jobXml.LoadXml(wantedXml);

                if (jobXml.ChildNodes.Count > 0)
                {
                    //Read XML Node
                    XmlNode xmlJobNode = jobXml.SelectSingleNode("job");

                    if (xmlJobNode != null)
                    {
                        //Read Source
                        //Do not import "Government of Canada" jobs
                        string jobSource = xmlJobNode.SelectSingleNode("sources/source")?.Attributes["name"]?.InnerText ?? "";

                        bool isFederal = jobSource.ToLower().Equals("government of canada");
                        bool isWorkBc = jobSource.ToLower().Equals("workbc");
                        bool isCcp = jobSource.ToLower().Equals("canadian consumer panels");

                        if (!(jobSource == "" || isFederal || isWorkBc || isCcp))
                        {
                            //add to new Model
                            XmlNode nocNode = xmlJobNode.SelectSingleNode("occupation/noc");

                            string refreshedText = xmlJobNode.SelectSingleNode("dates").Attributes["refreshed"].InnerText;
                            var refreshedDate = Convert.ToDateTime(refreshedText + "T00:00:00-08:00");

                            string postedText = xmlJobNode.SelectSingleNode("dates").Attributes["posted"].InnerText;
                            var postedDate = Convert.ToDateTime(postedText + "T00:00:00-08:00");

                            string city = xmlJobNode.SelectSingleNode("locations/location/city").Attributes["label"].InnerText;
                            XmlAttribute provinceAttribute = xmlJobNode.SelectSingleNode("locations/location/state").Attributes["label"];

                            // call CleanUpCityName to apply XmlManualOverrides
                            city = CleanUpCityName(city);

                            job = new ElasticSearchJob
                            {
                                //SHARED
                                JobId = xmlJobNode.Attributes["id"].InnerText,
                                // -08:00 will ensure the correct date for both PST and PDT.  All that really matters is the date.
                                DatePosted = refreshedDate, // we display "refreshed" date and call it "posted" date because some wanted jobs are really old
                                LastUpdated = refreshedDate,
                                ActualDatePosted = postedDate,
                                Lang = "en",
                                SkillCategories = new List<SkillCategory>(),
                                City = new[] { GetJobCity(city) },
                                Location = new Location[] { },
                                Province =
                                    provinceAttribute != null ? provinceAttribute.InnerText.Trim() : string.Empty,
                                Region = new[] { GetJobRegion(city, 0, UniqueCities, DuplicateCities) },
                                HoursOfWork = new HoursOfWork(),
                                PeriodOfEmployment = new PeriodOfEmployment(),
                                EmploymentTerms = new EmploymentTerms(),
                                IsFederalJob = false,
                                ExpireDate = refreshedDate.AddDays(WantedJobExpiryDays),

                                //WANTED
                                Industry = xmlJobNode.SelectSingleNode("industry").Attributes["label"].InnerText,
                                JobDescription = xmlJobNode.SelectSingleNode("description").Attributes["value"].InnerText,
                                Occupation = nocNode == null ? string.Empty : nocNode.Attributes["label"].InnerText,
                                Function = xmlJobNode.SelectSingleNode("function").Attributes["label"].InnerText
                            };

                            #region Employer Name

                            //Test the employer name for english characters (we want to exclude non-english names)
                            var regEnglishWord = new Regex("[a-zA-Z0-9$@!%*?&#^_.+-]+");

                            XmlNode employerNode = xmlJobNode.SelectSingleNode("employer");

                            if (employerNode.Attributes["superalias"] != null)
                            {
                                //Try to match with the superalias 
                                Match isMatch = regEnglishWord.Match(employerNode.Attributes["superalias"].InnerText.Trim());

                                if (isMatch.Success)
                                {
                                    //set the employer name to the superalias
                                    job.EmployerName = employerNode.Attributes["superalias"].InnerText.Trim();
                                }
                            }

                            //if we could not set the employer name with the superalias we will try the name attribute
                            if (job.EmployerName != null && job.EmployerName == string.Empty)
                            {
                                //Use the "name" attribute for the employer name field
                                if (employerNode.Attributes["name"] != null &&
                                    employerNode.Attributes["name"].InnerText.Trim() != string.Empty)
                                {
                                    job.EmployerName = employerNode.Attributes["name"].InnerText.Trim();
                                }
                                else
                                {
                                    job.EmployerName = string.Empty;
                                }
                            }

                            #endregion

                            #region Salary

                            //Only import where salary of type "POSTED", ignore other types ("Modeled") as per current site.
                            XmlNodeList salaryNode = xmlJobNode.SelectNodes("salaries/salary");
                            if (salaryNode.Count > 0)
                            {
                                foreach (XmlNode salary in salaryNode)
                                {
                                    if (salary.Attributes["type"].Value.ToLower().Equals("posted"))
                                    {
                                        decimal salaryAmount = decimal.Parse(salary.Attributes["value"].Value);
                                        job.Salary = salaryAmount;
                                        job.SalarySummary = $"${salaryAmount:C0} annually";
                                    }
                                }
                            }

                            if ((job.Salary ?? 0m) < 0.01m)
                            {
                                job.SalarySummary = "N/A";
                            }

                            #endregion

                            #region Hours of work

                            job.HoursOfWork.Description = new List<string>();
                            foreach (XmlNode jobType in xmlJobNode.SelectNodes("jobtypes/jobtype"))
                            {
                                string jobTypeStr = jobType.Attributes["label"].InnerText;
                                switch (jobTypeStr.ToLower())
                                {
                                    case "full-time":
                                    case "part-time":
                                        job.HoursOfWork.Description.Add(jobType.Attributes["label"].InnerText
                                            .Replace("-Time", "-time"));
                                        break;
                                }
                            }

                            #endregion

                            #region Period of Employment

                            job.PeriodOfEmployment.Description = new List<string>();
                            foreach (XmlNode jobType in xmlJobNode.SelectNodes("jobtypes/jobtype"))
                            {
                                string jobTerm = jobType.Attributes["label"].InnerText;
                                switch (jobTerm.ToLower())
                                {
                                    case "permanent":
                                    case "contract":
                                    case "temporary":
                                        job.PeriodOfEmployment.Description.Add(jobType.Attributes["label"].InnerText);
                                        break;
                                }
                            }

                            #endregion

                            #region Location

                            XmlNode geoNode = xmlJobNode.SelectSingleNode("locations/location/position");
                            Location location = Location.LocationOrNull(
                                geoNode.Attributes["latitude"].InnerText,
                                geoNode.Attributes["longitude"].InnerText
                            );

                            if (location != null)
                            {
                                //Only add valid long and lat else Elastic search breaks since this "LocationGeo" is a field of type "geo_point"
                                if (location.Lat.StartsWith("999") && location.Lon.StartsWith("999"))
                                {
                                    location.Lat = "54.5000992";
                                    location.Lon = "-125.1159973";
                                }

                                job.Location = new[] { location };

                                job.LocationGeo = new[]
                                    { $"{location.Lat},{location.Lon}" }; //lat,long ("geo_point" field)
                            }

                            #endregion

                            #region Education

                            var id = Convert.ToInt32(
                                xmlJobNode.SelectSingleNode("education").Attributes["id"].InnerText);

                            switch (id)
                            {
                                case 1: // Less than high school
                                    job.EduLevel = "No education";
                                    break;
                                case 4: // Some college, no degree"
                                case 6: // Associate's degree
                                case 7: // Postsecondary nondegree award
                                    job.EduLevel = "College or apprenticeship";
                                    break;
                                case 2: // Doctoral or professional degree
                                case 3: // Master's degree
                                case 5: // Bachelor's degree
                                    job.EduLevel = "University";
                                    break;
                                case 8: // High school diploma or equivalent
                                    job.EduLevel = "Secondary school or job-specific training";
                                    break;
                            }

                            #endregion

                            #region Apply fields

                            job.ApplyEmailAddress = string.Empty;
                            job.ApplyPhoneNumber = string.Empty;
                            job.ApplyWebsite = string.Empty;
                            job.PositionsAvailable = 1;
                            job.NaicsId = null;

                            #endregion

                            #region Noc code

                            string noc = nocNode == null ? "" : nocNode.Attributes["code"].InnerText;
                            var nocInt = 0;
                            if (!string.IsNullOrEmpty(noc))
                            {
                                //conver to int
                                int.TryParse(noc, out nocInt);

                                // make sure the code is valid
                                if (NocCodes.All(c => c.Id != nocInt))
                                {
                                    nocInt = 0;
                                }

                                //set job noc code
                                job.Noc = nocInt == 0 ? (int?)null : nocInt;
                            }

                            if (nocInt > 0)
                            {
                                job.NocGroup = GetNocGroup(nocInt);
                                job.NocJobTitle = nocNode.Attributes["label"].InnerText.Replace("\u200B", ""); // remove zero width space;;
                            }

                            #endregion

                            #region Job title

                            if (xmlJobNode.SelectSingleNode("title").Attributes["value"] != null &&
                                xmlJobNode.SelectSingleNode("title").Attributes["value"].InnerText.Trim() !=
                                string.Empty)
                            {
                                job.Title = xmlJobNode.SelectSingleNode("title").Attributes["value"].InnerText.Trim()
                                    .Replace("\u200B", ""); // remove zero width space
                            }

                            // substitute NocJobTitle for Title if Title is invalid
                            if (Regex.Matches(job.Title ?? "", @"[a-zA-Z]").Count == 0)
                            {
                                job.Title = string.IsNullOrEmpty(job.NocJobTitle) 
                                    ? "No job title provided" 
                                    : job.NocJobTitle;
                            }

                            // if the title is all caps then convert it to all lowercase
                            // CSS will do the rest of the title-casing using "text-transform: capitalize"
                            if (!job.Title.Any(char.IsLower))
                            {
                                job.Title = job.Title.ToLower();
                            }

                            // always capitalize PT (Part-Time)
                            job.Title = Regex.Replace(job.Title, @"(?i)\bpt\b", "PT");
                            // always capitalize FT (Full-Time)
                            job.Title = Regex.Replace(job.Title, @"(?i)\bft\b", "FT");

                            #endregion

                            #region External Source

                            job.ExternalSource = new ExternalJobSource
                            {
                                Source = new List<ExternalSource>()
                            };

                            var sourceCount = 0;
                            foreach (XmlNode source in xmlJobNode.SelectNodes("sources/source"))
                            {
                                var es = new ExternalSource
                                {
                                    Url = source.Attributes["url"].InnerText,
                                    Source = source.Attributes["name"].InnerText
                                };

                                sourceCount++;

                                if (sourceCount <= 5)
                                {
                                    // only add the top 5 sources (we really only use 1)
                                    job.ExternalSource.Source.Add(es);
                                }
                            }

                            #endregion
                        }
                    }
                }

                SetSalarySort(job);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ConvertToElasticJob(), reason: " + ex.Message);
                Console.WriteLine("XML: " + wantedXml);
                Console.WriteLine(ex);
            }

            return job;
        }
    }
}