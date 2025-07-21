using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.DuplicatePackageReference;

public partial class Analyzer
{
    public string Name => "DuplicatePackageReferenceAnalyzer";

    public string Description => "Analyzes project files to detect duplicate NuGet package references. " +
                                 "Duplicate references, especially with conflicting versions, can cause build warnings, " +
                                 "dependency resolution issues, and increased maintenance complexity. " +
                                 "This analyzer helps ensure that each package is referenced only once per project " +
                                 "to maintain clean and consistent dependency management.";

    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Warning, 
        AnalysisResultCode.Ok
    ];
    
    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project, 
        AnalysisResultType.PackageReference
    ];

    
}