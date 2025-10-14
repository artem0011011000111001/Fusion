using System;
using System.Text;

namespace Fusion.Core;

public interface IReadOnlyStorage
{
    byte[] Data { get; }

    #region Maintain

    bool MaintainsSections { get; }
    bool MaintainsSubSections { get; }
    bool MaintainsArrays { get; }

    #endregion

    #region Getters

    /// <summary>
    /// Returns string value from the path.
    /// </summary>
    string Get(string path);

    /// <summary>
    /// Returns T value from the path.
    /// </summary>
    /// <typeparam name="T">Type of value to return</typeparam>
    T Get<T>(string path);

    /// <summary>
    /// Returns double value from the path.
    /// </summary>
    /// <typeparam name="T">Type of array element</typeparam>
    T[] GetArray<T>(string path);

    #endregion
}
