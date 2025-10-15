using Fusion.Core;
using Fusion.Storages;
using System;

namespace Fusion;

public partial class Application
{
    public string LocalPath { get; }
    public string GlobalPath { get; }
    public string SharedPath { get; }

    private Application(string localPath, string globalPath, string sharedPath)
    {
        LocalPath = localPath;
        GlobalPath = globalPath;
        SharedPath = sharedPath;
        ConfigPath = Path.Combine(GlobalPath, Constants.ConfigFolderName);
        CachePath = Path.Combine(LocalPath, Constants.CacheFolderName);
        LogsPath = Path.Combine(LocalPath, Constants.LogsFolderName);
        ApiPath = Path.Combine(SharedPath, Api.Constants.ApiFolderName);
    }

    /// <returns>If application exists</returns>
    public static bool Exists(ApplicationIdenity applicationIdenity)
    {
        (string local, string global, string shared) = GetLocalGlobalSharedPaths(applicationIdenity);

        return Directory.Exists(local) && Directory.Exists(global) && Directory.Exists(shared);
    }

    /// <summary>
    /// Installs application folders and files
    /// </summary>
    /// <returns>Created application</returns>
    public static Application Install(ApplicationInfo applicationInfo)
    {
        (string local, string global, string shared) 
            = GetLocalGlobalSharedPaths(applicationInfo);

        Application app = new(local, global, shared);

        Directory.CreateDirectory(app.LocalPath);
        Directory.CreateDirectory(app.GlobalPath);
        Directory.CreateDirectory(app.SharedPath);

        Directory.CreateDirectory(app.ConfigPath);

        string baseConfigPath = Path.Combine(app.ConfigPath, Constants.InfoConfig.Filename);
        File.WriteAllBytes(baseConfigPath, MakeInfoConfigData(applicationInfo));

        Directory.CreateDirectory(app.CachePath);
        Directory.CreateDirectory(app.LogsPath);

        app._applicationInfo = applicationInfo;

        return app;
    }

    /// <summary>
    /// Loads application
    /// </summary>
    /// <returns>Application object</returns>
    /// <exception cref="Exception"></exception>
    public static Application Load(ApplicationIdenity identity)
    {
        if (Exists(identity))
        {
            (string local, string global, string shared) = GetLocalGlobalSharedPaths(identity);

            Application app = new(local, global, shared);

            app._applicationInfo = 
                app.TryLoadConfig(Constants.InfoConfig.Filename, out IniConfig infoConfig) 
                ? GetApplicationInfoFromInfoConfig(infoConfig) 
                : new(identity.Name, identity.CompanyName);

            return app;
        }

        throw new Exception($"Application {identity} does not exist");
    }

    /// <summary>
    /// Tries to load application
    /// </summary>
    /// <returns>If application has been loaded</returns>
    public static bool TryLoad(ApplicationIdenity identity, out Application app)
    {
        try
        {
            app = Load(identity);
            return true;
        }
        catch
        {
            app = default!;
            return false;
        }
    }

    /// <summary>
    /// Loads application if it already exists; otherwise installs a new one
    /// </summary>
    /// <returns>Created or loaded application</returns>
    public static Application InstallOrLoad(ApplicationInfo applicationInfo)
        => Exists(applicationInfo) ? Load(applicationInfo) : Install(applicationInfo);
}
