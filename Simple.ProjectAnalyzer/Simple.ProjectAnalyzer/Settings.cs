namespace Simple.ProjectAnalyzer;

public static class Settings
{
    public const string ApplicationCulture = "en-US";
    public static readonly string ApplicationName = typeof(Settings).Namespace!;
    public const string ApplicationExample = "git --path https://github.com/henkla/Simple.ProjectAnalyzer.git --verbose";
    public const string ApplicationDescription = "ProjectAnalyzer is a command-line tool designed to perform " +
                                                 "static analysis on one or more .NET projects. It helps " +
                                                 "developers understand project structure, assess framework and " +
                                                 "dependency usage, and identify potential upgrade paths.";
}