using System;

namespace Fusion.Core;

public class Args(string[] args)
{
    private readonly string[] _args = args;

    public string[] Source => _args;
    public int Count => _args.Length;

    public string? this[int index] => Get(index);
    public int? this[string key] => GetIndex(key);

    public bool Exists(string key) => _args.Contains(key);

    public int? GetIndex(string key)
    {
        if (Exists(key))
        {
            return Array.IndexOf(_args, key);
        }

        return null;
    }

    public bool TryGetIndex(string key, out int index)
    {
        if (Exists(key))
        {
            index = Array.IndexOf(_args, key);
            return true;
        }

        index = default;
        return false;
    }

    public string? Get(int index)
    {
        if (index >= 0 && index < _args.Length)
        {
            return _args[index];
        }

        return null;
    }

    public bool TryGet(int index, out string value)
    {
        if (index >= 0 && index < _args.Length)
        {
            value = _args[index];
            return true;
        }

        value = default!;
        return false;
    }

    public int GetCountAfter(string key)
    {
        if (TryGetIndex(key, out int index))
        {
            return _args.Length - index;
        }

        return 0;
    }
}
