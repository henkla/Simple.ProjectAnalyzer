using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.TestProjectNaming;

public partial class Analyzer
{
    public string Name => "TestProjectNamingAnalyzer";
    
    public string Description =>
        "Ensures that test projects follow a consistent naming convention, typically ending with '.Tests'. " +
        "Consistent naming helps improve discoverability, test filtering, and overall project structure clarity.";

    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Ok
    ];

    public IEnumerable<AnalysisResultType> Targets =>
    [
        AnalysisResultType.Project
    ];
}