using System;
using Fusion.Core;

namespace Fusion;

public partial class Application : IDisposable
{
    private CommandLineParser? _commandLine;
    public CommandLineParser CommandLine => _commandLine ??= new CommandLineParser();

    /// <summary>
    /// Stops application
    /// </summary>
    /// <param name="code">Exit code</param>
    public void Exit(int code = 0)
    {
        Environment.Exit(code);
    }

    public void Dispose() => Exit();
}
