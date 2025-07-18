namespace Simple.ProjectAnalyzer.Domain.Models;

public class PackageReference : IAnalyzable
{
    public required string Name { get; init; }
    public required string Version { get; init; }
    public List<AnalysisResult> AnalysisResults { get; } = [];
    public string? Include { get; init; }
}