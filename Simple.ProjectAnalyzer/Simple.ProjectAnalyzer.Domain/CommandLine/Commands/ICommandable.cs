namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public interface ICommandable
{
    public bool Verbose { get; set; }
    public string Path { get; set; }
}