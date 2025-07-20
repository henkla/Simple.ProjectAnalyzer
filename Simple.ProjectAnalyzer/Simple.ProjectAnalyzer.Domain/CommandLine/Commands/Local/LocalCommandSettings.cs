using System.ComponentModel;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;

public class LocalCommandSettings : CommandSettings, IAnalyzeCommandSettings
{
    [CommandOption("-p|--path <PATH>")]
    [Description("The location of the project file(s) to analyze.")]
    public string? Path { get; set; }

    [CommandOption("-a|--analyzers <ANALYZER>")]
    [Description("Specify analyzers (can be used multiple times).")]
    public string[]? Analyzers { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output.")]
    public bool Verbose { get; set; }

    public bool IsValid(out string message)
    {
        if (Path is null || !Directory.Exists(Path))
        {
            Output.Verbose("Path not specified - using current directory.");
            Path ??= Directory.GetCurrentDirectory();
        }
        
        message = string.Empty;
        return true;
    }
}