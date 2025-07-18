using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.Utilities;

public static class ExceptionHandler
{
    public static int OnException(Exception exception, ITypeResolver? resolver)
    {
        var settings = ResolveAnalyzeSettings(resolver);

        if (settings is not null && settings.Verbose)
        {
            AnsiConsole.WriteException(exception, ExceptionFormats.ShowLinks);
        }
        else
        {
            AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
        }

        return GetExitCode(exception);
    }

    private static AnalyzeCommandSettings? ResolveAnalyzeSettings(ITypeResolver? resolver)
    {
        try
        {
            var settings = resolver?.Resolve(typeof(AnalyzeCommandSettings));
            return settings as AnalyzeCommandSettings;
        }
        catch (Exception exception)
        {
            AnsiConsole.WriteException(exception);
            return null;
        }
    }
    
    private static int GetExitCode(Exception exception)
    {
        // todo: find out which error code to return based on exception
        return (int)ExitCode.Exception;
    }
}