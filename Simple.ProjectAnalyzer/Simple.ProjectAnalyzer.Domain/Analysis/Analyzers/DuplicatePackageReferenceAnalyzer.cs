using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class DuplicatePackageReferenceAnalyzer : IAnalyzer
{
    public string Description => "Analyzes project files to detect duplicate NuGet package references. " +
                                 "Duplicate references, especially with conflicting versions, can cause build warnings, " +
                                 "dependency resolution issues, and increased maintenance complexity. " +
                                 "This analyzer helps ensure that each package is referenced only once per project " +
                                 "to maintain clean and consistent dependency management.";

    public IEnumerable<AnalysisResultCode> ResultCodes => [AnalysisResultCode.Warning, AnalysisResultCode.Ok];

    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(DuplicatePackageReferenceAnalyzer)}.{nameof(Run)} started");
        
        foreach (var project in context.Projects)
        {
            var duplicates = project.PackageReferences
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in duplicates)
            {
                var versions = string.Join(", ", group.Select(p => p.Version).Distinct());
                group.ToList().ForEach(reference => reference.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Warning,
                    Title = "Duplicate package reference",
                    Message = $"Duplicate package reference {group.Key} with versions: {versions}",
                    Details = "The project file contains multiple references to the same NuGet package, " +
                              "either with identical or conflicting versions. This redundancy can lead to " +
                              "confusion, unnecessary build warnings, and potential versioning conflicts " +
                              "during dependency resolution.",
                    Parent = project,
                    Source = nameof(DuplicatePackageReferenceAnalyzer),
                    Recommendation = "Remove duplicate references to the same NuGet package in the project " +
                                     "file. Ensure each package is only referenced once, and that the version " +
                                     "specified matches the intended version for the project."
                }));
            }

            if (duplicates.Count == 0)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = nameof(DuplicatePackageReferenceAnalyzer),
                    Parent = project,
                    Code = AnalysisResultCode.Ok,
                    Title = "Duplicate package reference",
                    Message = "No duplicate package references found",
                    Details = "The project file has been scanned for multiple references to the " +
                              "same NuGet package. No duplicate references were detected, which " +
                              "means the project's dependencies are well-structured in this regard " +
                              "and avoid unnecessary redundancy or potential versioning conflicts."
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Warning,
                    Parent = project,
                    Source = nameof(DuplicatePackageReferenceAnalyzer),
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
                });
            }
        }

        return Task.CompletedTask;
    }
}