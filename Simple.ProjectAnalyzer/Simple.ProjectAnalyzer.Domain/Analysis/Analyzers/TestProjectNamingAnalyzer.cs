using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class TestProjectNamingAnalyzer : IAnalyzer
{
    public string Description =>
        "Ensures that test projects follow a consistent naming convention, typically ending with '.Tests'. " +
        "Consistent naming helps improve discoverability, test filtering, and overall project structure clarity.";

    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Ok
    ];

    public IEnumerable<AnalysisResultType> Targets =>
    [
        AnalysisResultType.Project
    ];
    
    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(TestProjectNamingAnalyzer)}.{nameof(Run)} started");

        foreach (var project in context.Projects)
        {
            if (!IsTestProject(project))
            {
                continue;
            }

            if (!project.Name.EndsWith(".Tests", StringComparison.OrdinalIgnoreCase))
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Hint,
                    Parent = project,
                    Type = AnalysisResultType.Project,
                    Source = nameof(TestProjectNamingAnalyzer),
                    Title = "Test project naming convention",
                    Message = "The project appears to be a test project but does not follow the '.Tests' suffix convention.",
                    Details = "Projects that reference test frameworks like xUnit, NUnit, or MSTest should be named consistently for easier identification.",
                    Recommendation = "Consider renaming the project to end with '.Tests'."
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Ok,
                    Parent = project,
                    Type = AnalysisResultType.Project,
                    Source = nameof(TestProjectNamingAnalyzer),
                    Title = "Test project naming convention",
                    Message = "The project follows the expected naming convention.",
                    Details = "Project ends with '.Tests' and appears valid."
                });
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsTestProject(Project project)
    {
        var knownTestFrameworks = new[]
        {
            "xunit", "xunit.runner.visualstudio",
            "nunit", "nunit3testadapter",
            "mstest.testadapter", "mstest.testframework"
        };

        return project.PackageReferences
                      .Any(p => knownTestFrameworks.Any(fw =>
                          p.Name.Contains(fw, StringComparison.OrdinalIgnoreCase)));
    }
}