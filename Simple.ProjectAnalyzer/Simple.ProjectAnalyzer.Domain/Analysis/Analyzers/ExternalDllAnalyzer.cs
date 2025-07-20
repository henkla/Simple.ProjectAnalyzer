using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class ExternalDllAnalyzer : IAnalyzer
{
    public string Description =>  "Scans project files to identify direct external DLL references. " +
                                  "Such references rely on fixed file paths and versions, which can lead to fragile builds, " +
                                  "version conflicts, and difficulties in dependency management across environments. " +
                                  "This analyzer highlights projects that use direct DLL references and encourages " +
                                  "replacing them with NuGet packages or project references to improve build reliability, " +
                                  "versioning consistency, and maintainability.";
    
    public IEnumerable<AnalysisResultCode> ResultCodes => [AnalysisResultCode.Hint, AnalysisResultCode.Ok];
    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(ExternalDllAnalyzer)}.{nameof(Run)} started");
        
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