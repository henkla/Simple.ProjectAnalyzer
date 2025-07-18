namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public abstract class AnalyzerBase
{
    public abstract Task Run(ref Context context);
}