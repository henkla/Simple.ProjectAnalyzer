namespace Simple.ProjectAnalyzer.Domain.Models;

public class AnalysisResult
{
    public required string Source { get; init; }
    public required AnalysisResultType Type { get; init; }
    public required AnalysisResultCode Code { get; init; }
    public required string Title { get; set; }
    public required string Message { get; init; }
    public required string Details { get; init; }
    public string? Recommendation { get; init; }
    public required IAnalyzable Parent { get; init; }
}