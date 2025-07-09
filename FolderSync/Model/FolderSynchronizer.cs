using Model.Interface;

namespace Model;

public class FolderSynchronizer : IFolderSynchronizer
{
    private readonly IFileHasher _fileHasher;
    public FolderSynchronizer(IFileHasher fileHasher)
    {
        _fileHasher = fileHasher;
    }
    public void Synchronize(string sourcePath, string replicaPath)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new DirectoryNotFoundException($"Source directory does not exist: {sourcePath}");
        }

        Directory.CreateDirectory(replicaPath);

        SyncFiles(sourcePath, replicaPath);
        DeleteRemovedFiles(sourcePath, replicaPath);
        DeleteRemovedDirectories(sourcePath, replicaPath);

    }
    private void SyncFiles(string sourcePath, string replicaPath)
    {
        var sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        foreach (var sourceFile in sourceFiles)
        {
            string relativePath = Path.GetRelativePath(sourcePath, sourceFile);
            string replicaFile = Path.Combine(replicaPath, relativePath);

            if (!File.Exists(replicaFile))
            {
                var dirPath = Path.GetDirectoryName(replicaFile);
                if (dirPath == null)
                    throw new InvalidOperationException($"Could not determine directory for path: {replicaFile}");

                Directory.CreateDirectory(dirPath);
                File.Copy(sourceFile, replicaFile);
                // Log: New file copied
            }


            else
            {
                string sourceHash = _fileHasher.ComputeHash(sourceFile);
                string replicaHash = _fileHasher.ComputeHash(replicaFile);

                if (sourceHash != replicaHash)
                {
                    File.Copy(sourceFile, replicaFile, overwrite: true);
                    // Log: File overwritten
                }
            }
        }
    }
    private void DeleteRemovedFiles(string sourcePath, string replicaPath)
    {
        var replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);

        foreach (var replicaFile in replicaFiles)
        {
            string relativePath = Path.GetRelativePath(replicaPath, replicaFile);
            string sourceFile = Path.Combine(sourcePath, relativePath);

            if (!File.Exists(sourceFile))
            {
                File.Delete(replicaFile);
                // Log: File deleted
            }
        }
    }
    private void DeleteRemovedDirectories(string sourcePath, string replicaPath)
    {
        var replicaDirs = Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories)
                                   .OrderByDescending(d => d.Length);

        foreach (var replicaDir in replicaDirs)
        {
            string relativePath = Path.GetRelativePath(replicaPath, replicaDir);
            string sourceDir = Path.Combine(sourcePath, relativePath);

            if (!Directory.Exists(sourceDir))
            {
                Directory.Delete(replicaDir, recursive: true);
                // Log: Directory deleted
            }
        }
    }




}