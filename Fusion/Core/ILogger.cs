using System;

namespace Fusion.Core;

public enum LogLevel
{
    Calm,
    Medium,
    Hard,
    Everything
}

public enum LogType
{
    Info,
    Warning,
    Error,
    Critical
}

public interface ILogger
{
    /// <summary>
    /// Indicates if logging is enabled
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Indicates level of logging frequency. It does not prevent any log calls, it is used to check required log level and prevent unimportant logs
    /// Example:
    /// if (Logger.LogLevel >= LogLevel.Hard)
    ///    Logger.Log("Unimportant log")
    /// </summary>
    LogLevel LogLevel { get; set; }

    /// <summary>
    /// Logs message with specified type
    /// </summary>
    void Log(LogType type, string message);
    /// <summary>
    /// Logs info message
    /// </summary>
    void Log(string message);
    /// <summary>
    /// Logs warning message
    /// </summary>
    void LogWarning(string message);
    /// <summary>
    /// Logs error message
    /// </summary>
    void LogError(string message);
    /// <summary>
    /// Logs critical message
    /// </summary>
    void LogCritical(string message);

    /// <summary>
    /// Clears all logs
    /// </summary>
    /// <returns>Count of cleared logs</returns>
    int ClearLogs();
}

