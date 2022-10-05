namespace WorkBC.Admin.Areas.Reports.Data.QueryServices
{
    /// <summary>
    ///     Base class for all query services
    /// </summary>
    public abstract class QueryServiceBase
    {
        protected readonly string ConnectionString;

        protected QueryServiceBase(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}