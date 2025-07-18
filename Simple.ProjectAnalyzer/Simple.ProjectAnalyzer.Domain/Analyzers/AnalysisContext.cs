using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Utilities;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class AnalysisContext
{
    public required CurrentLtsVersion CurrentLtsVersion { get; init; }
    public List<Project> Projects { get; set; } = [];
    public required AnalyzeSettings Settings { get; set; }
    public ExitCode ExitCode { get; set; } = ExitCode.Ok;
}