using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.LegacyProject;

public partial class Analyzer
{
    public string Name => "LegacyProjectAnalyzer";

    public string Description =>  "Analyzes projects to identify usage of legacy, non-SDK style project formats " +
                                  "and outdated target frameworks. Legacy projects often lack modern build features, " +
                                  "improved maintainability, and compatibility with current tooling. This analyzer " +
                                  "highlights projects that should consider migrating to the SDK-style format and " +
                                  "updating their target frameworks to supported .NET versions (>= .NET 5) to ensure " +
                                  "better maintainability, security, and support.";

    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Warning, 
        AnalysisResultCode.Ok
    ];
    
    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project, 
    ];
}