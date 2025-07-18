using System.Runtime.Serialization;

namespace Simple.ProjectAnalyzer.Domain;

/// <summary>
/// Represents errors specific to the project analyzer.
/// </summary>
public class ProjectAnalyzerException : Exception
{
    public ProjectAnalyzerException()
    {
    }

    public ProjectAnalyzerException(string message)
        : base(message)
    {
    }

    public ProjectAnalyzerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}