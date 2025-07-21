using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.DuplicatePackageReference;

public partial class Analyzer : IAnalyzer
{
    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(DuplicatePackageReference.Analyzer)}.{nameof(Run)} started");
        
        foreach (var project in context.Projects)
        {
            var duplicates = project.PackageReferences
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in duplicates)
            {
                var versions = string.Join(", ", group.Select(p => p.Version).Distinct());
                group.ToList().ForEach(reference => 
                    reference.AnalysisResults
                    .Add(GetWarningForReferenceOnDuplicatesFound(reference, group.Key, versions)));
            }

            project.AnalysisResults.Add(duplicates.Count == 0 
                ? GetOkForProjectNoDuplicatesFound(project) 
                : GetWarningForProjectOnDuplicatesFound(project));
        }

        return Task.CompletedTask;
    }
    
    private AnalysisResult GetWarningForReferenceOnDuplicatesFound(IAnalyzable parent, string package, string version) => new()
    {
        Code = AnalysisResultCode.Warning,
        Type = AnalysisResultType.Project,
        Title = "Duplicate package reference",
        Message = $"Duplicate package reference {package} with versions: {version}",
        Details = "The project file contains multiple references to the same NuGet package, " +
                  "either with identical or conflicting versions. This redundancy can lead to " +
                  "confusion, unnecessary build warnings, and potential versioning conflicts " +
                  "during dependency resolution.",
        Parent = parent,
        Source = Name,
        Recommendation = "Remove duplicate references to the same NuGet package in the project " +
                         "file. Ensure each package is only referenced once, and that the version " +
                         "specified matches the intended version for the project."
    };

    private AnalysisResult GetOkForProjectNoDuplicatesFound(IAnalyzable parent) => new()
    {
        Source = Name,
        Type = AnalysisResultType.Project,
        Parent = parent,
        Code = AnalysisResultCode.Ok,
        Title = "Duplicate package reference",
        Message = "No duplicate package references found",
        Details = "The project file has been scanned for multiple references to the " +
                  "same NuGet package. No duplicate references were detected, which " +
                  "means the project's dependencies are well-structured in this regard " +
                  "and avoid unnecessary redundancy or potential versioning conflicts."
    };

    private AnalysisResult GetWarningForProjectOnDuplicatesFound(IAnalyzable parent) => new AnalysisResult
    {
        Code = AnalysisResultCode.Warning,
        Parent = parent,
        Type = AnalysisResultType.Project,
        Source = Name,
        Title = "Duplicate package reference",
        Message = "Duplicate package reference(s) found",
        Details = "The project file contains multiple references to the same NuGet package, " +
                  "either with identical or conflicting versions. This redundancy can lead to " +
                  "confusion, unnecessary build warnings, and potential versioning conflicts " +
                  "during dependency resolution.",
        Recommendation = "Remove duplicate references to the same NuGet package in the " +
                         "project file. Ensure each package is only referenced once, and " +
                         "that the version specified matches the intended version for the " +
                         "project."
    };
}