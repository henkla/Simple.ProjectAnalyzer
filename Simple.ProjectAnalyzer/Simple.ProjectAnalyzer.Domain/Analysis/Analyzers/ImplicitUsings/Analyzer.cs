using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.ImplicitUsings;

public partial class Analyzer : IAnalyzer
{
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
                    Source = Name,
                    Title = "Declaration on <ImplicitUsings>",
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
                    Source = Name,
                    Title = "Declaration on <ImplicitUsings>",
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
                    Source = Name,
                    Title = "<ImplicitUsings>",
                    Message = $"Project '{project.Name}' has <ImplicitUsings> enabled.",
                    Details = "No action needed."
                });
            }
        }
        
        return Task.CompletedTask;
    }
}
