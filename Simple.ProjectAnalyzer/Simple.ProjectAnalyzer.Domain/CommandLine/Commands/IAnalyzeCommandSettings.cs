namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public interface IAnalyzeCommandSettings
{
    public bool Verbose { get; set; }
    public string Path { get; set; }
    public string[]? Analyzers { get; set; }
}