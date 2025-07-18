namespace Simple.ProjectAnalyzer.Domain.Models;

public class TargetFramework
{
    public string Alias { get; init; }
    public Version Version { get; init; }
    public bool IsLts => (Version.Major % 2 == 0);
}