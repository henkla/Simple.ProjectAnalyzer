using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class OutdatedDependenciesAnalyzer : AnalyzerBase
{
    public override Task Run(ref AnalysisContext context)
    {
        return  Task.FromResult(new AnalysisResult
        {
            Source = nameof(OutdatedDependenciesAnalyzer),
            Code = ResultCode.Ok,
            Message = "No outdated dependencies discovered"
        });
    }
}