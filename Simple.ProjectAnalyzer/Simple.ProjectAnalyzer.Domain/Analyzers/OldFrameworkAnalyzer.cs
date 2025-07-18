using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class OldFrameworkAnalyzer : AnalyzerBase
{
    public override Task Run(ref AnalysisContext context)
    {
        var legacyProjects = context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major < 5))
            .ToList();

        var legacyProjectAnalysisResult = new AnalysisResult
        {
            Source = nameof(OldFrameworkAnalyzer),
            Message = "Project is of legacy type. Update to modern version",
            Code = ResultCode.Warning
        };
        
        legacyProjects.ForEach(p => p.AnalysisResults.Add(legacyProjectAnalysisResult));
        
        var sdkProjects = context.Projects
            .Where(p => p.TargetFrameworks.Any(tf => tf.Version.Major >= 5))
            .ToList();

        foreach (var sdkProject in sdkProjects)
        {
            var highestTargetFramework = GetHighestTargetFramework(sdkProject.TargetFrameworks);
            var projectResult = CompareProjectVersionToCurrentLts(highestTargetFramework.Version, context.CurrentLtsVersion.Version);

            sdkProject.AnalysisResults.Add(projectResult);
        }

        return Task.CompletedTask;
    }

    private static AnalysisResult CompareProjectVersionToCurrentLts(Version projectVersion, Version ltsVersion)
    {
        const string source = nameof(OldFrameworkAnalyzer);

        return projectVersion.CompareTo(ltsVersion) switch
        {
            > 0 => new AnalysisResult { Source = source, Message = "Project target framework are ahead of LTS version", Code = ResultCode.Ok },
            < 0 => new AnalysisResult { Source = source, Message = "Project target framework are behind LTS version", Code = ResultCode.Warning },
            _ => new AnalysisResult { Source = source, Message = "Project target framework equal to LTS version", Code = ResultCode.Ok }
        };
    }

    private static TargetFramework GetHighestTargetFramework(List<TargetFramework> targetFrameworks)
    {
        return targetFrameworks
            .OrderByDescending(tf => tf.Version)
            .First();
    }
}