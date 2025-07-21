using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public interface IAnalyzerMeta
{
    string Name { get; } 
    string Description { get; }
    IEnumerable<AnalysisResultCode> Codes { get; }
    IEnumerable<AnalysisResultType> Targets { get; }
}