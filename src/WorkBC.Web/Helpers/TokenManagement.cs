namespace WorkBC.Web.Helpers
{
    public class TokenManagement
    {
        public string Secret { get; set; }
        public int AccessExpiration { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
