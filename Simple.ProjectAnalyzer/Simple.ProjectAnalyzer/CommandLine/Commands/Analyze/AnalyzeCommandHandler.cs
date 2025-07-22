using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.CommandLine.Commands.Analyze;

public class AnalyzeCommandHandler(
    GitService gitService,
    ProjectFinder projectFinder,
    ProjectParser projectParser,
    DotnetService dotnetService,
    Orchestrator orchestrator,
    OutputHandler outputHandler,
    IConsoleOutput console
)
{
    public async Task<int> HandleCommand(AnalyzeCommandSettings settings)
    {
        console.Verbose($"{nameof(AnalyzeCommandHandler)}.{nameof(HandleCommand)} started");

        var path = settings.Path;

        if (settings.IsGitRepository())
        {
            var repositoryUri = new Uri(settings.Path);
            path = gitService.Clone(repositoryUri);    
        }

        var projectFiles = projectFinder.FindProjectFiles(path);
        var projects = projectParser.ParseProjectFiles(projectFiles);

        if (projects.Any(p => p.TargetFrameworks.Count == 0))
        {
            throw new Exception("There are projects with no TargetFrameworks.");
        }

        var currentLtsVersion = await dotnetService.GetCurrentLtsVersion();
        var context = new Context
        {
            Projects = projects,
            CurrentLtsVersion = currentLtsVersion,
            Path = settings.Path,
            Verbose = settings.Verbose,
            Analyzers = settings.Analyzers
        };

        await orchestrator.AnalyzeProjects(context);

        if (context.AnalysisHasRun)
        {
            outputHandler.AnalysisResultToConsole(context);
        }

        return (int)context.ApplicationExitCode;
    }
}