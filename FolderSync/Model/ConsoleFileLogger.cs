using Model.Interface;

namespace Model;

public class ConsoleFileLogger : ILogger
{
    private readonly string _logPath;
    public ConsoleFileLogger(string logPath)
    {
        _logPath = logPath;    
    }
    public void Log(string message)
    {
        var logMessage = $"({DateTimeOffset.Now:u}) {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(_logPath, logMessage + Environment.NewLine);
    }
}
