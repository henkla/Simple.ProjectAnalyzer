using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.OutdatedFramework;

public partial class Analyzer
{
    public string Name => "OutdatedFrameworkAnalyzer";
    
    public string Description => "Analyzes project files to detect whether target frameworks are outdated " +
                                 "in relation to the current .NET Long-Term Support (LTS) version. Projects " +
                                 "targeting frameworks older than the current LTS may miss out on critical " +
                                 "security updates, performance improvements, and modern features. This analyzer " +
                                 "highlights such projects and recommends upgrading to the current LTS to ensure " +
                                 "long-term stability and support.";
    
    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Warning, 
        AnalysisResultCode.Ok
    ];
    
    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project
    ];
}