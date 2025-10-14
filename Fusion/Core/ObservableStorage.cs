using System;
using System.Reflection;

namespace Fusion.Core;

public abstract class ObservableStorage : StorageBase
{
    private static MethodInfo? singleSetterInstanse;
    private static MethodInfo? arraySetterInstanse;

    private static MethodInfo SingleSetter => singleSetterInstanse ??= typeof(ObservableStorage)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(m => m.Name == nameof(SetImplementation) && m.IsGenericMethod);

    private static MethodInfo ArraySetter => arraySetterInstanse ??= typeof(ObservableStorage)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(m => m.Name == nameof(SetArrayImplementation) && m.IsGenericMethod);

    #region Events

    /// <summary>
    /// Invokes when some value has been changed
    /// First param is a path
    /// Second param is stringified value
    /// </summary>
    public event Action<string, string>? ValueChanged;

    protected void OnValueChanged(string path, string value) => ValueChanged?.Invoke(path, value);

    #endregion

    #region Setters

    protected abstract void SetImplementation(string path, string value);

    protected virtual void SetImplementation<T>(string path, T value)
    {
        SetImplementation(path, value?.ToString() ?? string.Empty);
    }

    protected abstract void SetArrayImplementation<T>(string path, T[] value);

    protected virtual void SetManyImplementation<T>(IDictionary<string, T> values)
    {
        SetManyGenericSpecifyMethods(values, SingleSetter, ArraySetter);
    }

    protected virtual void SetManyImplementation(IDictionary<string, object> values)
    {
        SetManyObjectsSpecifyMethods(values, SingleSetter, ArraySetter);
        var first = values.FirstOrDefault();

        OnValueChanged(first.Key, first.Value?.ToString() ?? string.Empty);
    }

    public override void Set(string path, string value)
    {
        SetImplementation(path, value);
        OnValueChanged(path, value);
    }

    public override void Set<T>(string path, T value)
    {
        SetImplementation(path, value);
        OnValueChanged(path, value?.ToString() ?? string.Empty);
    }

    public override void SetArray<T>(string path, T[] value)
    {
        SetArrayImplementation(path, value);
        OnValueChanged(path, value?.ToString() ?? string.Empty);
    }

    public override void SetMany<T>(IDictionary<string, T> values)
    {
        SetManyImplementation(values);

        var first = values.FirstOrDefault();
        OnValueChanged(first.Key, first.Value?.ToString() ?? string.Empty);
    }

    public override void SetMany(IDictionary<string, object> values)
    {
        SetManyImplementation(values);

        var first = values.FirstOrDefault();
        OnValueChanged(first.Key, first.Value?.ToString() ?? string.Empty);
    }

    #endregion
}
