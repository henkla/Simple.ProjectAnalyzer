using Spectre.Console;

namespace Simple.ProjectAnalyzer.Abstractions.CommandLine;

public interface IConsoleOutput
{
    public void EnableVerboseOutput();

    public void Write(string line);

    public void Line(string? line = null);

    public void Verbose(string line);

    public void Warning(string line);

    public void Error(string line);

    public void Exception(Exception exception, ExceptionFormats? format = null);

    public void Figlet(string text, Color? color = null);
}