using System;
using Fusion.Core;
using Fusion.Storages;

namespace Fusion;

public partial class Application
{
    private static byte[] MakeInfoConfigData(ApplicationInfo applicationInfo)
    {
        IniConfig config = new();

        config.SetMany(new Dictionary<string, object>
        {
            [Constants.InfoConfig.ApplicationNamePath] = applicationInfo.Name,
            [Constants.InfoConfig.CompanyNamePath] = applicationInfo.CompanyName,
            [Constants.InfoConfig.VersionPath] = applicationInfo.Version,
            [Constants.InfoConfig.DescriptionPath] = applicationInfo.Description ?? "",
            [Constants.InfoConfig.LicensePath] = applicationInfo.License ?? "",
            [Constants.InfoConfig.BuildDatePath] = (applicationInfo.BuildDate ?? DateTime.MinValue).ToString(),
            [Constants.InfoConfig.WebsitePath] = applicationInfo.Website ?? ""
        });

        return config.Data;
    }

    private static ApplicationInfo GetApplicationInfoFromInfoConfig(IniConfig infoConfig)
        => new(
            infoConfig.Get(Constants.InfoConfig.ApplicationNamePath),
            infoConfig.Get(Constants.InfoConfig.CompanyNamePath),
            infoConfig.Get(Constants.InfoConfig.VersionPath),
            infoConfig.Get(Constants.InfoConfig.DescriptionPath),
            infoConfig.Get(Constants.InfoConfig.LicensePath),
            infoConfig.Get<DateTime>(Constants.InfoConfig.BuildDatePath),
            infoConfig.Get(Constants.InfoConfig.WebsitePath)
            );

    private static string GetSession() => $"Session_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";

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
