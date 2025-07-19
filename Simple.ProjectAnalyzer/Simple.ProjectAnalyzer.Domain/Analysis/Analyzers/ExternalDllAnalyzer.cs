using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class ExternalDllAnalyzer : AnalyzerBase
{
    public override Task Run(ref Context context)
    {
        foreach (var project in context.Projects)
        {
            var hasDllReferences = project.References.Count > 0;

            if (hasDllReferences)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = nameof(ExternalDllAnalyzer),
                    Title = "External DLL reference(s)",
                    Message = "Direct DLL references detected in the project.",
                    Details = "This approach is considered fragile because it tightly couples the build to specific file paths and versions. " +
                              "It can lead to versioning issues, broken builds across different machines, and increased maintenance complexity. " +
                              "To improve reliability and maintainability, consider replacing these references with NuGet packages or project references. " +
                              "This enables better dependency resolution, version control, and smoother CI/CD workflows.",
                    Recommendation = "Consider replacing external DLL references with NuGet packages or project references.",
                    Code = AnalysisResultCode.Hint,
                    Parent = project
                });

                foreach (var reference in project.References)
                {
                    reference.AnalysisResults.Add(new AnalysisResult
                    {
                        Source = nameof(ExternalDllAnalyzer),
                        Title = "External DLL reference",
                        Message = "This is a direct DLL reference.",
                        Details = "This approach is considered fragile because it tightly couples the build to specific file paths and versions. " +
                                  "It can lead to versioning issues, broken builds across different machines, and increased maintenance complexity. " +
                                  "To improve reliability and maintainability, consider replacing these references with NuGet packages or project references. " +
                                  "This enables better dependency resolution, version control, and smoother CI/CD workflows.",
                        Recommendation = "Consider replacing external DLL references with NuGet packages or project references.",
                        Code = AnalysisResultCode.Hint,
                        Parent = project
                    });
                }
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = nameof(ExternalDllAnalyzer),
                    Title = "No external DLL references",
                    Message = "Project has no direct DLL references.",
                    Details = "No 'HintPath'-based direct DLL references were detected. This is the recommended approach.",
                    Code = AnalysisResultCode.Ok,
                    Parent = project
                });
            }
        }
        
        return Task.CompletedTask;
    }
}