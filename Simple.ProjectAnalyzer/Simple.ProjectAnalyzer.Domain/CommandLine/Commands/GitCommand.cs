using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Services;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public class GitCommand(
    GitCloneService gitCloneService,
    ProjectFinder projectFinder,
    ProjectParser projectParser,
    Orchestrator orchestrator,
    ResultOutputHandler resultOutputHandler
    ) : AsyncCommand<GitCommandSettings>
{
    public const string CommandName = "git";
    public const string CommandDescription = "Performs a static analysis of one or more .NET projects in a " +
                                             "git repository and outputs diagnostics related to framework " +
                                             "versions, package references, DLL dependencies, and upgrade suggestions.";

    public override async Task<int> ExecuteAsync(CommandContext commandContext, GitCommandSettings commandSettings)
    {
        if (!commandSettings.IsValid(out var validationMessage))
        {
            throw new ProjectAnalyzerException("Invalid command settings: " + validationMessage);
        }

        var repositoryUri = new Uri(commandSettings.Path);
        var repository = gitCloneService.CloneRepository(repositoryUri);
        
        var projectFiles = projectFinder.FindProjectFiles(repository, commandSettings.Verbose);
        var projects = projectParser.ParseMany(projectFiles);
        
        if (projects.Any(p => p.TargetFrameworks.Count == 0))
        {
            throw new Exception("There are projects with no TargetFrameworks.");
        }
        
        var context = await orchestrator.AnalyzeProjects(projects, commandSettings);
        resultOutputHandler.PrintResultToConsole(context);

        return (int)ExitCode.Hint;
    }
}