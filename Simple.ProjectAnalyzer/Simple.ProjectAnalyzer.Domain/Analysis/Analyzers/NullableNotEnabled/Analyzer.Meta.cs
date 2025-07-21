using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.NullableNotEnabled;

public partial class Analyzer
{
    public string Name => "NullableNotEnabledAnalyzer";
    
    public string Description => "Analyzes project files to determine whether nullable reference types are " +
                                 "properly configured. Projects without explicit <Nullable> settings, or with " +
                                 "nullability disabled, risk introducing null-related bugs that could have been " +
                                 "caught at compile time. This analyzer encourages enabling nullable reference types " +
                                 "to improve code safety, clarity, and consistency across the codebase.";
    
    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Warning, 
        AnalysisResultCode.Ok
    ];

    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project
    ];
}