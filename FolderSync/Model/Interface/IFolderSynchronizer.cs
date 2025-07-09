namespace Model.Interface;

public interface IFolderSynchronizer
{
    void Synchronize(string sourcePath, string replicaPath);
}