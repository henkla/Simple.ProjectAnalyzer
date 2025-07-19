namespace Simple.ProjectAnalyzer.Domain.Models;

public class Reference : IAnalyzable
{
    public required string Name { get; init; }
    public required string HintPath { get; init; }
    public bool Private { get; init; }
    public List<AnalysisResult> AnalysisResults { get; } = [];
}