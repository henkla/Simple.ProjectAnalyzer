namespace Simple.ProjectAnalyzer.Domain.Models;

public class Project : IAnalyzable
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public string? Sdk { get; init; }
    public required bool IsLegacy { get; init; }
    public bool? NullableEnabled { get; init; }
    public List<TargetFramework> TargetFrameworks { get; init; } = [];
    public List<string> ProjectReferences { get; init; } = [];
    public List<Reference> References { get; init; } = [];
    public List<PackageReference> PackageReferences { get; init; } = [];
    public List<AnalysisResult> AnalysisResults { get; } = [];
}