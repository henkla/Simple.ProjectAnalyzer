using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Services;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public class AnalyzeCommand(
    ProjectFinder projectFinder,
    ProjectParser projectParser,
    Orchestrator orchestrator,
    ResultOutputHandler resultOutputHandler
    ) : AsyncCommand<AnalyzeCommandSettings>
{
    public const string CommandName = "local";
    public const string Description = "Performs a static analysis of one or more .NET projects and " +
                                      "outputs diagnostics related to framework versions, package " +
                                      "references, DLL dependencies, and upgrade suggestions.";
    
    public override async Task<int> ExecuteAsync(CommandContext commandContext, AnalyzeCommandSettings commandSettings)
    {
        if (!commandSettings.IsValid(out var validationMessage))
        {
            throw new ProjectAnalyzerException("Invalid command settings: " + validationMessage);
        }

        var projectFiles = projectFinder.FindProjectFiles(commandSettings);
        var projects = projectParser.ParseMany(projectFiles);
        if (projects.Any(p => p.TargetFrameworks.Count == 0))
        {
            throw new Exception("There are projects with no TargetFrameworks.");
        }
        
        var context = await orchestrator.AnalyzeProjects(projects, commandSettings);
        resultOutputHandler.PrintResultToConsole(context);

        return (int)context.ExitCode;
    }
}