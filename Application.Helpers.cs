using System;
using Fusion.Core;
using Fusion.Configs;

namespace Fusion;

public partial class Application
{
    private static byte[] MakeInfoConfigData(ApplicationInfo applicationInfo)
    {
        IniConfig config = new();

        config.Set(Constants.InfoConfig.ApplicationNamePath, applicationInfo.Name);
        config.Set(Constants.InfoConfig.CompanyNamePath, applicationInfo.CompanyName);
        config.Set(Constants.InfoConfig.VersionPath, applicationInfo.Version);
        config.Set(Constants.InfoConfig.DescriptionPath, applicationInfo.Description ?? "");
        config.Set(Constants.InfoConfig.BuildDatePath, (applicationInfo.BuildDate ?? DateTime.MinValue).ToString());
        config.Set(Constants.InfoConfig.WebsitePath, applicationInfo.Website ?? "");

        return config.Data;
    }

    internal static string GetLocalPath(ApplicationIdenity idenity)
        => Path.Combine(OS.Paths.UserLocal, Constants.AppsFolderName, idenity.CompanyName, idenity.Name);

    internal static string GetGlobalPath(ApplicationIdenity idenity)
        => Path.Combine(OS.Paths.UserGlobal, Constants.AppsFolderName, idenity.CompanyName, idenity.Name);

    internal static string GetSharedPath(ApplicationIdenity idenity)
        => Path.Combine(OS.Paths.Shared, Constants.AppsFolderName, idenity.CompanyName, idenity.Name);

    internal static (string, string, string) GetLocalGlobalSharedPaths(ApplicationIdenity idenity)
    {
        string appFolders = Path.Combine(Constants.AppsFolderName, idenity.CompanyName, idenity.Name);

        return (Path.Combine(OS.Paths.UserLocal, appFolders),
            Path.Combine(OS.Paths.UserGlobal, appFolders),
            Path.Combine(OS.Paths.Shared, appFolders));
    }
}
