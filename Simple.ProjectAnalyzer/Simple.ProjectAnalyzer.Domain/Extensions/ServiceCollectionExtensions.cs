using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Analyzers;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection services)
    {
        services.AddServices();
        services.AddAnalyzersExperimental();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<Orchestrator>();
        services.AddSingleton<ProjectFinder>();
        services.AddSingleton<DotnetService>();
        services.AddSingleton<ProjectParser>();
        services.AddSingleton<OutputHandler>();
        services.AddSingleton<GitService>();
        services.AddSingleton<GitCommandHandler>();
        services.AddSingleton<LocalCommandHandler>();
        services.AddSingleton<AnalyzersCommandHandler>();
        

        return services;
    }
    
    private static IServiceCollection AddAnalyzersExperimental(this IServiceCollection services)
    {
        var interfaceType = typeof(IAnalyzer);
        var typeImplementations = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t is not null)!;
                }
            })
            .Where(t => t is { IsAbstract: false, IsInterface: false } && interfaceType.IsAssignableFrom(t));


        foreach (var typeImplementation in typeImplementations)
        {
            if (typeImplementation is null)
            {
                Output.Error("An error occured during registration of analyzer");
                continue;
            }
            
            services.AddSingleton(interfaceType, typeImplementation);
        }
        
        return services;
    }
}