using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.ImplicitUsings;

public partial class Analyzer
{
    public string Name => "ImplicitUsingsAnalyzer";

    public string Description => "Analyzes project files to determine whether <ImplicitUsings> is explicitly defined. " +
                                 "Implicit usings provide a set of default using directives depending on the target framework, " +
                                 "which can simplify code but may also introduce hidden dependencies. " +
                                 "This analyzer helps ensure that the implicit using behavior is intentional and clearly declared " +
                                 "in the project file, improving project clarity and reducing ambiguity.";

    public IEnumerable<AnalysisResultCode> Codes =>
    [
        AnalysisResultCode.Ok,
        AnalysisResultCode.Hint,
        AnalysisResultCode.Warning
    ];

    public IEnumerable<AnalysisResultType> Targets =>
    [
        AnalysisResultType.Project
    ];
}