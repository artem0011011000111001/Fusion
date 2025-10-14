using System;
using Fusion.Core;

namespace Fusion;

public partial class Application : IDisposable
{
    private CommandLineParser? _commandLine;
    public CommandLineParser CommandLine => _commandLine ??= new CommandLineParser();

    public void Exit(int code)
    {
        RemoveIntermediateCaches();

        Environment.Exit(code);
    }

    public void Exit() => Exit(0);

    public void Dispose() => Exit();
}
