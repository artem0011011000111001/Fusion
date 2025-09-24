using System;

namespace Fusion.Core;

public class FileLogger : ObservableLogger, IDisposable
{
    private readonly string _filePath;
    private StreamWriter _stream;
    private readonly Lock _lock = new();

    public FileLogger(string filePath)
    {
        _filePath = filePath;
        _stream = new StreamWriter(filePath, append: true) { AutoFlush = true };
    }

    protected override void LogImplementation(LogType type, string message)
    {
        string formatted = $"[{type}][{DateTime.Now:HH:mm:ss}] {message}";

        lock (_lock)
        {
            _stream.WriteLine(formatted);
        }
    }

    protected override int ClearLogsImplementation()
    {
        lock (_lock)
        {
            _stream.Dispose();
        }

        int lineCount = 0;
        if (File.Exists(_filePath))
        {
            lineCount = File.ReadAllLines(_filePath).Length;
        }

        File.WriteAllText(_filePath, string.Empty);

        _stream = new StreamWriter(_filePath, append: true) { AutoFlush = true };

        return lineCount;
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _stream.Dispose();
        }
    }
}

