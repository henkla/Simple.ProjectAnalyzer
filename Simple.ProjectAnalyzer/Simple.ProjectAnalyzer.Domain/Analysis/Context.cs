using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Utilities;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Context
{
    public required TargetFramework CurrentLtsVersion { get; init; }
    public List<Project> Projects { get; set; } = [];
    public required IAnalyzeCommandSettings AnalyzeCommandSettings { get; set; }
    public ApplicationExitCode ApplicationExitCode { get; set; } = ApplicationExitCode.Ok;
    public bool AnalysisHasRun => Projects.Any(p => p.AnalysisResults.Count != 0);
}