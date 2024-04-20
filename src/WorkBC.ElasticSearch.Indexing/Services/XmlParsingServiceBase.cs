using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing.ParsingHelpers;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Settings;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public abstract class XmlParsingServiceBase
    {
        protected readonly List<Data.Model.JobBoard.Location> DuplicateCities;
        protected readonly List<string> DuplicateCityNames;
        protected readonly List<NocCode> NocCodes;
        protected readonly List<NocCode2021> NocCodes2021;
        protected readonly Dictionary<string, string> UniqueCities;

        protected int WantedJobExpiryDays { get; set; }
        protected readonly JobBoardContext JobBoardContext;
        protected readonly IConfiguration Configuration;

        protected XmlParsingServiceBase(IConfiguration configuration)
        {
            this.Configuration = configuration;
            var connectionSettings = new ConnectionSettings();
            configuration.GetSection("ConnectionStrings").Bind(connectionSettings);

            try
            {
                WantedJobExpiryDays = configuration.GetSection("WantedSettings").Exists()
                    ? int.Parse(configuration["WantedSettings:JobExpiryDays"])
                    : General.DefaultWantedJobExpiryDays;
            } 
            catch
            {
                WantedJobExpiryDays = General.DefaultWantedJobExpiryDays;
            }

            JobBoardContext = new JobBoardContext(connectionSettings.DefaultConnection);
            var cityService = new CityIndexingService(JobBoardContext);
            UniqueCities = cityService.GetUniqueCitiesForIndexing().Result;
            DuplicateCities = cityService.GetDuplicateCitiesForIndexing().Result;
            NocCodes = JobBoardContext.NocCodes.ToList();
            NocCodes2021 = JobBoardContext.NocCodes2021.ToList();
            // store the cities for quick lookup during indexing
            DuplicateCityNames = DuplicateCities.Select(c => c.City.ToLower()).Distinct().ToList();
        }

        protected XmlParsingServiceBase(List<Data.Model.JobBoard.Location> duplicateCities,
            Dictionary<string, string> uniqueCities, List<NocCode> nocCodes, List<NocCode2021> nocCodes2021)
        {
            UniqueCities = uniqueCities;
            DuplicateCities = duplicateCities;
            NocCodes = nocCodes;
            NocCodes2021 = nocCodes2021;
            // store the cities for quick lookup during indexing
            DuplicateCityNames = DuplicateCities.Select(c => c.City.ToLower()).Distinct().ToList();
            WantedJobExpiryDays = General.DefaultWantedJobExpiryDays;
        }

        /// <summary>
        ///     Converts city names to duplicated city names when needed
        ///     (e.g. "Mill Bay" becomes "Mill Bay - Vancouver Island / Coast")
        /// </summary>
        protected string GetJobCity(string cityName, int cityId = 0)
        {
            if (!DuplicateCityNames.Contains(cityName.ToLower()))
            {
                return cityName;
            }

            Data.Model.JobBoard.Location location;

            // get the city by id
            if (cityId != 0)
            {
                location = DuplicateCities.FirstOrDefault(c => c.FederalCityId == cityId);

                if (location != null)
                {
                    return location.Label;
                }
            }
            else
            {
                // this is for wanted jobs....
                location = DuplicateCities.FirstOrDefault(c => c.City.ToLower() == cityName.ToLower());

                if (location != null)
                {
                    return location.Label;
                }
            }

            // get the first matching duplicate city with a null FederalCityId (matching by name)
            location = DuplicateCities.FirstOrDefault(c =>
                c.City.ToLower() == cityName.ToLower() && c.FederalCityId == null);

            if (location != null)
            {
                return location.Label;
            }

            // return the city name as a fallback
            return cityName;
        }

        protected static string CleanUpCityName(string cityName)
        {
            if (cityName.Contains("&apos;"))
            {
                // fixing an we found issue with "Hudson's Hope" which might impact other locations
                cityName = cityName.Replace("&apos;", "'");
            }

            // check if the city name has been physically remapped...
            if (XmlManualOverRides.AlternateCityNames.ContainsKey(cityName.ToLower()))
            {
                cityName = XmlManualOverRides.AlternateCityNames[cityName.ToLower()];
            }

            return cityName;
        }

        /// <summary>
        ///     Get the Region for the job.
        /// </summary>
        protected string GetJobRegion(string cityName, int cityId, Dictionary<string, string> cityRegions,
            List<Data.Model.JobBoard.Location> duplicateCities)
        {
            //Find  the region name based on the city name
            cityName = cityName.ToLower();

            if (cityRegions.ContainsKey(cityName))
            {
                return cityRegions[cityName];
            }

            Data.Model.JobBoard.Location location;

            if (cityId != 0)
            {
                // look up by city id
                location = duplicateCities.FirstOrDefault(c => c.FederalCityId == cityId);

                if (location != null)
                {
                    // remove the first part from the label leaving only the region
                    return location.Label.Replace(location.City + " - ", "");
                }
            }

            // look up again by name
            location = duplicateCities.FirstOrDefault(c => c.City.ToLower() == cityName);

            if (location != null)
            {
                // remove the first part from the label leaving only the region
                return location.Label.Replace(location.City + " - ", "");
            }

            return string.Empty;
        }

        protected string GetNocGroup(int? nocId, bool isFrench = false)
        {
            if (nocId == null || nocId == 0)
            {
                return string.Empty;
            }

            string codeStr = nocId.ToString().PadLeft(4, '0');

            try
            {
                string groupName = isFrench
                    ? (
                        from nc in NocCodes
                        where nc.Id == nocId
                        select nc.FrenchTitle
                    ).FirstOrDefault()
                    : (
                        from nc in NocCodes
                        where nc.Id == nocId
                        select nc.Title
                    ).FirstOrDefault();

                if (groupName != null)
                {
                    return $"{groupName} ({codeStr})";
                }

                return codeStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in GetNocGroup(), reason: " + ex.Message);
                Console.WriteLine("Noc: " + nocId);
                return string.Empty;
            }
        }

        protected string ConvertToTime(decimal time)
        {
            //Possible values: 8, 8.3, 20
            //Should display as 08:00am , 08:30am, 20:00pm

            //get the number without the decimal.
            decimal fullHour = Math.Abs(time);
            decimal minutes = time - Math.Truncate(time);

            //convert to Timespan to handle the 12hour time
            var ts = new TimeSpan(Convert.ToInt32(fullHour), Convert.ToInt32(minutes), 0);
            var dateTime = new DateTime(ts.Ticks);
            return dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }

        protected static void SetSalarySort(ElasticSearchJob job)
        {
            if (job.SalarySummary == "N/A")
            {
                // N/A gets sorted to the end
                job.SalarySort = new SalarySort
                {
                    Ascending = 99999999m,
                    Descending = -99999999m
                };
            }
            else
            {
                // jobs with zero salary but not N/A are sorted before N/A jobs
                job.SalarySort = new SalarySort
                {
                    Ascending = (job.Salary ?? 0m) < 0.01m ? 88888888m : job.Salary.Value,
                    Descending = (job.Salary ?? 0m) < 0.01m ? -88888888m : job.Salary.Value
                };
            }
        }
    }
}