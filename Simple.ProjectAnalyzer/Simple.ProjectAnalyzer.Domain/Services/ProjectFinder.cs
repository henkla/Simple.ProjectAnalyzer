using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Abstractions.Output;

namespace Simple.ProjectAnalyzer.Domain.Services;

public partial class ProjectFinder(IConsoleOutput console)
{
    private static readonly string[] IgnoredDirectories = ["bin", "obj", ".git", ".vs", ".idea" ];
    private const string ProjectFileExtension = "*.csproj";
    private const string SolutionFileExtension = "*.sln";

    [GeneratedRegex("^Project\\(\"{.*}\"\\) = \".*\", \"(.*\\.csproj)\", \".*\"$", RegexOptions.IgnoreCase, "sv-SE")]
    private static partial Regex SolutionFileRegex();
    
    public List<string> FindProjectFiles(string path)
    {
        console.Verbose($"{nameof(ProjectFinder)}.{nameof(FindProjectFiles)} started");
        console.Verbose($"Ignored directories: {string.Join(", ", IgnoredDirectories)}");
        console.Verbose($"Starting path: {path}");

        var projectFiles = GetProjectFilesInPathDirectory(path);
        if (projectFiles.Count != 0)
        {
            console.Verbose($"Found {projectFiles.Count} project file(s) in provided path directory");
            return projectFiles;
        }

        projectFiles = GetProjectFilesBelowPathDirectory(path);
        if (projectFiles.Count != 0)
        {
            console.Verbose($"Found {projectFiles.Count} project file(s) recursively below provided path directory");
            return projectFiles;
        }

        projectFiles = GetProjectFilesAbovePathDirectory(path);
        if (projectFiles.Count != 0)
        {
            console.Verbose($"Found {projectFiles.Count} project file(s) above path directory");
        }

        projectFiles = GetProjectFilesFromSolutionFileAbovePathDirectory(path);
        if (projectFiles.Count != 0)
        {
            console.Verbose($"Found {projectFiles.Count} project file(s) from solution file above path directory");
        }

        console.Verbose("No project file(s) found");
        return [];
    }

    private List<string> GetProjectFilesFromSolutionFileAbovePathDirectory(string path)
    {
        var directory = new DirectoryInfo(path);

        while (directory is not null)
        {
            var solutionFiles = Directory.EnumerateFiles(directory.FullName, SolutionFileExtension, SearchOption.TopDirectoryOnly).ToList();
            if (solutionFiles.Count != 0)
            {
                var solutionFile = solutionFiles.First();
                var projectsInSln = ParseProjectFilesFromSolutionFile(solutionFile);

                var projectFiles = projectsInSln
                    .Select(projectFile => Path.GetFullPath(Path.Combine(directory.FullName, projectFile)))
                    .Where(p => File.Exists(p) && !IsInIgnoredDirectory(p))
                    .ToList();

                if (projectFiles.Count != 0)
                {
                    return projectFiles;
                }
            }

            directory = directory.Parent;
        }

        return [];
    }

    private static List<string> GetProjectFilesAbovePathDirectory(string path)
    {
        var directory = new DirectoryInfo(path);

        while (directory != null)
        {
            var projectFiles = Directory.EnumerateFiles(directory.FullName, "*.csproj", SearchOption.TopDirectoryOnly)
                .Where(p => !IsInIgnoredDirectory(p))
                .ToList();

            if (projectFiles.Count != 0)
            {
                return projectFiles;
            }

            directory = directory.Parent;
        }

        return [];
    }

    private static List<string> GetProjectFilesInPathDirectory(string path)
    {
        return Directory.EnumerateFiles(path, ProjectFileExtension, SearchOption.TopDirectoryOnly)
            .Where(p => !IsInIgnoredDirectory(p))
            .ToList();
    }

    private static bool IsInIgnoredDirectory(string path)
    {
        var directoryParts = Path.GetDirectoryName(path)?.Split(Path.DirectorySeparatorChar) ?? [];
        return directoryParts.Any(part => IgnoredDirectories.Contains(part, StringComparer.OrdinalIgnoreCase));
    }

    private static List<string> GetProjectFilesBelowPathDirectory(string path)
    {
        return Directory.EnumerateFiles(path, ProjectFileExtension, SearchOption.AllDirectories)
            .Where(p => !IsInIgnoredDirectory(p))
            .ToList();
    }

    private static List<string> ParseProjectFilesFromSolutionFile(string path)
    {
        var projectFiles = new List<string>();

        foreach (var line in File.ReadAllLines(path))
        {
            var match = SolutionFileRegex().Match(line);
            if (match is { Success: true, Groups.Count: > 1 })
            {
                projectFiles.Add(match.Groups[1].Value.Replace('/', Path.DirectorySeparatorChar));
            }
        }

        return projectFiles;
    }
}