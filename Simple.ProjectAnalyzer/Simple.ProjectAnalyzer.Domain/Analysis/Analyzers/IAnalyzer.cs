using System.Reflection;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public interface IAnalyzer
{
    Task Run(Context context);

    AnalyzerManifestAttribute GetManifest() =>
        GetType().GetCustomAttribute<AnalyzerManifestAttribute>()
        ?? throw new Exception("Unable to find AnalyzerManifestAttribute");
}