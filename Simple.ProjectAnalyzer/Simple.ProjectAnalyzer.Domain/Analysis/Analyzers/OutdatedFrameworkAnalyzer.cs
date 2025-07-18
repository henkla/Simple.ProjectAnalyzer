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
            var projectResult = CompareProjectVersionToCurrentLts(sdkProject, context.CurrentLtsVersion.Version);
            sdkProject.AnalysisResults.Add(projectResult);
        }

        return Task.CompletedTask;
    }

    private static AnalysisResult CompareProjectVersionToCurrentLts(Project sdkProject, Version ltsVersion)
    {
        const string source = nameof(OutdatedFrameworkAnalyzer);
        var highestTargetFramework = GetHighestTargetFramework(sdkProject.TargetFrameworks);

        return highestTargetFramework.Version.CompareTo(ltsVersion) switch
        {
            // ahead of LTS
            > 0 => new AnalysisResult
            {
                Source = source,
                Code = ResultCode.Ok,
                Parent = sdkProject,
                Message = "The project targets a framework version that is newer than the current LTS.",
                Details = "Using a newer framework version may give you access to the latest features " +
                          "and improvements. However, be aware that non-LTS versions have shorter support " +
                          "windows and may not be suitable for long-term production use."
            },
            // behind LTS
            < 0 => new AnalysisResult
            {
                Source = source,
                Code = ResultCode.Warning,
                Parent = sdkProject,
                Message = "The project targets a framework version that is older than the current LTS.",
                Details = "Using a framework older than the current Long-Term Support (LTS) version may " +
                          "expose your application to security vulnerabilities, performance issues, and " +
                          "compatibility problems. Consider upgrading to the current LTS version to ensure " +
                          "long-term support and stability."
            },
            // equal to LTS
            _ => new AnalysisResult
            {
                Source = source,
                Code = ResultCode.Ok,
                Parent = sdkProject,
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