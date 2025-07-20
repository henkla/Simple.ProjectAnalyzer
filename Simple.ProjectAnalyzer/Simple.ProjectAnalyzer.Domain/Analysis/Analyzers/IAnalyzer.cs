namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public interface IAnalyzer
{
    Task Run(Context context);
}