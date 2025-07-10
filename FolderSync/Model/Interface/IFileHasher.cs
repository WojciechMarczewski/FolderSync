namespace Model.Interface;

public interface IFileHasher
{
    string ComputeHash(string filePath);
}