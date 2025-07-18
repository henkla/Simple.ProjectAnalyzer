using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.Domain.Analyzers;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this ServiceCollection services)
    {
        services.AddSingleton<AnalyzeHandler>();
        services.AddSingleton<OldFrameworkAnalyzer>();
        services.AddSingleton<OutdatedDependenciesAnalyzer>();
        services.AddSingleton<UnusedDependenciesAnalyzer>();
        services.AddSingleton<ProjectFinder>();
        services.AddSingleton<CurrentLtsService>();
        services.AddSingleton<ProjectParser>();
        services.AddSingleton<ExternalDllAnalyzer>();
        // services.AddSingleton<TargetFrameworkAnalyzer>();
        services.AddSingleton<ProjectTypeAnalyzer>();
        services.AddSingleton<ProjectParser>();
        services.AddSingleton<PreReleasePackageReferenceAnalyzer>();

        return services;
    }
}