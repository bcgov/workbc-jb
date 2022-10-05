namespace WorkBC.Shared.Settings
{
    public class ConnectionSettings
    {
        //SQL Server connection string
        public string DefaultConnection { get; set; }
        public string EnterpriseConnection { get; set; }
        public string ElasticSearchServer { get; set; }
    }
}
