namespace Fusion;

public partial class Application
{
    public static class Constants
    {
        public const string AppsFolderName = "FusionApps";

        public const string ConfigFolderName = "Config";
        public const string CacheFolderName = "Cache";
        public const string LogsFolderName = "Logs";
        public const string IntermediateCacheFolderName = "Intermediate";

        public static class InfoConfig
        {
            public const string Filename = "Info.ini";

            public const string ApplicationInfoSection = "AppInfo";

            public const string ApplicationNameProperty = "AppName";
            public const string CompanyNameProperty = "CompanyName";
            public const string VersionProperty = "Version";
            public const string DescriptionProperty = "Description";
            public const string LicenseProperty = "License";
            public const string BuildDateProperty = "BuildDate";
            public const string WebsiteProperty = "Website";

            public const string ApplicationNamePath = $"{ApplicationInfoSection}.{ApplicationNameProperty}";
            public const string CompanyNamePath = $"{ApplicationInfoSection}.{CompanyNameProperty}";
            public const string VersionPath = $"{ApplicationInfoSection}.{VersionProperty}";
            public const string DescriptionPath = $"{ApplicationInfoSection}.{DescriptionProperty}";
            public const string LicensePath = $"{ApplicationInfoSection}.{LicenseProperty}";
            public const string BuildDatePath = $"{ApplicationInfoSection}.{BuildDateProperty}";
            public const string WebsitePath = $"{ApplicationInfoSection}.{WebsiteProperty}";
        }
    }
}
