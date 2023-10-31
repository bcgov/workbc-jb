using System.Threading.Tasks;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Shared.Services
{
    public interface IGeocodingApiService : IGeocodingService
    {
        // deliberately empty
    }
    
    public interface IGeocodingService
    {
        Task<GeocodedLocationCache> GetLocation(string location);
    }
}