namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public interface ICommand
{
    public static abstract string Name { get; }
    public static abstract string Description { get; }
}