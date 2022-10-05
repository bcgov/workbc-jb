using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WorkBC.Data;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class MigrationService
    {
        private readonly JobBoardContext _dbContext;
        private readonly ILogger _logger;

        public MigrationService(JobBoardContext context, ILogger logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public bool RunDbMigrations()
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _logger.Information("Applying migrations...");

                try
                {
                    _dbContext.Database.Migrate();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                    _logger.Warning("When migrations fail, it is usually due to a foreign key violation or other data-related issue. Read the stack trace above to determine which tables and columns are causing the error. If possible, try to correct the error through SQL Management studio by deleting or modifying the offending records.");
                    _logger.Warning("Each migration is run in a transaction, and the transaction for the failed migration has been rolled back.");

                    return false;
                }
            }

            _logger.Information("No pending migrations");
            return true;
        }
    }
}