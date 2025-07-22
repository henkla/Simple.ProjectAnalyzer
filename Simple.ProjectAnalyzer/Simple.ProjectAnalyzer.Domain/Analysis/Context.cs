using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Context
{
    public required TargetFramework CurrentLtsVersion { get; init; }
    public List<Project> Projects { get; set; } = [];
    public ApplicationExitCode ApplicationExitCode { get; set; } = ApplicationExitCode.Ok;
    public bool AnalysisHasRun => Projects.Any(p => p.AnalysisResults.Count != 0);
    public string? Path { get; set; }
    public string[]? Analyzers { get; set; }
    public bool Verbose { get; set; }
}