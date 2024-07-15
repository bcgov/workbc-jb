using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> CountriesAsync()
        {
            var result = await _locationService.GetCountriesAsync();
            return Ok(result);
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> ProvincesAsync()
        {
            var result = await _locationService.GetProvincesAsync();
            return Ok(result);
        }


        [HttpGet("regions/{city}")]
        public async Task<IActionResult> RegionsAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city) || city.Trim().Length < 3)
            {
                return Ok(Array.Empty<KeyValuePair<short,string>>());
            }

            var result = await _locationService.GetRegionsAsync(city);
            return Ok(result);
        }

        [HttpGet("cities/{city}/{includeRegion?}")]
        public async Task<IActionResult> CitiesAsync(string city, bool includeRegion = false)
        {
            var result = await _locationService.GetCitiesAsync(city.Replace("_", "/"), includeRegion);
            return Ok(result);
        }

        //Kentico API (Tiles widget)
        [HttpGet("all")]
        public async Task<IActionResult> All()
        {
            var result = await _locationService.GetAllLocationsAsync();

            return Ok(result);
        }

        //Kentico API (Tiles widget)
        [HttpGet("regions")]
        public async Task<IActionResult> AllRegions()
        {
            var result = await _locationService.GetAllRegionsAsync();

            return Ok(result);
        }
    }
}
