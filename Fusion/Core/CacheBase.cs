using System;
using MessagePack;

namespace Fusion.Core;

public abstract class CacheBase : ObservableStorage
{
    public abstract void InitFromBytes(byte[] bytes);
}
