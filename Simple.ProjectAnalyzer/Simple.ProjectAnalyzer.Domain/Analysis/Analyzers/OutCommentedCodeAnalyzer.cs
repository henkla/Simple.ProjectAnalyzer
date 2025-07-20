using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public partial class OutCommentedCodeAnalyzer : IAnalyzer
{
    [GeneratedRegex(@"<!--\s*<[^>]+>.*?-->", RegexOptions.Singleline)]
    private static partial Regex OutCommentedCodeRegex();
    
    public async Task Run(Context context)
    {
        Output.Verbose($"{nameof(OutCommentedCodeAnalyzer)}.{nameof(Run)} started");

        foreach (var project in context.Projects)
        {
            var projectFileHasComments = await ContainsCommentedOutXmlCode(project.Path);
            if (projectFileHasComments)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Hint,
                    Parent = project,
                    Source = nameof(OutCommentedCodeAnalyzer),
                    Title = "Commented-out code in project file",
                    Message = "Commented-out XML elements detected in the project file.",
                    Details = "The .csproj file contains XML elements inside comments, which may be leftover code or configurations.",
                    Recommendation = "Remove or clean up commented-out XML to improve clarity and maintainability."
                });
            }
            else
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Ok,
                    Parent = project,
                    Source = nameof(OutCommentedCodeAnalyzer),
                    Title = "Commented-out code in project file",
                    Message = "No commented-out code found in the project file.",
                    Details = "The .csproj file appears to be free of commented-out XML elements."
                });
            }
        }
    }
    
    private static async Task<bool> ContainsCommentedOutXmlCode(string projectFilePath)
    {
        var projectFileText = await File.ReadAllTextAsync(projectFilePath);

        return OutCommentedCodeRegex().Matches(projectFileText).Count > 0;
    }
}