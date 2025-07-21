namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public interface IAnalyzer : IAnalyzerMeta
{
    Task Run(Context context);
}