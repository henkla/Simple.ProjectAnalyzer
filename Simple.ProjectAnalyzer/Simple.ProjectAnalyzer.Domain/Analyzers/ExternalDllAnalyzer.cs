using System.Xml.Linq;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analyzers;

public class ExternalDllAnalyzer : AnalyzerBase
{
    public override Task Run(ref AnalysisContext context)
    {
        const string source = nameof(ExternalDllAnalyzer);
        
        foreach (var project in context.Projects)
        {
            var hasDllReferences = HasDllReferences(project.Path);
            
            if (hasDllReferences)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = source,
                    Message = "This project contains direct references to external DLL files via HintPath. " +
                              "This approach is fragile, as it can lead to version conflicts and makes dependency " +
                              "management harder. Consider using NuGet packages or project references instead.",
                    Code = ResultCode.Suggestion
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = source,
                    Message = "No direct references to external DLL files found.",
                    Code = ResultCode.Ok
                });
            }
        }
        
        return Task.CompletedTask;
        
    }
    
    private static bool HasDllReferences(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

        return doc.Descendants(ns + "Reference")
            .Any(r => r.Element(ns + "HintPath")?.Value.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) == true);
    }
}