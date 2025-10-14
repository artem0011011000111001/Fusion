using System;
using System.Reflection;
using Fusion.Storages;

namespace Fusion;

public class Api
{
    private readonly Dictionary<string, Delegate> _functions = new();

    public bool Exists(string name)
        => _functions.ContainsKey(name);

    public void Add(string name, Delegate function)
        => _functions.Add(name, function);

    public void Remove(string name)
        => _functions.Remove(name);

    public TDelegate Get<TDelegate>(string name) where TDelegate : Delegate
        => (TDelegate)_functions[name];

    public bool TryGet<TDelegate>(string name, out TDelegate @delegate) where TDelegate : Delegate
    {
        if (_functions.TryGetValue(name, out Delegate? value) && value is not null && value is TDelegate typedValue)
        {
            @delegate = typedValue;
            return true;
        }

        @delegate = default!;
        return false;
    }

    public static class Constants
    {
        public const string ApiFolderName = "Api";

        public static class Manifest
        {
            public const string Filename = "Manifest.ini";

            public const string ApiFilenameProperty = "ApiFilename";
        }
    }

    public static Api Load(ApplicationIdenity idenity)
    {
        string apiPath = Path.Combine(Application.GetSharedPath(idenity), Constants.ApiFolderName);

        if (Directory.Exists(apiPath))
        {
            string manifestPath = Path.Combine(apiPath, Constants.Manifest.Filename);

            if (File.Exists(manifestPath))
            {
                IniConfig manifest = new(manifestPath);

                string apiFilename = manifest.Get(Constants.Manifest.ApiFilenameProperty);
                string apiFilePath = Path.Combine(apiPath, apiFilename);

                if (!File.Exists(apiPath))
                {
                    throw new FileNotFoundException($"File {apiPath} is not found");
                }

                Type? type = Assembly.LoadFrom(Path.Combine(apiPath, apiFilename))
                    .GetTypes().FirstOrDefault(t => typeof(IApplicationApi).IsAssignableFrom(t) && !t.IsInterface)
                    ?? throw new Exception($"IApplicationApi not found in {apiFilename}");

                object? instance = Activator.CreateInstance(type)
                    ?? throw new Exception($"Failed to create instance from type {type}");

                return ((IApplicationApi)instance).Get();
            }
            else
            {
                throw new FileNotFoundException($"File {manifestPath} is not found");
            }
        }

        throw new DirectoryNotFoundException($"Directory {apiPath} is not found");
    }

    public static bool TryLoad(ApplicationIdenity idenity, out Api api)
    {
        try
        {
            api = Load(idenity);
            return true;
        }
        catch
        {
            api = default!;
            return false;
        }
    }
}
