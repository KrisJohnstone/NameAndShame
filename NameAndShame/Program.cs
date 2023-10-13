using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NameAndShame.Services;
using CommandLine;
using NameAndShame.Models;
using NameAndShame.Repositories;

namespace NameAndShame;

class Program
{
    static async Task<int> Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddScoped<IBitBucketRepo, BitBucketRepo>()
                    .AddSingleton<Filters>()
                    .AddSingleton<Handler>()
                    .AddOptions<AppSettings>()
                    .Configure(opt =>
                        Parser.Default.ParseArguments(() => opt, Environment.GetCommandLineArgs()));

            })
            .Build();
        
        using (var writer = new StringWriter())
        {
            var parser = new Parser(configuration =>
            {
                configuration.AutoHelp = true;
                configuration.AutoVersion = false;
                configuration.CaseSensitive = false;
                configuration.IgnoreUnknownArguments = true;
                configuration.HelpWriter = writer;
            });

            if (args.Length == 0)
            {
                Console.WriteLine("No Arguements Passed");
                return 1;
            }
            var result = parser.ParseArguments<AppSettings>(args);
            result.WithNotParsed(errors => HandleErrors(errors, writer));
        }
        
        return await host.Services.GetService<Handler>().Run();
    }
    
    private static void HandleErrors(IEnumerable<Error> errors, TextWriter writer)
    {
        if (!errors.Any(e => e.Tag != ErrorType.HelpRequestedError && e.Tag != ErrorType.VersionRequestedError))
            return;
        
        Console.WriteLine(writer);
    }
}
