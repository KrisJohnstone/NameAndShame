using CommandLine;

namespace NameAndShame.Models;

public class AppSettings
{

    [Option('u', "uri", Required = true, 
        HelpText = "Domain for your bitbucket provider. i.e. git.company.com")]
    public string Uri { get; set; }

    // Omitting long name, defaults to name of property, ie "--verbose"
    [Option('p', "projects", Required = true, HelpText = "Key Value Pair. <project:repo> to supply list of repos <project:repo1,repo2,repo3>")]
    public string Projects { get; set; }

    [Option('a',"auth", Required = true, HelpText = "Base64 Credential (Basic Auth)")]
    public string Auth { get; set; }
}
