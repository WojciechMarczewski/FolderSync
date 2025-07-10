using Model;

namespace FolderSync.Tests;

public class FolderSyncTests
{
    private string _tempDir;
    private FolderSynchronizer _sync;
    private string _source;
    private string _replica;
    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _sync = new FolderSynchronizer(new FileHasher(), new ConsoleFileLogger(Path.Combine(Path.GetTempPath(), "syncLogger.txt")));
        _source = Path.Combine(_tempDir, "source");
        _replica = Path.Combine(_tempDir, "replica");
        Directory.CreateDirectory(_source);
    }

    [Test]
    public void File_Is_Copied_From_Source_To_Replica()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_source, "file.txt"), "Hello");

        // Act
        _sync.Synchronize(_source, _replica);

        // Assert
        Assert.That(File.Exists(Path.Combine(_replica, "file.txt")), Is.True);
    }
    [Test]
    public void Replica_File_Is_Overwritten_On_Change_In_Source()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_source, "file.txt"), "Hello");

        // Act
        _sync.Synchronize(_source, _replica);
        File.WriteAllText(Path.Combine(_source, "file.txt"), "Hello world!");
        _sync.Synchronize(_source, _replica);

        // Assert
        Assert.That(File.ReadAllText(Path.Combine(_replica, "file.txt")), Is.EqualTo("Hello world!"));



    }
    [Test]
    public void Replica_File_Is_Deleted_On_Deletion_In_Source()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_source, "file.txt"), "Hello");

        // Act
        _sync.Synchronize(_source, _replica);
        File.Delete(Path.Combine(_source, "file.txt"));
        _sync.Synchronize(_source, _replica);

        // Assert
        Assert.That(File.Exists(Path.Combine(_replica, "file.txt")), Is.False);
    }
    [Test]
    public void New_Subdirectory_And_File_Are_Copied()
    {
        // Arrange
        _sync.Synchronize(_source, _replica);
        var sourceSubDir = Path.Combine(_source, "subdir");
        Directory.CreateDirectory(sourceSubDir);
        File.WriteAllText(Path.Combine(sourceSubDir, "inner.txt"), "nested content");

        // Act
        _sync.Synchronize(_source, _replica);


        //Assert
        string replicatedFile = Path.Combine(_replica, "subdir", "inner.txt");
        Assert.That(File.Exists(replicatedFile), Is.True);
        Assert.That(File.ReadAllText(replicatedFile), Is.EqualTo("nested content"));
    }
    [Test]
    public void Empty_Directory_In_Source_Exists_In_Replica()
    {
        // Arrange
        var sourceSubDir = Path.Combine(_source, "subdir");
        Directory.CreateDirectory(sourceSubDir);


        // Act
        _sync.Synchronize(_source, _replica);

        //Assert
        var replicaSubDir = Path.Combine(_replica, "subdir");
        Assert.That(Directory.Exists(replicaSubDir), Is.True);
    }
    [Test]
    public void Existing_Directory_Removed_From_Replica_If_Deleted_In_Source()
    {
        // Arrange
        var sourceSubDir = Path.Combine(_source, "subdir");
        Directory.CreateDirectory(sourceSubDir);


        // Act
        _sync.Synchronize(_source, _replica);
        Directory.Delete(sourceSubDir);
        _sync.Synchronize(_source, _replica);


        //Assert
        var replicaSubDir = Path.Combine(_replica, "subdir");
        Assert.That(Directory.Exists(replicaSubDir), Is.False);
    }
    [Test]
    public void LogFile_Is_Created_And_Contains_Log_Entry()
    {
        // Arrange
        var logPath = Path.Combine(Path.GetTempPath(), "syncLogger.txt");
      

        // Act
        _sync.Synchronize(_source, _replica);
        File.WriteAllText(Path.Combine(_source, "file.txt"), "Hello");
        _sync.Synchronize(_source, _replica);

        // Assert
        Assert.That(File.Exists(logPath), Is.True, "Log file was not created");

        var logContent = File.ReadAllText(logPath);
        Assert.That(logContent, Does.Contain("Created new file"), "Expected log entry not found");
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
        var logPath = Path.Combine(Path.GetTempPath(), "syncLogger.txt");
        if (File.Exists(logPath))
        {
            try
            {
                File.Delete(logPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete log file: {logPath}\n{ex}");
            }
        }
    }
}
