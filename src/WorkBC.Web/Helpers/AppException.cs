using System;

namespace WorkBC.Web.Helpers
{
    // Custom exception class for throwing application specific exceptions (e.g. for validation) 
    // that can be caught and handled within the application
    public class AppException : Exception
    {
        public enum Fields
        {
            NoSpecificField = -1,
            Email = 1,
            Password = 2,
        }

        public AppException(string message, Fields field = Fields.NoSpecificField) : base(message)
        {
            Field = field;
        }

        public Fields Field { get; set; }
    }
}