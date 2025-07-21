using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class PreReleasePackageReferenceAnalyzer : IAnalyzer
{
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
    
    private static readonly Regex PreReleaseRegex = new(@"-\w+", RegexOptions.Compiled);

    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(PreReleasePackageReferenceAnalyzer)}.{nameof(Run)} started");
        
        foreach (var project in context.Projects)
        {
            if (project.PackageReferences.Count == 0)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Title = "No package references found",
                    Message = "The project does not reference any NuGet packages.",
                    Details = "This project currently has no package dependencies declared. While " +
                              "this can simplify dependency management, it may limit access to external " +
                              "libraries, updates, and bug fixes. If your project does not require external " +
                              "packages, this is perfectly fine. Otherwise, consider adding necessary " +
                              "package references to leverage shared functionality, security updates, " +
                              "and ongoing improvements from third-party libraries.",
                    Code = AnalysisResultCode.Ok,
                    Type = AnalysisResultType.Project,
                    Parent = project,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });

                continue;
            }
            
            var preReleasePackages = project.PackageReferences
                .Where(package => IsPreRelease(package.Version))
                .ToList();
            
            if (preReleasePackages.Count != 0)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Title = "Pre-release packages",
                    Message = "Project includes one or more pre-release packages.",
                    Details = $"Pre-release packages are intended for testing and early feedback. They may include unfinished " +
                              $"features, breaking changes, or bugs. Avoid depending on them in production unless you fully understand " +
                              $"the risks and accept the maintenance overhead.",
                    Recommendation = "Use stable versions if possible.",
                    Code = AnalysisResultCode.Hint,
                    Type = AnalysisResultType.Project,
                    Parent = project,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Title = "Stable packages",
                    Message = "Stable package version is in use, which provides a reliable and tested foundation for your application.",
                    Details = "Stable package releases have undergone thorough testing and quality assurance. " +
                              "They offer greater reliability and backward compatibility, reducing the risk of unexpected issues. " +
                              "Using stable versions in production environments helps ensure security, performance, and maintainability. " +
                              "Keep dependencies updated to benefit from the latest fixes and improvements.",
                    Code = AnalysisResultCode.Ok,
                    Type = AnalysisResultType.Project,
                    Parent = project,
                    Source = nameof(PreReleasePackageReferenceAnalyzer)
                });
            }

            foreach (var preReleasePackage in preReleasePackages)
            {
                preReleasePackage.AnalysisResults.Add(new AnalysisResult
                {
                    Title = "Pre-release package",
                    Message = "This package is using a pre-release version.",
                    Details = "This package is a pre-release, which may include experimental features, unexpected " +
                              "changes, and potential instability in production environments. It is recommended " +
                              "to switch to a stable release for improved reliability, compatibility, and long-term " +
                              "support.",
                    Code = AnalysisResultCode.Hint,
                    Type = AnalysisResultType.PackageReference,
                    Parent = preReleasePackage,
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