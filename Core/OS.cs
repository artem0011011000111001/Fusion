using System;

namespace Fusion.Core;

public static class OS
{
    private static readonly OperatingSystem _os = Environment.OSVersion;

    public static string FullName => _os.VersionString;
    public static string Description => System.Runtime.InteropServices.RuntimeInformation.OSDescription;
    public static PlatformID Platform => _os.Platform;
    public static Version Version => _os.Version;

    public static bool IsWindows => OperatingSystem.IsWindows();

    public static bool IsLinux => OperatingSystem.IsLinux();

    public static bool IsMacOS => OperatingSystem.IsMacOS();

    public static bool IsAndroid => OperatingSystem.IsAndroid();

    public static bool IsIOS => OperatingSystem.IsIOS();

    public static class Paths
    {
        public static string Home =>
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        ?? Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static string System
        {
            get
            {
                if (IsWindows)
                {
                    return Environment.Is64BitOperatingSystem
                        ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                        : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                        ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                }
                else if (IsLinux)   return "/opt";
                else if (IsMacOS)   return "/Applications";
                else if (IsAndroid) return "/system/app";
                else if (IsIOS)     return "/Applications";

                throw new PlatformNotSupportedException();
            }
        }

        public static string Shared
        {
            get
            {
                if (IsWindows)      return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                else if (IsLinux)   return "/usr/share";
                else if (IsMacOS)   return "/Library/Application Support";
                else if (IsAndroid) return "/data";
                else if (IsIOS)     return "/Library/Application Support";

                throw new PlatformNotSupportedException();
            }
        }

        public static string UserLocal
        {
            get
            {
                if (IsWindows) return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (IsLinux) return Path.Combine(Home, ".local", "share");
                if (IsMacOS) return Path.Combine(Home, "Library", "Application Support");
                if (IsAndroid) return Environment.GetFolderPath(Environment.SpecialFolder.Personal) ?? "/data/data";
                if (IsIOS) return Path.Combine(Home, "Library", "Application Support");

                throw new PlatformNotSupportedException();
            }
        }

        public static string UserGlobal
        {
            get
            {
                if (IsWindows) return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (IsLinux) return Path.Combine(Home, ".config");
                if (IsMacOS) return Path.Combine(Home, "Library", "Preferences");
                if (IsAndroid) return Path.Combine(Home, "shared_prefs");
                if (IsIOS) return Path.Combine(Home, "Library", "Preferences");

                throw new PlatformNotSupportedException();
            }
        }
    }
}
