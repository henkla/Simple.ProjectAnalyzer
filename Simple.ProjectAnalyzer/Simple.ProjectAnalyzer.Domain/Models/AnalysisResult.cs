namespace Simple.ProjectAnalyzer.Domain.Models;

public class AnalysisResult
{
    public required string Message { get; set; }
    public ResultCode Code { get; set; }
    public required string Source { get; set; }
}