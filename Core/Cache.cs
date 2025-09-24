using System;
using System.Text;

namespace Fusion.Core;

public class Cache : ObservableStorage, IDisposable
{
    private readonly string _filePath;
    private StreamWriter _streamWriter;
    private StreamReader _streamReader;
    private readonly Lock _lock = new();

    public Cache(string filePath)
    {
        _filePath = filePath;
        _streamWriter = new StreamWriter(filePath, append: true) { AutoFlush = true };
        _streamReader = new StreamReader(filePath);
    }

    public override byte[] Data
    {
        get
        {
            lock (_lock)
            {
                return GetBytes(_streamReader.ReadToEnd());
            }
        }
    }

    public override bool MaintainsSections => false;

    public override bool MaintainsSubSections => false;

    public override bool MaintainsArrays => true;

    public override string Get(string path)
    {
        return null!; // Not implemented
    }

    public override T[] GetArray<T>(string path)
    {
        throw new NotImplementedException();
    }

    protected override void SetArrayImplementation<T>(string path, T[] value)
    {
        throw new NotImplementedException();
    }

    protected override void SetImplementation(string path, string value)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _streamWriter.Dispose();
            _streamReader.Dispose();
        }
    }

    private static byte[] GetBytes(string value)
        => Encoding.UTF8.GetBytes(value);
}
