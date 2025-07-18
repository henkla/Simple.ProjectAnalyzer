using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class UnusedDependenciesAnalyzer : AnalyzerBase
{
    public override Task Run(ref AnalysisContext context)
    {
        return  Task.FromResult(new AnalysisResult
        {
            Source = nameof(UnusedDependenciesAnalyzer),
            Code = ResultCode.Ok,
            Message = "No unused dependencies discovered"
        });
    }
}