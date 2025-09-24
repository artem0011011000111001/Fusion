using System;
using System.Text;

namespace Fusion.Core;

public interface IStorage : IReadOnlyStorage
{
    #region Setters

    /// <summary>
    /// Sets string value to the path
    /// </summary>
    void Set(string path, string value);

    /// <summary>
    /// Sets T value to the path
    /// </summary>
    /// <typeparam name="T">Type of value to set</typeparam>
    void Set<T>(string path, T value);

    /// <summary>
    /// Sets array to the path
    /// </summary>
    void SetArray<T>(string path, T[] value);

    /// <summary>
    /// Sets many typed values at once and persists them in one operation.
    /// </summary>
    /// <param name="values">Collection of key/value pairs</param>
    void SetMany<T>(IDictionary<string, T> values);

    /// <summary>
    /// Sets many typed values at once and persists them in one operation.
    /// </summary>
    /// <param name="values">Collection of key/value pairs</param>
    void SetMany(IDictionary<string, object> values);

    #endregion
}
