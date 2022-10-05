namespace WorkBC.Web.Models
{
    public interface IAccountBase
    {
        string Email { get; set; }
        string Username { get; set; }
    }

    public interface IUserBase
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public interface IUser : IUserBase, IAccountBase
    {
        string Id { get; set; }
    }

    public interface IUserInfo : IUser
    {
        string Token { get; set; }
    }
}