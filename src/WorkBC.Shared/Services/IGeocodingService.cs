using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Shared.Services
{
    public interface IGeocodingService
    {
        Task<GeocodedLocationCache> GetLocation(string location);
    }
}