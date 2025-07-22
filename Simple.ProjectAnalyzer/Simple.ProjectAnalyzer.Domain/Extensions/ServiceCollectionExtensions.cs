using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddServices();
        services.AddAnalyzers();

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

        return services;
    }
    
    private static IServiceCollection AddAnalyzers(this IServiceCollection services)
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
                Console.WriteLine("An error occured during registration of analyzer"); // todo: inte console.wl här
                continue;
            }
            
            services.AddSingleton(interfaceType, typeImplementation);
        }
        
        return services;
    }
}