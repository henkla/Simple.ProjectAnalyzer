using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.ExternalDllReference;

public partial class Analyzer
{
    public string Name => "ExternalDllReferenceAnalyzer";
    
    public string Description =>  "Scans project files to identify direct external DLL references. " +
                                  "Such references rely on fixed file paths and versions, which can lead to fragile builds, " +
                                  "version conflicts, and difficulties in dependency management across environments. " +
                                  "This analyzer highlights projects that use direct DLL references and encourages " +
                                  "replacing them with NuGet packages or project references to improve build reliability, " +
                                  "versioning consistency, and maintainability.";
    
    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Ok
    ];
    
    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project, 
        AnalysisResultType.ExternalReference
    ];
}