using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Web.Models;
using WorkBC.Web.Models.Search;
using MEF = Microsoft.EntityFrameworkCore.EF;

namespace WorkBC.Web.Services
{
    public interface INocSearchService
    {
        Task<IEnumerable<NocCodeModel>> SearchNocCodes(string startsWith);
    }

    public class NocSearchService : INocSearchService
    {
        private readonly JobBoardContext _jobBoardContext;

        public NocSearchService(JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
        }

        public async Task<IEnumerable<NocCodeModel>> SearchNocCodes(string startsWith)
        {
            return (await _jobBoardContext.NocCodes.AsNoTracking()
                    .Where(x =>
                        MEF.Functions.Like(x.Code, startsWith + "%") ||
                        MEF.Functions.Like(x.Title, startsWith + "%") ||
                        startsWith.Length > 1 &&
                        MEF.Functions.Like(x.Title, "% " + startsWith + "%"))
                    .ToListAsync())
                .OrderByDescending(x =>
                    x.Title.StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Code.StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase))
                .ThenBy(x => $"{x.Code} {x.Title}")
                .Take(50)
                .Select(x => new NocCodeModel
                {
                    NocCode = x.Code,
                    Name = $"NOC {x.Code} {x.Title}"
                });
        }
    }
}