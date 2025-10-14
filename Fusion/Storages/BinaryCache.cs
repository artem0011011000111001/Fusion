using Fusion.Core;
using MessagePack;
using System;

namespace Fusion.Storages;

public class BinaryCache : CacheBase
{
    private Dictionary<string, byte[]> _store = new();

    public override byte[] Data => MessagePackSerializer.Serialize(_store);

    public override bool MaintainsSections => false;
    public override bool MaintainsSubSections => false;
    public override bool MaintainsArrays => true;

    public override void InitFromBytes(byte[] bytes)
    {
        _store = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(bytes);
    }

    public override string Get(string path)
    {
        if (TryGetBytes(path, out var bytes))
            return MessagePackSerializer.Deserialize<string>(bytes);

        throw new Exception($"Path '{path}' is not found");
    }

    public override T Get<T>(string path)
    {
        if (TryGetBytes(path, out var bytes))
            return MessagePackSerializer.Deserialize<T>(bytes);

        throw new Exception($"Path '{path}' is not found");
    }

    public override T[] GetArray<T>(string path)
    {
        if (TryGetBytes(path, out var bytes))
            return MessagePackSerializer.Deserialize<T[]>(bytes);

        throw new Exception($"Path '{path}' is not found");
    }

    protected override void SetImplementation(string path, string value)
    {
        _store[path] = MessagePackSerializer.Serialize(value);
    }

    protected override void SetImplementation<T>(string path, T value)
    {
        _store[path] = MessagePackSerializer.Serialize(value);
    }

    protected override void SetArrayImplementation<T>(string path, T[] value)
    {
        _store[path] = MessagePackSerializer.Serialize(value);
    }

    private bool TryGetBytes(string path, out byte[] bytes)
    {
        return _store.TryGetValue(path, out bytes!);
    }
}
