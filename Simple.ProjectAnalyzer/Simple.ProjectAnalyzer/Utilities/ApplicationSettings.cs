namespace Simple.ProjectAnalyzer.Utilities;

public static class ApplicationSettings
{
    public const string ApplicationCulture = "en-US";
    public const string ApplicationName = "Simple.ProjectAnalyzer";
    public const string ApplicationExample = "--path https://github.com/henkla/Simple.ProjectAnalyzer.git " +
                                             "--analyzer DuplicatePackageReferenceAnalyzer --verbose";
    public const string ApplicationDescription = "ProjectAnalyzer is a command-line tool designed to perform " +
                                                 "static analysis on one or more .NET projects. It helps " +
                                                 "developers understand project structure, assess framework and " +
                                                 "dependency usage, and identify potential upgrade paths.";
}