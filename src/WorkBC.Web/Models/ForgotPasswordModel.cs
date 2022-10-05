namespace WorkBC.Web.Models
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
