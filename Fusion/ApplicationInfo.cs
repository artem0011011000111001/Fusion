using System;
using System.Text;

namespace Fusion;

public class ApplicationInfo(
    string appName,
    string companyName,
    string version = "1.0.0",
    string? description = null,
    string? license = null,
    DateTime? buildDate = null,
    string? website = null)
{
    public ApplicationIdenity Idenity { get; set; } = new(appName, companyName);
    public string Name => Idenity.Name;
    public string CompanyName => Idenity.CompanyName;
    public string Version { get; set; } = version;
    public string? Description { get; set; } = description;
    public string? License { get; set; } = license;
    public DateTime? BuildDate { get; set; } = buildDate;
    public string? Website { get; set; } = website;

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(Idenity.ToString());

        if (Description != null) stringBuilder.AppendLine(Description);
        if (License != null) stringBuilder.AppendLine($"Covered by '{License}' license");
        stringBuilder.AppendLine($"Version: {Version}");
        if (BuildDate != null) stringBuilder.AppendLine($"BuildDate: {BuildDate}");
        if (Website != null) stringBuilder.AppendLine(Website);

        return stringBuilder.ToString();
    }
}