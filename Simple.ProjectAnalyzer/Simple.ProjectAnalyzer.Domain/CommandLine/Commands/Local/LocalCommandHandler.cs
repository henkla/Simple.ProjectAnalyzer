using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;

public class LocalCommandHandler(
    ProjectFinder projectFinder,
    ProjectParser projectParser,
    DotnetService dotnetService,
    Orchestrator orchestrator,
    ResultOutputHandler resultOutputHandler
    ) : ICommandHandler
{
    public async Task<Context> HandleCommand(ICommandSettings commandSettings)
    {
        Output.Verbose($"{nameof(LocalCommandHandler)}.{nameof(HandleCommand)} started");
        
        var projectFiles = projectFinder.FindProjectFiles(commandSettings.Path);
        var projects = projectParser.ParseProjectFiles(projectFiles);
        if (projects.Any(p => p.TargetFrameworks.Count == 0))
        {
            throw new Exception("There are projects with no TargetFrameworks.");
        }
        
        var currentLtsVersion = await dotnetService.GetCurrentLtsVersion();
        var context = new Context
        {
            Projects = projects,
            CommandSettings = commandSettings,
            CurrentLtsVersion = currentLtsVersion
        };
        
        await orchestrator.AnalyzeProjects(context);
        resultOutputHandler.ToConsole(context);

        return context;
    }
}