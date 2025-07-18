namespace Simple.ProjectAnalyzer.Domain.Models;

public class AnalysisResult
{
    public required string Source { get; init; }
    public required ResultCode Code { get; init; }
    public required string Message { get; init; }
    public string? Details { get; init; }

    public required IAnalyzable Parent { get; init; } 
}