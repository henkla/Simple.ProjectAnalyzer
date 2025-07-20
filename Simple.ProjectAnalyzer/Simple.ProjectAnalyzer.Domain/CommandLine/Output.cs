using Spectre.Console;

namespace Simple.ProjectAnalyzer.Domain.CommandLine;

public static class Output
{
    private static bool? _verboseEnabled;

    public static void EnableVerboseOutput()
    {
        if (_verboseEnabled.HasValue)
        {
            throw new InvalidOperationException("Output verbosity mode can only be set once.");
        }
        
        _verboseEnabled = true;
        Verbose("Verbose output has been enabled.");
    }
    
    public static void Write(string line)
    {
        AnsiConsole.Write(line);
    }

    public static void Line(string? line = null)
    {
        AnsiConsole.WriteLine(line ?? string.Empty);
    }

    public static void Verbose(string line)
    {
        if (_verboseEnabled is true)
        {
            AnsiConsole.MarkupLine($"[bold][gray]VERBOSE:[/][/] [gray]{line}[/]");    
        }
    }
    
    public static void Warning(string line)
    {
        AnsiConsole.MarkupLine($"[bold][yellow]WARNING:[/][/] [white]{line}[/]");
    }
    
    public static void Error(string line)
    {
        AnsiConsole.MarkupLine($"[bold][red]ERROR:[/][/] [white]{line}[/]");
    }
    
    public static void Exception(Exception exception, ExceptionFormats? format = null)
    {
        if (format is null)
        {
            AnsiConsole.WriteException(exception);
            return;
        }
        
        AnsiConsole.WriteException(exception, format.Value);
    }

    
}