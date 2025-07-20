namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public interface ICommandSettings
{
    public bool Verbose { get; set; }
    public string Path { get; set; }
}