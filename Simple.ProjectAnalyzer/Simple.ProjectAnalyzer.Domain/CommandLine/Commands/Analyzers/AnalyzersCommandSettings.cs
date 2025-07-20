using System.ComponentModel;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Analyzers;

public class AnalyzersCommandSettings : CommandSettings
{
    [CommandOption("-l|--list")]
    [Description("List all available analyzers")]
    [DefaultValue(false)]
    public bool List { get; set; }
}