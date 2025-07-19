using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Orchestrator(
    PreReleasePackageReferenceAnalyzer  preReleasePackageReferenceAnalyzer,
    CurrentLtsService currentLtsService,
    LegacyProjectAnalyzer legacyProjectAnalyzer,
    OutdatedFrameworkAnalyzer outdatedFrameworkAnalyzer,
    ExternalDllAnalyzer  externalDllAnalyzer
)
{
    public async Task<Context> AnalyzeProjects(List<Project> projectsToAnalyze, ICommandable commandSettings)
    {
        // in order to perform analysis on project files, we need to know 
        // which version of dotnet that is current LTS
        var currentLtsVersion = await currentLtsService.GetCurrentLtsVersion();

        var context = new Context
        {
            Projects = projectsToAnalyze,
            CommandSettings = commandSettings,
            CurrentLtsVersion = currentLtsVersion
        };
            
        var analysisTasks = new List<Task>()
        {
            legacyProjectAnalyzer.Run(ref context),
            outdatedFrameworkAnalyzer.Run(ref context),
            externalDllAnalyzer.Run(ref context),
            preReleasePackageReferenceAnalyzer.Run(ref context)
        };
        
        Task.WhenAll(analysisTasks).Wait();

        return context;
    }
}