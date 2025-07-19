using System.ComponentModel;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public class GitCommandSettings : CommandSettings, ICommandable
{
    [CommandOption("-p|--path <PATH>")]
    [Description("Path to the git repository")]
    public string Path { get; set; }

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