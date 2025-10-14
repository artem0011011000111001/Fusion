using System;

namespace Fusion.Core;

public abstract class LoggerBase : ILogger
{
    public virtual bool Enabled { get; set; } = true;

    public virtual LogLevel LogLevel { get; set; } = LogLevel.Medium;

    public abstract void Log(LogType type, string message);
    public virtual void Log(string message) => LogIfEnabled(LogType.Info, message);
    public virtual void LogWarning(string message) => LogIfEnabled(LogType.Warning, message);
    public virtual void LogError(string message) => LogIfEnabled(LogType.Error, message);
    public virtual void LogCritical(string message) => LogIfEnabled(LogType.Critical, message);
    public abstract int ClearLogs();

    private void LogIfEnabled(LogType type, string message)
    {
        if (Enabled)
            Log(type, message);
    }
}
