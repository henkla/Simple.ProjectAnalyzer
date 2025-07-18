using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public abstract class AnalyzerBase
{
    public abstract Task Run(ref AnalysisContext context);
}