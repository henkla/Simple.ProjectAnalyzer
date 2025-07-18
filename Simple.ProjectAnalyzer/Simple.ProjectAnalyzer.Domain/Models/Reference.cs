namespace Simple.ProjectAnalyzer.Domain.Models;

public class Reference
{
    public string Type { get; init; }
    public string HintPath { get; init; }
    public bool Private { get; init; }
}