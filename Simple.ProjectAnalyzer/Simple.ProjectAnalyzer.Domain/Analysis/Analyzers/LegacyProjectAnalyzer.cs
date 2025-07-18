using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class LegacyProjectAnalyzer : AnalyzerBase
{
    public override Task Run(ref Context context)
    {
        context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major is > 0 and < 5))
            .ToList()
            .ForEach(legacyProject => legacyProject.AnalysisResults.Add(new AnalysisResult
            {
                Source = nameof(LegacyProjectAnalyzer),
                Code = ResultCode.Warning,
                Parent = legacyProject,
                Message = "Project targets a legacy .NET version (< .NET 5). Consider upgrading.",
                Details = "This project is using a legacy .NET framework version (older than .NET 5), " +
                          "which no longer receives active feature development or long-term support " +
                          "from Microsoft. Upgrading to .NET 6 or later (preferably a current LTS " +
                          "version) is recommended to ensure security, performance, compatibility " +
                          "with modern libraries, and access to the latest language features."
            }));
        
        context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major >= 5))
            .ToList()
            .ForEach(project => project.AnalysisResults.Add(new AnalysisResult
            {
                Source = nameof(LegacyProjectAnalyzer),
                Code = ResultCode.Ok,
                Parent = project,
                Message = $"Project '{project.Name}' targets a modern .NET version (>= .NET 5).",
                Details = "This project uses a supported .NET version, which benefits from active development, " +
                          "security updates, and compatibility with modern libraries and tools. " +
                          "Continue to monitor for new Long-Term Support (LTS) releases and plan upgrades " +
                          "to stay current and maintain optimal performance, security, and support."
            }));
        
        return  Task.CompletedTask;
    }
}