using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class ImplicitUsingsAnalyzer : IAnalyzer
{
    public string Description => "Analyzes project files to determine whether <ImplicitUsings> is explicitly defined. " +
                                 "Implicit usings provide a set of default using directives depending on the target framework, " +
                                 "which can simplify code but may also introduce hidden dependencies. " +
                                 "This analyzer helps ensure that the implicit using behavior is intentional and clearly declared " +
                                 "in the project file, improving project clarity and reducing ambiguity.";

    public IEnumerable<AnalysisResultCode> Codes =>
    [
        AnalysisResultCode.Ok,
        AnalysisResultCode.Hint,
        AnalysisResultCode.Warning
    ];

    public IEnumerable<AnalysisResultType> Targets =>
    [
        AnalysisResultType.Project
    ];

    public Task Run(Context context)
    {

        foreach (var project in context.Projects)
        {
            if (project.ImplicitUsingsEnabled == null)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Hint,
                    Parent = project,
                    Type = AnalysisResultType.Project,
                    Source = nameof(ImplicitUsingsAnalyzer),
                    Title = "ImplicitUsings not defined",
                    Message = "Project does not explicitly define <ImplicitUsings>.",
                    Details = "Consider adding <ImplicitUsings> to make the configuration more explicit and consistent across projects."
                });
            }
            else if (project.ImplicitUsingsEnabled == false)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Warning,
                    Parent = project,
                    Type = AnalysisResultType.Project,
                    Source = nameof(ImplicitUsingsAnalyzer),
                    Title = "ImplicitUsings is disabled",
                    Message = "Project has <ImplicitUsings> set to false.",
                    Details = "Disabling implicit usings might lead to more verbose code. Ensure this is intentional and consistent with the team's conventions."
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Ok,
                    Parent = project,
                    Type = AnalysisResultType.Project,
                    Source = nameof(ImplicitUsingsAnalyzer),
                    Title = "ImplicitUsings is enabled",
                    Message = $"Project '{project.Name}' has <ImplicitUsings> enabled.",
                    Details = "No action needed."
                });
            }
        }
        
        return Task.CompletedTask;
    }
}
