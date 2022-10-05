namespace WorkBC.Web.Models
{
    public interface ILoginModel: IAccountBase
    {
        string Password { get; set; }
    }

    public class LoginModel : ILoginModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
