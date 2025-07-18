using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class PreReleasePackageReferenceAnalyzer : AnalyzerBase
{
    private static readonly Regex PreReleaseRegex = new(@"-\w+", RegexOptions.Compiled);

    public override Task Run(ref AnalysisContext context)
    {
        foreach (var project in context.Projects)
        {
            foreach (var package in project.PackageReferences.Where(package => IsPreRelease(package.Version)))
            {
                var packageMessage = $"Consider using a stable version instead of the pre-release " +
                                     $"'{package.Version}' of package '{package.Name}' to ensure " +
                                     $"better reliability in production.";

                package.AnalysisResults.Add(new AnalysisResult
                {
                    Message = packageMessage,
                    Code = ResultCode.Suggestion,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });

                var projectMessage = $"Package '{package.Name}' in project '{project.Name}' is using pre-release " +
                                     $"version '{package.Version}'. Pre-release versions are intended for testing and " +
                                     $"evaluation purposes only. They may introduce breaking changes, contain " +
                                     $"incomplete features, or behave unexpectedly. Avoid using them in production " +
                                     $"unless absolutely necessary.";

                project.AnalysisResults.Add(new AnalysisResult
                {
                    Message = projectMessage,
                    Code = ResultCode.Suggestion,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsPreRelease(string version)
    {
        return PreReleaseRegex.IsMatch(version);
    }
}