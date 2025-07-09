using System.Security.Cryptography;
using Model.Interface;

namespace Model;

public class FileHasher : IFileHasher
{
    public string ComputeHash(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return Convert.ToBase64String(hash);
    }
}