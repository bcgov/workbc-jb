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
            var term = (startsWith ?? string.Empty).ToLower();
            return (await _jobBoardContext.NocCodes2021.AsNoTracking()
                    .Where(x =>
                        MEF.Functions.Like(x.Code.ToLower(), term + "%") ||
                        MEF.Functions.Like(x.Title.ToLower(), term + "%") ||
                        term.Length > 1 &&
                        MEF.Functions.Like(x.Title.ToLower(), "% " + term + "%"))
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