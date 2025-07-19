namespace Simple.ProjectAnalyzer.Domain.Models;

public class Project : IAnalyzable
{
    public string Name { get; init; }
    public string? Sdk { get; set; }
    public List<TargetFramework> TargetFrameworks { get; set; } = [];
    public List<string> ProjectReferences { get; set; } = [];
    public List<Reference> References { get; set; } = [];
    public required string Path { get; init; }
    public List<PackageReference> PackageReferences { get; init; } = [];
    public List<AnalysisResult> AnalysisResults { get; } = [];
    public required bool IsLegacy { get; set; }
}