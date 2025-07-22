using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AnalyzerManifestAttribute(
    string name,
    string description,
    AnalysisResultCode[] codes,
    AnalysisResultType[] targets)
    : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public AnalysisResultCode[] Codes { get; } = codes;
    public AnalysisResultType[] Targets { get; } = targets;
}