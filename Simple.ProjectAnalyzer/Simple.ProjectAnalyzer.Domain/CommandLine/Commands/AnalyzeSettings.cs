using System.ComponentModel;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public class AnalyzeSettings : CommandSettings
{
    [CommandOption("-p|--path <PATH>")]
    [Description("Path to the folder or .sln file to analyze.")]
    public string? Path { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output.")]
    public bool Verbose { get; set; }

    public bool AreValid(out string message)
    {
        if (Path is null || !Directory.Exists(Path))
        {
            Path ??= Directory.GetCurrentDirectory();
        }
        
        message = string.Empty;
        return true;
    }
}