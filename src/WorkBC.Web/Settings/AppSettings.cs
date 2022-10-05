namespace WorkBC.Web.Settings
{
    public class AppSettings
    {
        public string[] CorsDomains { get; set; }
        public string GoogleMapsIPApi { get; set; }
        public string GoogleMapsReferrerApi { get; set; }
        public string UseRedisCache { get; set; }
        public bool UseJbAccountApp { get; set; }
        public bool UseSpa { get; set; }
        public bool IsProduction { get; set; }
    }
}