using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.PreReleasePackageReference;

public partial class Analyzer
{
    public string Name => "PreReleasePackageReferenceAnalyzer";
    
    public string Description => "Analyzes project files to detect NuGet package references that use " +
                                 "pre-release versions. Pre-release packages may contain unfinished features, " +
                                 "breaking changes, or unstable code, which can introduce risks in production " +
                                 "environments. This analyzer helps identify such packages so teams can evaluate " +
                                 "whether to replace them with stable releases for improved reliability and support.";
    
    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Ok
    ];
    
    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project, 
        AnalysisResultType.PackageReference
    ];
}