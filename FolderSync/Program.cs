using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Invocation;
using Model;
using Model.Interface;
namespace FolderSync;

class Program
{
        static async Task Main(string[] args)
    {


        var services = CreateServices();

        App app = services.GetRequiredService<App>();

        var rootCommand = AddCmdLineOptions();

        rootCommand.SetAction(async parseResult => await app.Run(
            parseResult.GetValue<DirectoryInfo>("--source")!.FullName,
            parseResult.GetValue<DirectoryInfo>("--replica")!.FullName,
            parseResult.GetValue<int>("--interval")));

        await rootCommand.Parse(args).InvokeAsync();

    }
   
    private static ServiceProvider CreateServices()
    {
        var serviceProvider = new ServiceCollection()
                .AddSingleton<IFileHasher, FileHasher>()
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
        rootCommand.Description = "Syncs source directory with replica directory to maintain identical copy of source folder";
        return rootCommand;
        
    }
}
