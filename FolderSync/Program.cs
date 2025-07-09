using Microsoft.Extensions.DependencyInjection;
using Model;
using Model.Interface;
namespace FolderSync;

class Program
{
    static async Task Main(string[] args)
    {
        var services = CreateServices();

        App app = services.GetRequiredService<App>();
        int interval = 5;
        string sourcePath = "";
        string replicaPath = "";
        await app.Run(sourcePath, replicaPath, interval);
        //some app logic
        
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
}
