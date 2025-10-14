using Fusion.Core;
using Fusion.Storages;
using System;
using System.ComponentModel;
using System.Security.Principal;

namespace Fusion;

public partial class Application
{
    public string ConfigPath { get; private set; }
    public string CachePath { get; private set; }
    public string LogsPath { get; private set; }
    public string ApiPath { get; private set; }

    private string? _intermediateCachePath = null;
    public string IntermediateCachePath => _intermediateCachePath ??= Path.Combine(CachePath, Constants.IntermediateCacheFolderName);

    #region Config

    private ApplicationInfo? _applicationInfo = null;
    public ApplicationInfo Info => _applicationInfo ??= GetApplicationInfoFromInfoConfig(LoadConfig<IniConfig>(Constants.InfoConfig.Filename));
    public string Name => Info.Name;
    public string CompanyName => Info.CompanyName;
    public string Version => Info.Version;
    public string? Description =>  Info.Description;
    public string? License => Info.License;
    public DateTime? BuildDate => Info.BuildDate;
    public string? Website => Info.Website;


    /// <returns>If config exists</returns>
    public bool HasConfig(string name)
        => File.Exists(Path.Combine(ConfigPath, name));

    public TConfig MakeConfig<TConfig>(string name) where TConfig : ConfigBase, new()
    {
        string path = Path.Combine(ConfigPath, name);
        TConfig config = new();

        config.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, config.Data);
        };

        if (File.Exists(path))
            File.Delete(path);

        File.Create(path).Dispose();

        return config;
    }

    public TConfig LoadConfig<TConfig>(string name) where TConfig : ConfigBase, new()
    {
        string path = Path.Combine(ConfigPath, name);

        if (!File.Exists(path)) throw new FileNotFoundException($"Config '{name}' is not found");

        TConfig config = new();
        config.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, config.Data);
        };

        if (!config.InitFromFile(path)) throw new Exception($"Failed to initialize config '{name}' with type '{config.GetType()}' from '{path}'");

        return config;
    }

    public bool TryLoadConfig<TConfig>(string name, out TConfig config) where TConfig : ConfigBase, new()
    {
        try
        {
            config = LoadConfig<TConfig>(name);
            return true;
        }
        catch
        {
            config = default!;
            return false;
        }
    }

    public TConfig MakeOrLoadConfig<TConfig>(string name) where TConfig : ConfigBase, new()
        => HasConfig(name) ? LoadConfig<TConfig>(name) : MakeConfig<TConfig>(name);

    #endregion

    #region Caching

    private CacheBase? _intermediateCache = null;
    public CacheBase IntermediateCache
    {
        get
        {
            if (_intermediateCache is null)
            {
                _intermediateCache = new BinaryCache();

                string path = Path.Combine(IntermediateCachePath, $"{GetSession()}_cache");
                _intermediateCache.ValueChanged += (_, _) =>
                {
                    File.WriteAllBytes(path, _intermediateCache.Data);
                };

                File.Create(path).Dispose();
            }
            
            return _intermediateCache;
        }
        set => _intermediateCache = value;
    }

    public TCache MakeOrLoadCache<TCache>(string name) where TCache : CacheBase, new()
    {
        string path = Path.Combine(CachePath, name);
        TCache cache = new();

        cache.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, cache.Data);
        };

        if (File.Exists(path))
        {
            cache.InitFromBytes(File.ReadAllBytes(path));
            return cache;
        }

        File.Create(path).Dispose();

        return cache;
    }

    public bool ClearCache(string name)
    {
        string path = Path.Combine(CachePath, name);

        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }

        return false;
    }

    #endregion

    #region Logging

    private ObservableLogger? _logger;
    public ObservableLogger Logger
    {
        get => _logger ??= new FileLogger(Path.Combine(LogsPath, $"{GetSession()}_log.txt"));
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
