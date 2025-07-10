using Model.Interface;

namespace FolderSync;

public class App
{
    private const int defaultInterval = 5;
    private readonly IFolderSynchronizer _folderSynchronizer;
    public App(IFolderSynchronizer folderSynchronizer)
    {

        _folderSynchronizer = folderSynchronizer;
         
    }



    public async Task Run(string sourcePath, string replicaPath, int interval)
    {
        try
        {
            _folderSynchronizer.Synchronize(sourcePath, replicaPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
        if (interval <= 0) interval = defaultInterval;
        Console.WriteLine("Press ESC to stop synchronization...");
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(interval));
        
        var cancellationSource = new CancellationTokenSource();


        var keyListener = Task.Run(() =>
        {
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    cancellationSource.Cancel();
                    break;
                }
            }
        });

        while (await timer.WaitForNextTickAsync(cancellationSource.Token))
        {
            try
            {
                
                _folderSynchronizer.Synchronize(sourcePath, replicaPath);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
        await keyListener;
    }
        

}