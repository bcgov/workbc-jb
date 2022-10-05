using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WorkBC.ElasticSearch.Indexing.XmlParsingHelpers
{
    /// <summary>
    ///     Helper class for parsing multiple locations from Federal XML.
    ///     It specifically deals with some bad data in the Federal XML feed to ensure that
    ///     only locations in BC are imported, and the same city is never imported more
    ///     than once for a job. And it also sorts the locations into alphabetical order by
    ///     city name
    /// </summary>
    public class FederalXmlLocations
    {
        private readonly List<City> _cities;

        public FederalXmlLocations(XmlNode xmlJobNode)
        {
            _cities = new List<City>();

            // get data for multiple locations
            XmlNodeList cityIdNodes = xmlJobNode.SelectNodes("city_id/string");
            XmlNodeList cityNameNodes = xmlJobNode.SelectNodes("city_name/string");
            XmlNodeList provinceNodes = xmlJobNode.SelectNodes("province_cd/string");
            XmlNodeList geoNodes = xmlJobNode.SelectNodes("latlng/string");

            if (cityIdNodes == null || cityNameNodes == null || provinceNodes == null || geoNodes == null)
            {
                return;
            }

            int locationCount = cityIdNodes.Count;

            if (cityNameNodes.Count < locationCount)
            {
                locationCount = cityNameNodes.Count;
            }

            if (geoNodes.Count < locationCount)
            {
                locationCount = geoNodes.Count;
            }

            for (var i = 0; i < locationCount; i++)
            {
                string province = provinceNodes.Count <= i
                    ? provinceNodes[0].InnerText
                    : provinceNodes[i].InnerText;

                string cityName = cityNameNodes[i].InnerText;
                string cityId = cityIdNodes[i].InnerText;
                string geoLocation = geoNodes[i].InnerText;

                if (province == "BC" && _cities.All(c => c.CityName != cityName))
                {
                    int.TryParse(cityId, out int id);

                    string[] geoCoordinates = geoLocation.Split(",");

                    _cities.Add(new City
                    {
                        CityId = id,
                        CityName = cityName,
                        Location = Location.LocationOrNull(
                            geoCoordinates[0],
                            geoCoordinates[1]
                        )
                    });
                }
            }
        }

        public string[] CityNames
        {
            get
            {
                if (_cities.Any())
                {
                    return _cities.OrderBy(c => c.CityName).Select(c => c.CityName).ToArray();
                }

                return new[] { string.Empty };
            }
        }

        public int[] CityIds => _cities.OrderBy(c => c.CityName).Select(c => c.CityId).ToArray();

        public Location[] Locations => _cities
            .OrderBy(c => c.CityName)
            .Where(c => c.Location != null)
            .Select(c => c.Location)
            .ToArray();

        public string[] LocationGeos
        {
            get
            {
                if (Locations.Any())
                {
                    return Locations.Select(
                        location => location == null ? string.Empty : $"{location.Lat},{location.Lon}"
                    ).ToArray();
                }

                return new string[] { };
            }
        }

        public string Province => _cities.Any() ? "BC" : string.Empty;

        public int Count => _cities.Count();

        private class City
        {
            public int CityId { get; init; }
            public string CityName { get; init; }
            public Location Location { get; init; }
        }
    }
}