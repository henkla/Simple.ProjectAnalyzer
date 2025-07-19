using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class OutdatedFrameworkAnalyzer : AnalyzerBase
{
    public override Task Run(ref Context context)
    {
        var sdkProjects = context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major >= 5))
            .ToList();

        foreach (var sdkProject in sdkProjects)
        {
            var projectResult = CompareProjectVersionToCurrentLts(sdkProject, context.CurrentLtsVersion);
            sdkProject.AnalysisResults.Add(projectResult);
        }

        return Task.CompletedTask;
    }

    private static AnalysisResult CompareProjectVersionToCurrentLts(Project sdkProject, TargetFramework currentLtsVersion)
    {
        var currentLtsVersionAlias = currentLtsVersion.Alias;
        const string source = nameof(OutdatedFrameworkAnalyzer);
        var highestTargetFramework = GetHighestTargetFramework(sdkProject.TargetFrameworks);

        return highestTargetFramework.Version.CompareTo(currentLtsVersion.Version) switch
        {
            > 0 => new AnalysisResult // ahead of LTS
            {
                Source = source,
                Code = AnalysisResultCode.Ok,
                Parent = sdkProject,
                Title = "TargetFramework is ahead of LTS",
                Message = "The project targets a framework version that is newer than the current LTS.",
                Details = "Using a newer framework version may give you access to the latest features " +
                          "and improvements. However, be aware that non-LTS versions have shorter support " +
                          "windows and may not be suitable for long-term production use.",
                Recommendation = "Target upcoming LTS version once available."
            },
            < 0 => new AnalysisResult // behind LTS
            {
                Source = source,
                Code = AnalysisResultCode.Warning,
                Parent = sdkProject,
                Title = "Target Framework is behind LTS",
                Message = "The project targets a framework version that is older than the current LTS.",
                Details = "Using a framework older than the current Long-Term Support (LTS) version may " +
                          "expose your application to security vulnerabilities, performance issues, and " +
                          "compatibility problems. Consider upgrading to the current LTS version to ensure " +
                          "long-term support and stability.",
                Recommendation = $"Target current LTS version ({currentLtsVersionAlias})."
            },
            _ => new AnalysisResult // equal to current LTS
            {
                Source = source,
                Code = AnalysisResultCode.Ok,
                Parent = sdkProject,
                Title = "Target Framework is current LTS",
                Message = "The project targets the current LTS framework version.",
                Details = "Targeting the current Long-Term Support (LTS) version ensures maximum stability, " +
                          "broad ecosystem support, and continued security updates."
            }
        };
    }

    private static TargetFramework GetHighestTargetFramework(List<TargetFramework> targetFrameworks)
    {
        return targetFrameworks
            .OrderByDescending(tf => tf.Version)
            .First();
    }
}