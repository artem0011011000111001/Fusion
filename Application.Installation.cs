using System;
using Fusion.Core;

namespace Fusion;

public partial class Application(string localPath, string globalPath, string sharedPath)
{
    public string LocalPath { get; } = localPath;
    public string GlobalPath { get; } = globalPath;
    public string SharedPath { get; } = sharedPath;


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

        return app;
    }

    /// <summary>
    /// Loads application
    /// </summary>
    /// <returns>Application object</returns>
    public static Application Load(ApplicationIdenity identity)
    {
        if (Exists(identity))
        {
            (string local, string global, string shared) = GetLocalGlobalSharedPaths(identity);

            return new Application(local, global, shared);
        }

        throw new Exception($"Application {identity} does not exist");
    }

    /// <summary>
    /// Tries to load application
    /// </summary>
    /// <returns>If application was loaded</returns>
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
    /// If application exists loads it otherwise installs new one
    /// </summary>
    /// <returns>Created or loaded application</returns>
    public static Application InstallOrLoad(ApplicationInfo applicationInfo)
        => Exists(applicationInfo) ? Load(applicationInfo) : Install(applicationInfo);
}
