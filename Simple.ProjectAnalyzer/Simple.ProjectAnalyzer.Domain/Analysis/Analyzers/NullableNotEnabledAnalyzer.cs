using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

public class NullableNotEnabledAnalyzer : IAnalyzer
{
    public string Description => "Analyzes project files to determine whether nullable reference types are " +
                                 "properly configured. Projects without explicit <Nullable> settings, or with " +
                                 "nullability disabled, risk introducing null-related bugs that could have been " +
                                 "caught at compile time. This analyzer encourages enabling nullable reference types " +
                                 "to improve code safety, clarity, and consistency across the codebase.";
    
    public IEnumerable<AnalysisResultCode> ResultCodes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Warning, 
        AnalysisResultCode.Ok
    ];
    
    public Task Run(Context context)
    {
        Output.Verbose($"{nameof(NullableNotEnabledAnalyzer)}.{nameof(Run)} started");

        foreach (var project in context.Projects)
        {
            switch (project.NullableEnabled)
            {
                case null:
                    project.AnalysisResults.Add(new AnalysisResult
                    {
                        Source = nameof(NullableNotEnabledAnalyzer),
                        Parent = project,
                        Code = AnalysisResultCode.Hint,
                        Title = "Nullable reference types unset",
                        Message = "Nullable context is not explicitly defined.",
                        Details = "The <Nullable> setting is missing from the project file. This means " +
                                  "the compiler may fall back to inherited configuration from files such " +
                                  "as Directory.Build.props or global build settings, which can lead to " +
                                  "inconsistent nullability enforcement across projects.",
                        Recommendation = "Consider adding <Nullable>enable</Nullable> to the project file " +
                                         "to make the intent explicit and ensure consistent behavior regardless " +
                                         "of inherited settings."
                    });
                    break;

                case false:
                    project.AnalysisResults.Add(new AnalysisResult
                    {
                        Source = nameof(NullableNotEnabledAnalyzer),
                        Parent = project,
                        Code = AnalysisResultCode.Warning,
                        Title = "Nullable reference types disabled",
                        Message = "Nullable reference types are explicitly disabled.",
                        Details = "The <Nullable> setting in the project file is set to 'disable', which " +
                                  "means the compiler will not issue warnings for potential null reference issues.",
                        Recommendation = "Enable nullable reference types by setting <Nullable>enable</Nullable> " +
                                         "in the project file to improve code safety and catch null-related issues " +
                                         "at compile time."
                    });
                    break;

                default:
                    project.AnalysisResults.Add(new AnalysisResult
                    {
                        Source = nameof(NullableNotEnabledAnalyzer),
                        Parent = project,
                        Code = AnalysisResultCode.Ok,
                        Title = "Nullable reference types enabled",
                        Message = "Nullable reference types are enabled.",
                        Details = "The project has <Nullable>enable</Nullable>, which enforces nullability checks during compilation."
                    });
                    break;
            }
        }

        return Task.CompletedTask;
    }
}