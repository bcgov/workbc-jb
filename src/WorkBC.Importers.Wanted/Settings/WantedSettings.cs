namespace WorkBC.Importers.Wanted.Settings
{
    public class WantedSettings
    {
        //Base URL with some default parameters that will never change
        public string ApiUrl { get; set; }

        //The pass key
        public string PassKey { get; set; }

        //Current page number
        public decimal PageIndex { get; set; } 

        //Number of results per page
        public decimal PageSize { get; set; }

        //Date from - yyyy-mm-dd
        public string DateFrom { get; set; }

        //Date to - yyyy-mm-dd
        public string DateEnd { get; set; }

        //How many days to import every time the importer run in regular mode
        public int DaysPerImport { get; set; }

        //How many days we need to import for the bulk importer
        public int JobExpiryDays { get; set; }

        //How many days we need to keep a job after it disappears from the
        //Wanted API, when we are running the importer in bulk mode
        public int DaysToKeepJobSinceLastSeen { get; set; }

        // Maximum number of jobs to expire at once
        public int MaximumJobsToExpireAtOnce { get; set; }
    }
}
