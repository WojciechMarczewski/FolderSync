using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Invocation;
using Model;
using Model.Interface;
namespace FolderSync;

class Program
{
    private const int defaultInterval = 5;
    static async Task Main(string[] args)
    {
        var rootCommand = AddCmdLineOptions();

        rootCommand.SetAction(async parseResult =>
        {
            var source = parseResult.GetValue<DirectoryInfo>("--source")!;
            var replica = parseResult.GetValue<DirectoryInfo>("--replica")!;
            var interval = parseResult.GetValue<int>("--interval");
            var logFile = parseResult.GetValue<FileInfo>("--log")!;

            var services = CreateServices(logFile.FullName);
            var app = services.GetRequiredService<App>();

            await app.Run(source.FullName, replica.FullName, interval);
        });

        await rootCommand.Parse(args).InvokeAsync();

    }

    private static ServiceProvider CreateServices(string logPath)
    {
        var serviceProvider = new ServiceCollection()
                .AddSingleton<IFileHasher, FileHasher>()
                .AddSingleton<ILogger>(sp => new ConsoleFileLogger(logPath))
                .AddSingleton<IFolderSynchronizer, FolderSynchronizer>()
                .AddSingleton<App>()
                .BuildServiceProvider();

        return serviceProvider;
    }
    private static RootCommand AddCmdLineOptions()
    {
        var sourceOption = new Option<DirectoryInfo>("--source")
        {
            Description = "Source directory to synchronize from",
            Required = true
        };

        var replicaOption = new Option<DirectoryInfo>("--replica")
        {
            Description = "Replica directory to synchronize to",
            Required = true

        };

        var intervalOption = new Option<int>("--interval")
        {
            Description = "Synchronization interval in seconds. Default value is 5 seconds.",
            DefaultValueFactory = _ => defaultInterval
        };


        var logOption = new Option<FileInfo>("--log")
        {
            Description = "Path to the log file",
            Required = true
        };

        var rootCommand = new RootCommand("FolderSync command-line app")
            {
                sourceOption,
                replicaOption,
                intervalOption,
                logOption
            };
        rootCommand.Description = "Synchronizes source directory with replica directory to maintain identical copy of source folder";
        return rootCommand;

    } 
}