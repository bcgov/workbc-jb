using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class CityIndexingService
    {
        private readonly JobBoardContext _jobBoardContext;

        public CityIndexingService(JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
        }

        private enum LocationType
        {
            Place = 3,
            District = 2,
            Region = 1
        }

        public async Task<Dictionary<string, string>> GetUniqueCitiesForIndexing()
        {
            // get the regions as a dictionary
            Dictionary<int, string> regions = await _jobBoardContext.Regions.AsNoTracking()
                .Where(r => r.Id == 0 || !r.IsHidden)
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            // build a list of cities and regions 
            return await _jobBoardContext.Locations.Where(l => !l.IsDuplicate && !l.IsHidden && l.RegionId != null)
                // ReSharper disable once PossibleInvalidOperationException
                .ToDictionaryAsync(l => l.City.ToLower(), l => regions[(short) l.RegionId] ?? "");
        }

        public async Task<List<Data.Model.JobBoard.Location>> GetDuplicateCitiesForIndexing()
        {
            return await _jobBoardContext.Locations.Where(l => l.IsDuplicate && !l.IsHidden).ToListAsync();
        }
    }
}
