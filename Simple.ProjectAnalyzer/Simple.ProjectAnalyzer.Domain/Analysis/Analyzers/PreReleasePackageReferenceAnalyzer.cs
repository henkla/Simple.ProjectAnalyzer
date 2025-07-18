using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class PreReleasePackageReferenceAnalyzer : AnalyzerBase
{
    private static readonly Regex PreReleaseRegex = new(@"-\w+", RegexOptions.Compiled);

    public override Task Run(ref Context context)
    {
        foreach (var project in context.Projects)
        {
            foreach (var package in project.PackageReferences.Where(package => IsPreRelease(package.Version)))
            {
                var packageMessage = $"Package '{package.Name}' uses pre-release version '{package.Version}'.";
                var packageDetails = $"Consider using a stable release instead of the pre-release version '{package.Version}' " +
                                     $"of package '{package.Name}' to ensure better reliability, compatibility, and long-term support. " +
                                     $"Pre-release packages may contain experimental features, are more likely to change, and can cause " +
                                     $"instabilities when used in production environments.";

                package.AnalysisResults.Add(new AnalysisResult
                {
                    Message = packageMessage,
                    Details = packageDetails,
                    Code = ResultCode.Suggestion,
                    Parent = package,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });

                var projectMessage = $"Project '{project.Name}' includes a pre-release package: '{package.Name}' version '{package.Version}'.";
                var projectDetails = $"Pre-release packages are intended for testing and early feedback. They may include unfinished " +
                                     $"features, breaking changes, or bugs. Avoid depending on them in production unless you fully understand " +
                                     $"the risks and accept the maintenance overhead.";

                project.AnalysisResults.Add(new AnalysisResult
                {
                    Message = projectMessage,
                    Details = projectDetails,
                    Code = ResultCode.Suggestion,
                    Parent = project,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });
            }

            var projectOkMessage = "Stable package version is in use, which provides a reliable and tested foundation for your application.";
            var projectOkDetails = "Stable package releases have undergone thorough testing and quality assurance. " +
                                   "They offer greater reliability and backward compatibility, reducing the risk of unexpected issues. " +
                                   "Using stable versions in production environments helps ensure security, performance, and maintainability. " +
                                   "Keep dependencies updated to benefit from the latest fixes and improvements.";

            project.AnalysisResults.Add(new AnalysisResult
            {
                Message = projectOkMessage,
                Details = projectOkDetails,
                Code = ResultCode.Ok,
                Parent = project,
                Source = nameof(PreReleasePackageReferenceAnalyzer)
            });

        }

        return Task.CompletedTask;
    }

    private static bool IsPreRelease(string version)
    {
        return PreReleaseRegex.IsMatch(version);
    }
}