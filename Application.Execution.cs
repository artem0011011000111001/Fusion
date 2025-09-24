using System;
using Fusion.Core;

namespace Fusion;

public partial class Application
{
    private CommandLineParser _commandLine;
    public CommandLineParser CommandLine => _commandLine ??= new CommandLineParser();
}
