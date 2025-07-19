namespace Simple.ProjectAnalyzer.Domain.Models;

public enum ExitCode
{
    Ok = 0,
    
    // warnings: 100-199
    
    Warning = 100,
    
    // errors: 200-299
    
    Error = 200,
    
    // exceptions: 300-399
    
    Exception = 300
}