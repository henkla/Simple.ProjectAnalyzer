namespace Simple.ProjectAnalyzer.Domain.Models;

public class AnalysisResult
{
    public required string Source { get; set; }
    public required AnalysisResultType Type { get; set; }
    public required AnalysisResultCode Code { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public required string Details { get; set; }
    public string? Recommendation { get; set; }
    public required IAnalyzable Parent { get; set; }
}

