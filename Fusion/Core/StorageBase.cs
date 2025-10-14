using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace Fusion.Core;

public abstract class StorageBase : IStorage
{
    private static readonly ConcurrentDictionary<Type, Action<StorageBase, string, object>> _setters = new();

    private static MethodInfo? singleSetterInstanse;
    private static MethodInfo? arraySetterInstanse;

    private static MethodInfo SingleSetter => singleSetterInstanse ??= typeof(StorageBase)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .First(m => m.Name == nameof(Set) && m.IsGenericMethod);

    private static MethodInfo ArraySetter => arraySetterInstanse ??= typeof(StorageBase)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .First(m => m.Name == nameof(SetArray) && m.IsGenericMethod);

    public abstract byte[] Data { get; }

    #region Maintains

    public abstract bool MaintainsSections { get; }
    public abstract bool MaintainsSubSections { get; }
    public abstract bool MaintainsArrays { get; }

    #endregion

    #region Setters

    public abstract void Set(string path, string value);

    public virtual void Set<T>(string path, T value)
    {
        Set(path, value?.ToString() ?? string.Empty);
    }

    public abstract void SetArray<T>(string path, T[] value);

    public virtual void SetMany<T>(IDictionary<string, T> values)
    {
        SetManyGenericSpecifyMethods(values, SingleSetter, ArraySetter);
    }

    public virtual void SetMany(IDictionary<string, object> values)
    {
        SetManyObjectsSpecifyMethods(values, SingleSetter, ArraySetter);
    }

    #endregion

    #region Getters

    public abstract string Get(string path);

    public virtual T Get<T>(string path)
    {
        Type Ttype = typeof(T);

        if (Ttype == typeof(string))
        {
            return (T)(object)Get(path);
        }

        // Check if T implements IParsable
        if (Ttype.GetInterface(typeof(IParsable<>).Name) != null ||
           Array.Exists(Ttype.GetInterfaces(), i =>
           i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>)))
        {
            MethodInfo? parseMethod = Ttype.GetMethod(
            "Parse",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [typeof(string), typeof(IFormatProvider)],
            modifiers: null);

            if (parseMethod != null)
            {
                object? result = parseMethod.Invoke(null, [Get(path), null!]);
                return (T)result!;
            }
        }

        throw new NotSupportedException($"Type {typeof(T)} is not supported by default Get<T> implementation");
    }

    public abstract T[] GetArray<T>(string path);

    #endregion

    #region Helpers

    protected virtual void SetManyGenericSpecifyMethods<T>(
        IDictionary<string, T> values,
        MethodInfo setMethod,
        MethodInfo setArrayMethod)
    {
        foreach (var value in values)
        {
            if (value.Value is Array array)
            {
                if (array.Rank != 1)
                    throw new NotSupportedException(
                        $"Only one-dimensional arrays are supported. Got {array.GetType()} for key {value.Key}");

                Type? elementType = typeof(T).GetElementType() ?? value.Value.GetType().GetElementType();
                if (elementType != null)
                {
                    var setter = _setters.GetOrAdd(array.GetType(), t =>
                    {
                        var elemType = t.GetElementType()!;
                        MethodInfo generic = setArrayMethod.MakeGenericMethod(elemType);
                        return (cfg, path, v) => generic.Invoke(cfg, [path, v]);
                    });

                    setter(this, value.Key, value.Value);
                    continue;
                }
            }

            var setterSingle = _setters.GetOrAdd(typeof(T), t =>
            {
                MethodInfo generic = setMethod.MakeGenericMethod(t);
                return (cfg, path, v) => generic.Invoke(cfg, [path, v]);
            });

            setterSingle(this, value.Key, value.Value!);
        }
    }

    protected void SetManyObjectsSpecifyMethods(
        IDictionary<string, object> values,
        MethodInfo setMethod,
        MethodInfo setArrayMethod)
    {
        foreach (var value in values)
        {
            if (value.Value is Array array)
            {
                if (array.Rank != 1)
                    throw new NotSupportedException(
                        $"Only one-dimensional arrays are supported. Got {array.GetType()} for key {value.Key}");

                Type? elementType = value.Value.GetType().GetElementType();
                if (elementType != null)
                {
                    var setter = _setters.GetOrAdd(value.Value.GetType(), t =>
                    {
                        var elemType = t.GetElementType()!;
                        MethodInfo generic = setArrayMethod.MakeGenericMethod(elemType);
                        return (cfg, path, v) => generic.Invoke(cfg, [path, v]);
                    });

                    setter(this, value.Key, value.Value);
                    continue;
                }
            }

            Type type = value.Value.GetType();
            var setterSingle = _setters.GetOrAdd(type, t =>
            {
                MethodInfo generic = setMethod.MakeGenericMethod(t);
                return (cfg, path, v) => generic.Invoke(cfg, [path, v]);
            });

            setterSingle(this, value.Key, value.Value);
        }
    }

    #endregion
}
