using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.CommandLine;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Orchestrator(
    PreReleasePackageReferenceAnalyzer preReleasePackageReferenceAnalyzer,
    LegacyProjectAnalyzer legacyProjectAnalyzer,
    OutdatedFrameworkAnalyzer outdatedFrameworkAnalyzer,
    DuplicatePackageReferenceAnalyzer duplicatePackageReferenceAnalyzer,
    OutCommentedCodeAnalyzer outCommentedCodeAnalyzer,
    NullableNotEnabledAnalyzer nullableNotEnabledAnalyzer,
    ExternalDllAnalyzer  externalDllAnalyzer
)
{
    public async Task AnalyzeProjects(Context context)
    {
        Output.Verbose($"{nameof(Orchestrator)}.{nameof(AnalyzeProjects)} started");
        
        var analysisTasks = new List<Task>
        {
            legacyProjectAnalyzer.Run(context),
            outdatedFrameworkAnalyzer.Run(context),
            outCommentedCodeAnalyzer.Run(context),
            externalDllAnalyzer.Run(context),
            preReleasePackageReferenceAnalyzer.Run(context),
            duplicatePackageReferenceAnalyzer.Run(context),
            nullableNotEnabledAnalyzer.Run(context)
        };

        await Task.WhenAll(analysisTasks);
    }
}