using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public interface IAnalyzer
{
    string Description { get; }
    IEnumerable<AnalysisResultCode> ResultCodes { get; }
    Task Run(Context context);
}