using System.Text;

namespace Fusion.Core;

public abstract class ConfigBase : ObservableStorage
{
    public virtual string StringifiedData { get => Encoding.UTF8.GetString(Data); }

    public abstract bool InitFromFile(string path);
}