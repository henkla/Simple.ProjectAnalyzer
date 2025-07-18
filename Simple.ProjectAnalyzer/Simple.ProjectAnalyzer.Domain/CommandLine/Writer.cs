using Spectre.Console;

namespace Simple.ProjectAnalyzer.Domain.CommandLine;

public static class Writer
{    
    public static void Write(string line)
    {
        AnsiConsole.Write(line);
    }
    
    public static void WriteLine(string line)
    {
        AnsiConsole.WriteLine(line);
    }

    public static void WriteVerbose(string line)
    {
        AnsiConsole.MarkupLine($"[bold][gray]VERBOSE:[/][/] [gray]{line}[/]");
    }
    
    public static void WriteException(Exception exception, ExceptionFormats format)
    {
        AnsiConsole.WriteException(exception, format);
    }
}