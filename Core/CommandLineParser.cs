using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusion.Core;

public class CommandLineParser
{
    private readonly Dictionary<string, Action<Args>> _handlers = new();

    /// <summary>
    /// Sets handler with name 'key' and body 'action'
    /// </summary>
    /// <param name="key">Name of handler</param>
    /// <param name="action">Body of handler</param>
    public void Set(string key, Action<Args> action)
    {
        _handlers[key] = action;
    }

    /// <summary>
    /// Removes handler by 'key'
    /// </summary>
    /// <param name="key">Name of handler</param>
    public void Remove(string key)
    {
        _handlers.Remove(key);
    }

    /// <summary>
    /// Removes all handlers
    /// </summary>
    public void Reset()
    {
        _handlers.Clear();
    }

    /// <summary>
    /// Parses args by handlers
    /// </summary>
    public void Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        var source = args;

        for (int i = 0; i < source.Length; i++)
        {
            string token = source[i];

            if (_handlers.TryGetValue(token, out var handler))
            {
                List<string> values = new();
                int j = i + 1;

                while (j < source.Length && !_handlers.ContainsKey(source[j]))
                {
                    values.Add(source[j]);
                    j++;
                }

                handler(new Args(values.ToArray()));

                i = j - 1;
            }
        }
    }
}
