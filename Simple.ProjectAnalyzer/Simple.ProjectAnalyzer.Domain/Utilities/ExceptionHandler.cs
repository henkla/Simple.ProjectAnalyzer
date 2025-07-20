using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;
using Simple.ProjectAnalyzer.Domain.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.Utilities;

public static class ExceptionHandler
{
    public static int OnException(Exception exception, ITypeResolver? resolver)
    {
        Output.Verbose($"{nameof(ExceptionHandler)}.{nameof(OnException)} started");
        
        var settings = ResolveCommandSettings(resolver);
        if (settings is not null && settings.Verbose)
        {
            Output.Exception(exception, ExceptionFormats.ShowLinks);
        }
        else
        {
            Output.Exception(exception, ExceptionFormats.ShortenEverything);
        }

        return GetExitCode(exception);
    }

    private static ICommandSettings? ResolveCommandSettings(ITypeResolver? resolver)
    {
        Output.Verbose("Trying to resolve command settings");
        
        try
        {
            var settings = resolver?.Resolve(typeof(LocalCommandSettings)) 
                           ?? resolver?.Resolve(typeof(GitCommandSettings));
            
            return settings as ICommandSettings;
        }
        catch (Exception exception)
        {
            Output.Exception(exception);
            return null;
        }
    }
    
    private static int GetExitCode(Exception exception)
    {
        // todo: find out which error code to return based on exception
        return (int)ApplicationExitCode.Exception;
    }
}