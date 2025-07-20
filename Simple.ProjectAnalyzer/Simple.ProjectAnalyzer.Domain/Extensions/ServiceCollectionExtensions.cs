using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this ServiceCollection services)
    {
        services.AddServices();
        services.AddAnalyzers();

        return services;
    }

    private static IServiceCollection AddServices(this ServiceCollection services)
    {
        services.AddSingleton<Orchestrator>();
        services.AddSingleton<ProjectFinder>();
        services.AddSingleton<DotnetService>();
        services.AddSingleton<ProjectParser>();
        services.AddSingleton<ResultOutputHandler>();
        services.AddSingleton<GitService>();
        services.AddSingleton<GitCommandHandler>();
        services.AddSingleton<LocalCommandHandler>();

        return services;
    }

    private static IServiceCollection AddAnalyzers(this ServiceCollection services)
    {
        services.AddSingleton<LegacyProjectAnalyzer>();
        services.AddSingleton<OutdatedFrameworkAnalyzer>();
        services.AddSingleton<ExternalDllAnalyzer>();
        services.AddSingleton<PreReleasePackageReferenceAnalyzer>();
        services.AddSingleton<DuplicatePackageReferenceAnalyzer>();
        services.AddSingleton<OutCommentedCodeAnalyzer>();
        services.AddSingleton<NullableNotEnabledAnalyzer>();

        return services;
    }
}