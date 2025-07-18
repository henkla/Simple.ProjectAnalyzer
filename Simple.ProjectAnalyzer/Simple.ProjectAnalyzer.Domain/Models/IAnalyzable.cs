namespace Simple.ProjectAnalyzer.Domain.Models;

public interface IAnalyzable
{
    public List<AnalysisResult> AnalysisResults { get; }
    public string Name { get;  }
}