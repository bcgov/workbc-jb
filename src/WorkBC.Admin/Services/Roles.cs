namespace WorkBC.Admin.Services
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Reporting = "Reporting";
    }

    public static class MinAccessLevel
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin,SuperAdmin";
        public const string Reporting = "Reporting,Admin,SuperAdmin";
    }
}