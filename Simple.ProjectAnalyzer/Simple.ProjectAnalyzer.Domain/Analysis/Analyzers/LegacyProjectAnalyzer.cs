using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class LegacyProjectAnalyzer : IAnalyzer
{
    public string Description =>  "Analyzes projects to identify usage of legacy, non-SDK style project formats " +
                                  "and outdated target frameworks. Legacy projects often lack modern build features, " +
                                  "improved maintainability, and compatibility with current tooling. This analyzer " +
                                  "highlights projects that should consider migrating to the SDK-style format and " +
                                  "updating their target frameworks to supported .NET versions (>= .NET 5) to ensure " +
                                  "better maintainability, security, and support.";
    
    public IEnumerable<AnalysisResultCode> ResultCodes => [AnalysisResultCode.Warning, AnalysisResultCode.Ok];
    
    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(LegacyProjectAnalyzer)}.{nameof(Run)} started");
        
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
            .Where(p => p.TargetFrameworks.All(tf => tf.Version.Major is > 0 and < 5))
            .ToList()
            .ForEach(legacyProject => legacyProject.AnalysisResults.Add(new AnalysisResult
            {
                Source = nameof(LegacyProjectAnalyzer),
                Code = AnalysisResultCode.Warning,
                Parent = legacyProject,
                Title = "Legacy project",
                Message = "Project targets only legacy .NET versions (< .NET 5). Consider upgrading.",
                Details = "All target frameworks for this project are legacy .NET Framework/Core versions (older than .NET 5). " +
                          "None of them are targeting modern LTS platforms. Upgrade is strongly recommended.",
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