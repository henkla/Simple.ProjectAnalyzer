namespace Simple.ProjectAnalyzer.Domain.Models;

public enum ApplicationExitCode
{
    Ok = 0,
    
    // hints: 100-199
    Hint = 100,
    
    // warnings: 200-299
    Warning = 200,
    
    // errors: 300-399
    Error = 300,
    
    // exceptions: 400-499
    Exception = 400
}