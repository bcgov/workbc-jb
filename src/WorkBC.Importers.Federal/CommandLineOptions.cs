using CommandLine;

namespace WorkBC.Importers.Federal
{
    public class CommandLineOptions
    {
        [Option("maxjobs", Required = false, HelpText = "Maximum number of jobs to import at once", Default = 20000)]
        public int MaxJobs { get; set; }

        [Option('m', "migrate", Required = false, HelpText = "Run SQL migrations (you must be a member of db_owners)")]
        public bool Migrate { get; set; }

        [Option('r', "reimport", Required = false, HelpText = "Re-import all jobs to the Jobs table")]
        public bool ReImport { get; set; }
    }
}