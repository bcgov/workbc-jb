namespace WorkBC.Data.Enums
{
    public enum AccountStatus
    {
        InvalidStatusZero = 0,
        Active = 1, //active
        Deactivated = 3,
        Pending = 4, //waiting for email activation
        Deleted = 99 //deleted
    }
}