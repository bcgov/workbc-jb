using CommandLine;

namespace WorkBC.Importers.Wanted
{
    public class CommandLineOptions
    {
        [Option('b', "bulk", Required = false,
            HelpText = "Import jobs for the past 30** days (**WantedSetting.JobExpiryDays can be changed in TFS)")]
        public bool Bulk { get; set; }

        [Option('m', "migrate", Required = false, HelpText = "Run SQL migrations (you must be a member of db_owners)")]
        public bool Migrate { get; set; }

        [Option('r', "reimport", Required = false, HelpText = "Re-import all jobs to the Jobs table")]
        public bool ReImport { get; set; }
    }
}