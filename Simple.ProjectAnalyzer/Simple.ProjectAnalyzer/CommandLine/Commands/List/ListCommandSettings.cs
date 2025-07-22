using System.ComponentModel;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.CommandLine.Commands.List;

public class ListCommandSettings : CommandSettings
{
    [CommandOption("-a|--analyzers")]
    [Description("List all available analyzers")]
    [DefaultValue(false)]
    public bool Analyzers { get; set; }
}