namespace WorkBC.Shared.Settings
{
    public class ProxySettings
    {
        public bool UseProxy { get; set; }

        public string ProxyHost { get; set; }

        public int ProxyPort { get; set; }

        public bool IgnoreSslErrors { get; set; }
    }
}