using System;

namespace Fusion.Core;

public abstract class ObservableLogger : LoggerBase
{
    /// <summary>
    /// Invokes when some message has been logged
    /// First param is a log type
    /// Second param is a message
    /// </summary>
    public event Action<LogType, string>? Logged;

    /// <summary>
    /// Invokes when ClearLogs has been called
    /// First param is a count of cleared logs
    /// </summary>
    public event Action<int>? Cleared;

    protected void OnLogged(LogType type, string message) => Logged?.Invoke(type, message);
    protected void OnCleared(int count) => Cleared?.Invoke(count);

    protected abstract void LogImplementation(LogType type, string message);
    protected virtual void LogImplementation(string message) => LogImplementation(LogType.Info, message);
    protected virtual void LogWarningImplementation(string message) => LogImplementation(LogType.Warning, message);
    protected virtual void LogErrorImplementation(string message) => LogImplementation(LogType.Error, message);
    protected virtual void LogCriticalImplementation(string message) => LogImplementation(LogType.Critical, message);
    protected abstract int ClearLogsImplementation();

    public override void Log(LogType type, string message)
    {
        if (!Enabled) return;

        LogImplementation(type, message);
        OnLogged(type, message);
    }

    public override void Log(string message)
    {
        if (!Enabled) return;

        LogImplementation(message);
        OnLogged(LogType.Info, message);
    }

    public override void LogWarning(string message)
    {
        if (!Enabled) return;

        LogWarningImplementation(message);
        OnLogged(LogType.Warning, message);
    }

    public override void LogError(string message)
    {
        if (!Enabled) return;

        LogErrorImplementation(message);
        OnLogged(LogType.Error, message);
    }

    public override void LogCritical(string message)
    {
        if (!Enabled) return;

        LogCriticalImplementation(message);
        OnLogged(LogType.Error, message);
    }

    public override int ClearLogs()
    {
        int count = ClearLogsImplementation();
        OnCleared(count);

        return count;
    }
}
