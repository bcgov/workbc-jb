using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Web.Models
{
    public interface IRegisterModel: ILoginModel, IUserBase
    {
        int CountryId { get; set; }
        int ProvinceId { get; set; }
        string City { get; set; }
        int LocationId { get; set; }
        int SecurityQuestionId { get; set; }
        string SecurityAnswer { get; set; }
        JobSeekerFlags JobSeekerFlags { get; set; }
    }

    public class RegisterModel : IRegisterModel
    {
        public string Email { get; set; }
        public string Username { get; set; }        
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public string City { get; set; }
        public int LocationId { get; set; }
        public int SecurityQuestionId { get; set; }
        public string SecurityAnswer { get; set; }
        public JobSeekerFlags JobSeekerFlags { get; set; }
    }
}