using System;
using Fusion.Core;
using Fusion.Configs;
using Fusion.Loggers;

namespace Fusion;

public partial class Application
{
    public string ConfigPath { get; } = Path.Combine(globalPath, Constants.ConfigFolderName);
    public string CachePath { get; } = Path.Combine(localPath, Constants.CacheFolderName);
    public string LogsPath { get; } = Path.Combine(localPath, Constants.LogsFolderName);
    public string ApiPath { get; } = Path.Combine(sharedPath, Api.Constants.ApiFolderName);

    #region Config

    public IReadOnlyStorage InfoConfig
    {
        get
        {
            string path = Path.Combine(ConfigPath, Constants.InfoConfig.Filename);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"InfoConfig file is not found {path}");
            }

            return new IniConfig(path);
        }
    }

    public bool ConfigExists(string name)
    {
        return File.Exists(Path.Combine(ConfigPath, name));
    }

    public TConfig MakeOrLoadConfig<TConfig>(string name) where TConfig : ConfigBase, new()
    {
        string path = Path.Combine(ConfigPath, name);
        TConfig config = new();

        config.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, config.Data);
        };

        if (File.Exists(path) && config.InitFromFile(path))
        {
            return config;
        }

        File.Create(path).Dispose();

        return config;
    }

    #endregion

    #region Caching



    #endregion

    #region Logging

    private ObservableLogger? _logger;
    public ObservableLogger Logger
    {
        get => _logger ??= new FileLogger(Path.Combine(LogsPath, $"Session_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt"));
        set => _logger = value;
    }

    /// <summary>
    /// Logs 'message' with type 'type'
    /// </summary>
    public void Log(LogType type, string message) => Logger.Log(type, message);
    /// <summary>
    /// Logs 'message'
    /// </summary>
    public void Log(string message) => Logger.Log(message);
    /// <summary>
    /// Logs warning 'message'
    /// </summary>
    public void LogWarning(string message) => Logger.LogWarning(message);
    /// <summary>
    /// Logs error 'message'
    /// </summary>
    public void LogError(string message) => Logger.LogError(message);
    /// <summary>
    /// Logs critical 'message'
    /// </summary>
    public void LogCritical(string message) => Logger.LogCritical(message);

    /// <summary>
    /// Clears all logs in current session
    /// </summary>
    /// <returns>Count of cleared logs</returns>
    public int ClearLogs() => Logger.ClearLogs();

    /// <summary>
    /// Removes all logs files in LogsPath
    /// </summary>
    /// <returns>Count of removed files</returns>
    public int RemoveAllLogsFiles()
    {
        if (_logger is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _logger = null;

        string[] files = Directory.GetFiles(LogsPath);
        int removed = 0;

        foreach (var file in files)
        {
            File.Delete(file);
            removed++;
        }

        return removed;
    }

    #endregion
}
