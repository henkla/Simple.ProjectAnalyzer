using System.Xml.Linq;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class ExternalDllAnalyzer : AnalyzerBase
{
    public override Task Run(ref Context context)
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
                    Message = $"Project '{project.Name}' contains direct DLL references.",
                    Details = "This project contains one or more direct references to external DLL files using 'HintPath' " +
                              "in the project file. This approach is considered fragile because it tightly couples the build " +
                              "to specific file paths and versions. It can lead to versioning issues, broken builds across " +
                              "machines, and maintenance complexity. To improve reliability and maintainability, consider " +
                              "replacing these references with NuGet packages or project references. This enables better " +
                              "dependency resolution, version control, and CI/CD compatibility.",
                    Code = ResultCode.Suggestion,
                    Parent = project
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = source,
                    Message = $"Project '{project.Name}' has no direct DLL references.",
                    Details = "No 'HintPath'-based direct DLL references were detected. This is the recommended approach.",
                    Code = ResultCode.Ok,
                    Parent = project
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