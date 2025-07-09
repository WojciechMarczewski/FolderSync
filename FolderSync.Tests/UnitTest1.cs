using Model;

namespace FolderSync.Tests;

public class Tests
{
    private string _tempDir;
    private FolderSynchronizer _sync;
    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _sync = new FolderSynchronizer(new FileHasher());
    }

    [Test]
    public void File_Is_Copied_From_Source_To_Replica()
    {
        // Arrange 
        string source = Path.Combine(_tempDir, "source");
        string replica = Path.Combine(_tempDir, "replica");
        Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "file.txt"), "Hello");

        // Act
        _sync.Synchronize(source, replica);

        // Assert
        Assert.That(File.Exists(Path.Combine(replica, "file.txt")), Is.True);
    }
    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
    {
        try
        {
            Directory.Delete(_tempDir, recursive: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete temp directory: {_tempDir}\n{ex}");
        }
    }
    }
}
