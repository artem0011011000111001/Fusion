using System;

namespace Fusion;

public class ApplicationIdenity(string appName, string companyName)
{
    public string Name { get; set; } = appName;
    public string CompanyName { get; set; } = companyName;

    public static implicit operator ApplicationIdenity(ApplicationInfo applicationInfo)
    {
        return new(applicationInfo.Name, applicationInfo.CompanyName);
    }

    public override string ToString() => $"{Name} by {CompanyName}";
}
