using Simple.ProjectAnalyzer.Abstractions.CommandLine;
using Spectre.Console;

namespace Simple.ProjectAnalyzer.CommandLine;

public class Output : IConsoleOutput
{
    private static bool? _verboseEnabled;

    public void EnableVerboseOutput()
    {
        if (_verboseEnabled.HasValue)
        {
            throw new InvalidOperationException("Output verbosity mode can only be set once.");
        }
        
        _verboseEnabled = true;
        Verbose("Verbose output has been enabled.");
    }
    
    public void Write(string line)
    {
        AnsiConsole.Write(line);
    }

    public void Line(string? line = null)
    {
        AnsiConsole.WriteLine(line ?? string.Empty);
    }

    public void Verbose(string line)
    {
        if (_verboseEnabled is true)
        {
            AnsiConsole.MarkupLine($"[bold][gray]VERBOSE:[/][/] [gray]{line}[/]");    
        }
    }
    
    public void Warning(string line)
    {
        AnsiConsole.MarkupLine($"[bold][yellow]WARNING:[/][/] [white]{line}[/]");
    }
    
    public void Error(string line)
    {
        AnsiConsole.MarkupLine($"[bold][red]ERROR:[/][/] [white]{line}[/]");
    }
    
    public void Exception(Exception exception, ExceptionFormats? format = null)
    {
        if (format is null)
        {
            AnsiConsole.WriteException(exception);
            return;
        }
        
        AnsiConsole.WriteException(exception, format.Value);
    }
    
    public void Figlet(string text, Color? color = null)
    {
        AnsiConsole.Write(new FigletText(text)
            .LeftJustified()
            .Color(color ?? Color.Default));
    }
}