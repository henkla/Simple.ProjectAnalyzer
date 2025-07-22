using Simple.ProjectAnalyzer.Abstractions.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

[AnalyzerManifest(
    name: nameof(NullableNotEnabledAnalyzer),
    codes:
    [
        AnalysisResultCode.Hint,
        AnalysisResultCode.Warning,
        AnalysisResultCode.Ok
    ],
    targets:
    [
        AnalysisResultType.Project
    ],
    description: "Analyzes project files to determine whether nullable reference types are properly configured. " +
                 "Projects without explicit <Nullable> settings, or with nullability disabled, risk introducing " +
                 "null-related bugs that could have been caught at compile time.")]
public class NullableNotEnabledAnalyzer(IConsoleOutput console) : IAnalyzer
{
    public Task Run(Context context)
    {
        console.Verbose($"{nameof(NullableNotEnabledAnalyzer)}.{nameof(Run)} started");

        foreach (var project in context.Projects)
        {
            var result = project.NullableEnabled switch
            {
                null => new AnalysisResult
                {
                    Source = nameof(NullableNotEnabledAnalyzer),
                    Parent = project,
                    Code = AnalysisResultCode.Hint,
                    Type = AnalysisResultType.Project,
                    Title = "Nullable reference types unset",
                    Message = "Nullable context is not explicitly defined.",
                    Details = "The <Nullable> setting is missing from the project file. This may cause " +
                              "inconsistent nullability enforcement across projects.",
                    Recommendation = "Add <Nullable>enable</Nullable> to ensure consistent null-safety behavior."
                },

                false => new AnalysisResult
                {
                    Source = nameof(NullableNotEnabledAnalyzer),
                    Parent = project,
                    Code = AnalysisResultCode.Warning,
                    Type = AnalysisResultType.Project,
                    Title = "Nullable reference types disabled",
                    Message = "Nullable reference types are explicitly disabled.",
                    Details = "This project opts out of nullability checks at compile time.",
                    Recommendation = "Consider enabling nullability by setting <Nullable>enable</Nullable> " +
                                     "to catch potential null reference issues early."
                },

                _ => new AnalysisResult
                {
                    Source = nameof(NullableNotEnabledAnalyzer),
                    Parent = project,
                    Code = AnalysisResultCode.Ok,
                    Type = AnalysisResultType.Project,
                    Title = "Nullable reference types enabled",
                    Message = "Nullable reference types are enabled.",
                    Details = "This project uses <Nullable>enable</Nullable>, enabling null-safety at compile time."
                }
            };

            project.AnalysisResults.Add(result);
        }

        return Task.CompletedTask;
    }
}