using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Data.Migrations
{
    public partial class Geocoding__Add_Permanent_Records : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            GeocodedLocationCache[] permanentLocations =
            {
                new GeocodedLocationCache
                    {Name = "Aberdeen - Thompson-Okanagan, BC, CANADA", Latitude = "50.638266", Longitude = "-120.359045"},
                new GeocodedLocationCache
                    {Name = "Aberdeen - Mainland / Southwest, BC, CANADA", Latitude = "49.060478", Longitude = "-122.425861"},
                new GeocodedLocationCache {Name = "Black Creek - Cariboo, BC, CANADA", Latitude = "52.3", Longitude = "-121.1"},
                new GeocodedLocationCache
                {
                    Name = "Black Creek - Vancouver Island / Coast, BC, CANADA", Latitude = "49.832962", Longitude = "-125.129371"
                },
                new GeocodedLocationCache
                    {Name = "Cherry Creek - Thompson-Okanagan, BC, CANADA", Latitude = "50.7126473", Longitude = "-120.6222831"},
                new GeocodedLocationCache
                {
                    Name = "Cherry Creek - Vancouver Island / Coast, BC, CANADA", Latitude = "49.283866", Longitude = "-124.78917"
                },
                new GeocodedLocationCache
                    {Name = "Dog Creek - Cariboo, BC, CANADA", Latitude = "52.101158", Longitude = "-122.127236"},
                new GeocodedLocationCache
                    {Name = "Dog Creek - North Coast & Nechako, BC, CANADA", Latitude = "54.284372", Longitude = "-124.264725"},
                new GeocodedLocationCache
                    {Name = "East Gate - Thompson-Okanagan, BC, CANADA", Latitude = "49.1376387", Longitude = "-120.6145763"},
                new GeocodedLocationCache
                    {Name = "Fairfield - Vancouver Island / Coast, BC, CANADA", Latitude = "48.416956", Longitude = "-123.352104"},
                new GeocodedLocationCache
                    {Name = "Fairfield - Mainland / Southwest, BC, CANADA", Latitude = "49.195283", Longitude = "-121.937967"},
                new GeocodedLocationCache
                    {Name = "Kelly Lake - Thompson-Okanagan, BC, CANADA", Latitude = "51.016667", Longitude = "-121.766667"},
                new GeocodedLocationCache
                    {Name = "Kelly Lake - Northeast, BC, CANADA", Latitude = "55.261085", Longitude = "-120.032901"},
                new GeocodedLocationCache
                    {Name = "Mill Bay - Vancouver Island / Coast, BC, CANADA", Latitude = "48.650339", Longitude = "-123.557072"},
                new GeocodedLocationCache
                    {Name = "Mill Bay - North Coast & Nechako, BC, CANADA", Latitude = "54.8260094", Longitude = "-126.1644895"},
                new GeocodedLocationCache
                {
                    Name = "Moresby Island - Vancouver Island / Coast, BC, CANADA", Latitude = "48.7153869",
                    Longitude = "-123.3122303"
                },
                new GeocodedLocationCache
                {
                    Name = "Moresby Island - North Coast & Nechako, BC, CANADA", Latitude = "53.051747", Longitude = "-132.0297092"
                },
                new GeocodedLocationCache
                    {Name = "Mud Bay - Mainland / Southwest, BC, CANADA", Latitude = "49.095211", Longitude = "-122.858102"},
                new GeocodedLocationCache
                    {Name = "Mud Bay - Vancouver Island / Coast, BC, CANADA", Latitude = "49.463063", Longitude = "-124.796544"},
                new GeocodedLocationCache
                    {Name = "Nechako - Cariboo, BC, CANADA", Latitude = "53.931315", Longitude = "-122.77613"},
                new GeocodedLocationCache
                    {Name = "Nechako - North Coast & Nechako, BC, CANADA", Latitude = "54.057177", Longitude = "-128.632567"},
                new GeocodedLocationCache
                    {Name = "Old Fort - North Coast & Nechako, BC, CANADA", Latitude = "55.0386111", Longitude = "-126.3144444"},
                new GeocodedLocationCache
                    {Name = "Old Fort - Northeast, BC, CANADA", Latitude = "56.202882", Longitude = "-120.825938"},
                new GeocodedLocationCache
                    {Name = "Pine Valley - Cariboo, BC, CANADA", Latitude = "52.176915", Longitude = "-122.083284"},
                new GeocodedLocationCache
                    {Name = "Pine Valley - Northeast, BC, CANADA", Latitude = "55.633333", Longitude = "-122.116667"},
                new GeocodedLocationCache
                    {Name = "Pineview - Cariboo, BC, CANADA", Latitude = "53.836932", Longitude = "-122.65638"},
                new GeocodedLocationCache
                    {Name = "Pineview - Northeast, BC, CANADA", Latitude = "56.330823", Longitude = "-120.768405"},
                new GeocodedLocationCache
                    {Name = "Shuswap - Thompson-Okanagan, BC, CANADA", Latitude = "50.785726", Longitude = "-119.710952"},
                new GeocodedLocationCache
                    {Name = "Shuswap - Kootenay, BC, CANADA", Latitude = "50.541149", Longitude = "-116.009155"},
                new GeocodedLocationCache
                    {Name = "Silver Creek - Mainland / Southwest, BC, CANADA", Latitude = "49.36106", Longitude = "-121.465968"},
                new GeocodedLocationCache
                    {Name = "Silver Creek - Thompson-Okanagan, BC, CANADA", Latitude = "50.605174", Longitude = "-119.365829"},
                new GeocodedLocationCache
                    {Name = "Skookumchuck - Kootenay, BC, CANADA", Latitude = "49.912481", Longitude = "-115.733706"},
                new GeocodedLocationCache
                    {Name = "Skookumchuck - Mainland / Southwest, BC, CANADA", Latitude = "49.933333", Longitude = "-122.4"},
                new GeocodedLocationCache
                    {Name = "Stillwater - Thompson-Okanagan, BC, CANADA", Latitude = "52.05", Longitude = "-119.95"},
                new GeocodedLocationCache
                    {Name = "Stillwater - Vancouver Island / Coast, BC, CANADA", Latitude = "49.765796", Longitude = "-124.307586"},
                new GeocodedLocationCache
                    {Name = "Summit Lake - Cariboo, BC, CANADA", Latitude = "54.281901", Longitude = "-122.640001"},
                new GeocodedLocationCache
                    {Name = "Summit Lake - Northeast, BC, CANADA", Latitude = "58.65", Longitude = "-124.633333"},
                new GeocodedLocationCache
                    {Name = "Summit Lake - Kootenay, BC, CANADA", Latitude = "50.157631", Longitude = "-117.66169"},
                new GeocodedLocationCache
                    {Name = "Willowbrook - Northeast, BC, CANADA", Latitude = "55.818183", Longitude = "-120.558022"},
                new GeocodedLocationCache
                    {Name = "Willowbrook - Thompson-Okanagan, BC, CANADA", Latitude = "49.266492", Longitude = "-119.592269"}
            };

            foreach (GeocodedLocationCache l in permanentLocations)
            {
                migrationBuilder.InsertData(
                    "GeocodedLocationCache",
                    new[] {"Name", "Latitude", "Longitude", "DateGeocoded", "IsPermanent"},
                    new object[]
                    {
                        l.Name, l.Latitude, l.Longitude, new DateTime(2019, 01, 01), true
                    });
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}