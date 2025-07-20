using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;

public class GitCommandHandler(
    GitService gitService,
    ProjectFinder projectFinder,
    ProjectParser projectParser,
    DotnetService dotnetService,
    Orchestrator orchestrator,
    OutputHandler outputHandler
) : IAnalyzeCommandHandler
{
    public async Task<Context> HandleCommand(IAnalyzeCommandSettings analyzeCommandSettings)
    {
        Output.Figlet("Analyzing git");
        Output.Verbose($"{nameof(GitCommandHandler)}.{nameof(HandleCommand)} started");

        var repositoryUri = new Uri(analyzeCommandSettings.Path);
        var repositoryPath = gitService.Clone(repositoryUri);

        var projectFiles = projectFinder.FindProjectFiles(repositoryPath);
        var projects = projectParser.ParseProjectFiles(projectFiles);

        if (projects.Any(p => p.TargetFrameworks.Count == 0))
        {
            throw new Exception("There are projects with no TargetFrameworks.");
        }

        var currentLtsVersion = await dotnetService.GetCurrentLtsVersion();
        var context = new Context
        {
            Projects = projects,
            AnalyzeCommandSettings = analyzeCommandSettings,
            CurrentLtsVersion = currentLtsVersion
        };

        await orchestrator.AnalyzeProjects(context);

        if (context.AnalysisHasRun)
        {
            outputHandler.AnalysisResultToConsole(context);
        }

        return context;
    }
}