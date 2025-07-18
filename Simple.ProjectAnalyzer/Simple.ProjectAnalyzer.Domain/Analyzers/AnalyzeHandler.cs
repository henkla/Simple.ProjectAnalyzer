using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class AnalyzeHandler(
    ProjectTypeAnalyzer projectTypeAnalyzer,
    // TargetFrameworkAnalyzer targetFrameworkAnalyzer,
    CurrentLtsService currentLtsService,
    OldFrameworkAnalyzer oldFrameworkAnalyzer,
    ExternalDllAnalyzer  externalDllAnalyzer,
    OutdatedDependenciesAnalyzer outdatedDependenciesAnalyzer,
    UnusedDependenciesAnalyzer unusedDependenciesAnalyzer
)
{
    public async Task<AnalysisContext> AnalyzeProjects(List<Project> projectsToAnalyze, AnalyzeSettings settings)
    {
        // in order to perform analysis on project files, we need to know 
        // which version of dotnet that is current LTS
        var currentLtsVersion = await currentLtsService.GetCurrentLtsVersion();

        var context = new AnalysisContext
        {
            Projects = projectsToAnalyze,
            Settings = settings,
            CurrentLtsVersion = currentLtsVersion
        };

        var analysisTasks = new List<Task>()
        {
            oldFrameworkAnalyzer.Run(ref context),
            externalDllAnalyzer.Run(ref context),
            outdatedDependenciesAnalyzer.Run(ref context),
            unusedDependenciesAnalyzer.Run(ref context)
        };
        
        
        Task.WhenAll(analysisTasks).Wait();

        return context;
    }
}