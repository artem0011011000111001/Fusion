Simple application example:

```C#
ApplicationInfo info = new("TestApp", "TestCompany");

Application app = Application.InstallOrLoad(info);

IniConfig config = app.MakeOrLoadConfig<IniConfig>("TestConfig.ini");

config.Set("TestKey", "TestValue");

Console.WriteLine(config.Get("TestKey"));

app.Log("Some message");
```