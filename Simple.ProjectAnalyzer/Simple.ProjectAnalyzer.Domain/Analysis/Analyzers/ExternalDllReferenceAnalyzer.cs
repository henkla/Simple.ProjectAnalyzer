using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

[AnalyzerManifest(
    name: nameof(ExternalDllReferenceAnalyzer),
    codes:
    [
        AnalysisResultCode.Hint,
        AnalysisResultCode.Ok
    ],
    targets:
    [
        AnalysisResultType.Project,
        AnalysisResultType.ExternalReference
    ],
    description: "Scans project files to identify direct external DLL references. Such references rely on " +
                 "fixed file paths and versions, which can lead to fragile builds, version conflicts, and " +
                 "difficulties in dependency management across environments. This analyzer highlights projects " +
                 "that use direct DLL references and encourages replacing them with NuGet packages or project " +
                 "references to improve build reliability, versioning consistency, and maintainability.")]
public class ExternalDllReferenceAnalyzer(IConsoleOutput console) : IAnalyzer
{
    public Task Run(Context context)
    {
        console.Verbose($"{nameof(ExternalDllReferenceAnalyzer)}.{nameof(Run)} started");

        foreach (var project in context.Projects)
        {
            var hasDllReferences = project.References.Count > 0;

            if (hasDllReferences)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Source = nameof(ExternalDllReferenceAnalyzer),
                    Type = AnalysisResultType.Project,
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
                        Source = nameof(ExternalDllReferenceAnalyzer),
                        Title = "External DLL reference",
                        Type = AnalysisResultType.Project,
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
                    Source = nameof(ExternalDllReferenceAnalyzer),
                    Title = "No external DLL references",
                    Type = AnalysisResultType.Project,
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