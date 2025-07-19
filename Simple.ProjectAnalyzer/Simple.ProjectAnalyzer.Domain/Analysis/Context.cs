using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Utilities;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Context
{
    public required TargetFramework CurrentLtsVersion { get; init; }
    public List<Project> Projects { get; set; } = [];
    public required AnalyzeCommandSettings CommandSettings { get; set; }
    public ExitCode ExitCode { get; set; } = ExitCode.Ok;
}