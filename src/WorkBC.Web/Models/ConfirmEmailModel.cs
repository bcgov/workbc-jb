namespace WorkBC.Web.Models
{
    public class ConfirmEmailModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public bool? IsEncodedCode { get; set; } = false;
    }
}
