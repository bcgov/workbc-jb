using CommandLine;

namespace JsonConfigUpdater
{
    public class CommandLineOptions
    {
        [Option('r', "release", Required = false, HelpText = "TFS $(Release.ReleaseName) variable")]
        public string ReleaseName { get; set; }

        [Option('s', "secret", Required = false, HelpText = "Keycloak.ClientSecret secret")]
        public string KeycloakClientSecret { get; set; }
    }
}