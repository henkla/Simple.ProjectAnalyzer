using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class LegacyProjectAnalyzer : AnalyzerBase
{
    public override Task Run(ref Context context)
    {
        var currentLtsVersion = context.CurrentLtsVersion.Alias;
        
        context.Projects
            .Where(p => p is { IsLegacy: true, Sdk: null })
            .ToList()
            .ForEach(legacySdkProject => legacySdkProject.AnalysisResults.Add(new AnalysisResult
            {
                Source = nameof(LegacyProjectAnalyzer),
                Code = AnalysisResultCode.Warning,
                Parent = legacySdkProject,
                Title = "Legacy non-SDK style",
                Message = "Project is a legacy non-SDK style project.",
                Details = "This project does not use the newer SDK-style project format introduced with .NET Core and .NET 5. " +
                          "Legacy project format may be harder to maintain and lacks many modern build improvements. " +
                          "Consider migrating to SDK-style project format for better tooling, compatibility, and maintainability.",
                Recommendation = "Consider migrating to SDK-style project."
            }));

        context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major is > 0 and < 5))
            .ToList()
            .ForEach(legacyProject => legacyProject.AnalysisResults.Add(new AnalysisResult
            {
                Source = nameof(LegacyProjectAnalyzer),
                Code = AnalysisResultCode.Warning,
                Parent = legacyProject,
                Title = "Legacy project",
                Message = "Project targets a legacy .NET version (< .NET 5). Consider upgrading.",
                Details = "This project is using a legacy .NET framework or .NET Core version (older than .NET 5), " +
                          "which no longer receives active feature development or long-term support " +
                          "from Microsoft. Upgrading to .NET 6 or later (preferably a current LTS " +
                          "version) is recommended to ensure security, performance, compatibility " +
                          "with modern libraries, and access to the latest language features.",
                Recommendation = $"Consider targeting current LTS version ({currentLtsVersion})."
            }));

        context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major >= 5))
            .ToList()
            .ForEach(project => project.AnalysisResults.Add(new AnalysisResult
            {
                Source = nameof(LegacyProjectAnalyzer),
                Code = AnalysisResultCode.Ok,
                Parent = project,
                Title = "Modern SDK style",
                Message = "Project targets a modern .NET version (>= .NET 5).",
                Details = "The project uses a supported .NET version, which benefits from active development, " +
                          "security updates, and compatibility with modern libraries and tools. " +
                          "It is recommended to continue monitoring new Long-Term Support (LTS) releases and plan upgrades " +
                          "to maintain optimal performance, security, and support."
            }));


        return Task.CompletedTask;
    }
}