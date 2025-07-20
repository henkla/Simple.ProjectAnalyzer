using System.ComponentModel;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;

public class GitCommandSettings : CommandSettings, IAnalyzeCommandSettings
{
    [CommandOption("-p|--path <PATH>")]
    [Description("The location of the project file(s) to analyze.")]
    public required string Path { get; set; }

    [CommandOption("-a|--analyzers <ANALYZER>")]
    [Description("Specify analyzers (can be used multiple times).")]
    public string[]? Analyzers { get; set; }
    
    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output.")]
    public bool Verbose { get; set; }

    public bool IsValid(out string message)
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            message = "Git repository path not specified";
            return false;
        }
        
        message = string.Empty;
        return true;
    }
}