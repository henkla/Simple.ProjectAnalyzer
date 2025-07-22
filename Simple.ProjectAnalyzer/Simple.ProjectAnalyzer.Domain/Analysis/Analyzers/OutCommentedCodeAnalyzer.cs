using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

[AnalyzerManifest(
    name: nameof(OutCommentedCodeAnalyzer),
    codes:
    [
        AnalysisResultCode.Hint,
        AnalysisResultCode.Ok
    ],
    targets:
    [
        AnalysisResultType.Project
    ],
    description: "Scans project files for commented-out XML elements, typically remnants " +
                 "of removed or temporarily disabled configurations. Such commented code " +
                 "can clutter the project file, reduce readability, and lead to confusion " +
                 "during maintenance. This analyzer helps identify and clean up obsolete or " +
                 "unnecessary commented-out code to promote cleaner and more maintainable project files.")]
public partial class OutCommentedCodeAnalyzer(IConsoleOutput console) : IAnalyzer
{
    [GeneratedRegex(@"<!--\s*<[^>]+>.*?-->", RegexOptions.Singleline)]
    private static partial Regex OutCommentedCodeRegex();

    public async Task Run(Context context)
    {
        console.Verbose($"{nameof(OutCommentedCodeAnalyzer)}.{nameof(Run)} started");

        foreach (var project in context.Projects)
        {
            var projectFileHasComments = await ContainsCommentedOutXmlCode(project.Path);
            if (projectFileHasComments)
            {
                project.AnalysisResults.Add(new AnalysisResult
                {
                    Code = AnalysisResultCode.Hint,
                    Parent = project,
                    Type = AnalysisResultType.Project,
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
                    Type = AnalysisResultType.Project,
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