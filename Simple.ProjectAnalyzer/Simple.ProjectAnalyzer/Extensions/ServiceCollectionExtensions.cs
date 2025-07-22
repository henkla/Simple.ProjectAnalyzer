using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.CommandLine;
using Simple.ProjectAnalyzer.CommandLine.Commands.Analyze;
using Simple.ProjectAnalyzer.CommandLine.Commands.List;

namespace Simple.ProjectAnalyzer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsoleOutput(this IServiceCollection services)
    {
        services.AddSingleton<IConsoleOutput, Output>();
        
        return services;
    }    
    
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddSingleton<ListCommandHandler>();
        services.AddSingleton<AnalyzeCommandHandler>();
        
        return services;
    }
}
