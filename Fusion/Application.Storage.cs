using Fusion.Core;
using Fusion.Storages;
using System;

namespace Fusion;

public partial class Application
{
    public string ConfigPath { get; private set; }
    public string CachePath { get; private set; }
    public string LogsPath { get; private set; }
    public string ApiPath { get; private set; }

    #region Config

    private ApplicationInfo? _applicationInfo = null;
    public ApplicationInfo Info => _applicationInfo ??= GetApplicationInfoFromInfoConfig(LoadConfig<IniConfig>(Constants.InfoConfig.Filename));
    public string Name => Info.Name;
    public string CompanyName => Info.CompanyName;
    public string Version => Info.Version;
    public string? Description => Info.Description;
    public string? License => Info.License;
    public DateTime? BuildDate => Info.BuildDate;
    public string? Website => Info.Website;


    /// <returns>If config exists</returns>
    public bool HasConfig(string name)
        => File.Exists(Path.Combine(ConfigPath, name));

    /// <summary>
    /// Creates config in 'ConfigPath' with specified name
    /// </summary>
    /// <remarks>If config already exists it will be deleted</remarks>
    /// <typeparam name="TConfig">Config type</typeparam>
    /// <returns>Created config</returns>
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

    /// <summary>
    /// Loads config from specified name
    /// </summary>
    /// <typeparam name="TConfig">Config type</typeparam>
    /// <returns>Loaded config</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="InitializationException"></exception>
    public TConfig LoadConfig<TConfig>(string name) where TConfig : ConfigBase, new()
    {
        string path = Path.Combine(ConfigPath, name);

        if (!File.Exists(path)) throw new FileNotFoundException($"Config '{name}' is not found");

        TConfig config = new();
        config.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, config.Data);
        };

        if (!config.InitFromFile(path)) throw new InitializationException($"Failed to initialize config '{name}' with type '{config.GetType()}' from '{path}'");

        return config;
    }

    /// <summary>
    /// Tries to load config with specified name
    /// </summary>
    /// <typeparam name="TConfig">Config type</typeparam>
    /// <returns>If config has been loaded</returns>
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

    /// <summary>
    /// Loads config if it already exists; otherwise creates a new one
    /// </summary>
    /// <typeparam name="TConfig">Config type</typeparam>
    /// <returns>Created or loaded config</returns>
    public TConfig MakeOrLoadConfig<TConfig>(string name) where TConfig : ConfigBase, new()
        => HasConfig(name) ? LoadConfig<TConfig>(name) : MakeConfig<TConfig>(name);

    #endregion

    #region Caching

    private CacheBase? _intermediateCache = null;

    /// <summary>
    /// Represents intermediate cache that does not exist between sessions
    /// </summary>
    /// <remarks>Value is 'BinaryCache' by default</remarks>
    public CacheBase IntermediateCache
    {
        get => _intermediateCache ??= new BinaryCache();
        set => _intermediateCache = value;
    }

    /// <returns>If cache exists</returns>
    public bool HasCache(string name)
        => File.Exists(Path.Combine(CachePath, name));

    /// <summary>
    /// Creates cache in 'CachePath' with specified name
    /// </summary>
    /// <remarks>If cache already exists it will be deleted</remarks>
    /// <typeparam name="TCache">Cache type</typeparam>
    /// <returns>Created cache</returns>
    public TCache MakeCache<TCache>(string name) where TCache : CacheBase, new()
    {
        string path = Path.Combine(CachePath, name);
        TCache cache = new();

        cache.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, cache.Data);
        };

        if (File.Exists(path))
            File.Delete(path);

        File.Create(path).Dispose();

        return cache;
    }

    /// <summary>
    /// Loads cache from specified name
    /// </summary>
    /// <typeparam name="TCache">Cache type</typeparam>
    /// <returns>Loaded cache</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public TCache LoadCache<TCache>(string name) where TCache : CacheBase, new()
    {
        string path = Path.Combine(CachePath, name);

        if (!File.Exists(path)) throw new FileNotFoundException($"Cache '{name}' is not found");

        TCache cache = new();
        cache.ValueChanged += (_, _) =>
        {
            File.WriteAllBytes(path, cache.Data);
        };

        cache.InitFromBytes(File.ReadAllBytes(path));

        return cache;
    }

    /// <summary>
    /// Tries to load cache with specified name
    /// </summary>
    /// <typeparam name="TCache">Cache type</typeparam>
    /// <returns>If cache has been loaded</returns>
    public bool TryLoadCache<TCache>(string name, out TCache cache) where TCache : CacheBase, new()
    {
        try
        {
            cache = LoadCache<TCache>(name);
            return true;
        }
        catch
        {
            cache = default!;
            return false;
        }
    }

    /// <summary>
    /// Loads cache if it already exists; otherwise creates a new one
    /// </summary>
    /// <typeparam name="TCache">Cache type</typeparam>
    /// <returns>Created or loaded cache</returns>
    public TCache MakeOrLoadCache<TCache>(string name) where TCache : CacheBase, new()
        => HasCache(name) ? LoadCache<TCache>(name) : MakeCache<TCache>(name);

    /// <summary>
    /// Clears a cache with specified name
    /// </summary>
    /// <returns>If cache has been cleared</returns>
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

    /// <summary>
    /// Application logger
    /// </summary>
    /// <remarks>Value is 'FileLogger' by default</remarks>
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
