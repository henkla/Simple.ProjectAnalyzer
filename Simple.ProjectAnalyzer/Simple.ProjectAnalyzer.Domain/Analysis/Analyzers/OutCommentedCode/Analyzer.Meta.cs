using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Analysis.Analyzers.OutCommentedCode;

public partial class Analyzer
{
    public string Name => "OutCommentedCodeAnalyzer";
    
    public string Description =>  "Scans project files for commented-out XML elements, typically remnants " +
                                  "of removed or temporarily disabled configurations. Such commented code " +
                                  "can clutter the project file, reduce readability, and lead to confusion " +
                                  "during maintenance. This analyzer helps identify and clean up obsolete or " +
                                  "unnecessary commented-out code to promote cleaner and more maintainable project files.";
    
    public IEnumerable<AnalysisResultCode> Codes => [
        AnalysisResultCode.Hint, 
        AnalysisResultCode.Ok
    ];
    
    public IEnumerable<AnalysisResultType> Targets => [
        AnalysisResultType.Project
    ];
}