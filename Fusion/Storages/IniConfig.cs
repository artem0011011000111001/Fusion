using Fusion.Core;
using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Fusion.Storages;

public class IniConfig : ConfigBase
{
    private IniData _data;

    public IniConfig()
    {
        _data = new IniData();
    }

    public IniConfig(string filePath)
    {
        if (!InitFromFile(filePath))
            _data = new IniData();
    }

    public override byte[] Data => Encoding.UTF8.GetBytes(_data.ToString());

    #region Maintain

    public override bool MaintainsSections => true;
    public override bool MaintainsSubSections => false;
    public override bool MaintainsArrays => false;

    #endregion

    [MemberNotNullWhen(true, nameof(_data))]
    public override bool InitFromFile(string path)
    {
        if (File.Exists(path))
        {
            FileIniDataParser parser = new();
            _data = parser.ReadFile(path);

            return true;
        }

        return false;
    }

    #region Setters

    protected override void SetImplementation(string path, string value)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        var (section, key) = ParsePath(path);

        if (string.IsNullOrEmpty(section))
            _data.Global[key] = value;
        else
        {
            _data.Sections.AddSection(section);
            _data[section][key] = value;
        }
    }

    protected override void SetArrayImplementation<T>(string path, T[] value)
    {
        Set(path, string.Join(",", value));
    }

    #endregion

    #region Getters

    public override string Get(string path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        var (section, key) = ParsePath(path);

        string? value = null;

        if (string.IsNullOrEmpty(section) && _data.Global.ContainsKey(key))
            value = _data.Global[key];
        else if (_data.Sections.ContainsSection(section) && _data[section].ContainsKey(key))
            value = _data[section][key];

        if (value is not null)
            return value;

        throw new Exception($"Path '{path}' is not found");
    }

    public override T[] GetArray<T>(string path)
    {
        string str = Get(path);

        string[] parts = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        T[] result = new T[parts.Length];

        for (int i = 0; i < parts.Length; i++)
            result[i] = (T)Convert.ChangeType(parts[i].Trim(), typeof(T));

        return result;
    }

    #endregion

    #region Helpers

    private static (string section, string key) ParsePath(string path)
    {
        int lastDot = path.LastIndexOf('.');
        if (lastDot == -1)
            return (string.Empty, path);
        return (path[..lastDot], path[(lastDot + 1)..]);
    }

    #endregion
}