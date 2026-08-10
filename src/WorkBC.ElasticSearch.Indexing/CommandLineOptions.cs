using CommandLine;

namespace WorkBC.ElasticSearch.Indexing
{
    public class CommandLineOptions
    {
        [Option('r', "reindex", Required = false, HelpText = "Re-index (you must also run the other indexer afterward WITHOUT THIS OPTION)")]
        public bool ReIndex { get; set; }

        [Option('o', "reopen", Required = false, HelpText = "Close and re-open the index")]
        public bool ReOpen { get; set; }

        [Option('n', "noreindex", Required = false, HelpText = "Skip the reindex step")]
        public bool SkipReIndex { get; set; }

        [Option('m', "migrate", Required = false, HelpText = "Run SQL migrations (you must be a member of db_owners)")]
        public bool Migrate { get; set; }

        [Option('d', "debug", Required = false, HelpText = "Show debug info")]
        public bool Debug { get; set; }
    }
}