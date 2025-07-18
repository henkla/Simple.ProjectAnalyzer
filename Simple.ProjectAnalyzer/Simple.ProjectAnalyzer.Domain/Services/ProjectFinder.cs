using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class ProjectFinder
{
    public List<string> FindProjectFiles(AnalyzeCommandSettings commandSettings)
    {
        if (commandSettings.Verbose)
        {
            Writer.WriteVerbose("Looking for project files...");
            Writer.WriteVerbose($"Path: {commandSettings.Path}");
        }

        // 1) Kolla om det finns csproj-filer i just startPath (icke rekursivt), exkludera bin/obj
        var csprojFiles = Directory.EnumerateFiles(commandSettings.Path!, "*.csproj", SearchOption.TopDirectoryOnly)
            .Where(path => !IsInIgnoredDir(path))
            .ToList();

        if (csprojFiles.Count != 0)
        {
            if (commandSettings.Verbose)
            {
                Writer.WriteVerbose($"Found {csprojFiles.Count} project(s)");
            }

            return csprojFiles;
        }

        // 2) Leta rekursivt neråt i alla barn-mappar, exkludera bin/obj
        csprojFiles = GetAllCsprojFilesRecursive(commandSettings.Path!);
        if (csprojFiles.Count != 0)
        {
            if (commandSettings.Verbose)
            {
                Writer.WriteVerbose($"Found {csprojFiles.Count} project(s)");
            }

            return csprojFiles;
        }

        // 3) Leta uppåt tills vi hittar en csproj-fil, returnera alla csproj i den mappen
        var dir = new DirectoryInfo(commandSettings.Path!);
        while (dir != null)
        {
            csprojFiles = Directory.EnumerateFiles(dir.FullName, "*.csproj", SearchOption.TopDirectoryOnly)
                .Where(path => !IsInIgnoredDir(path))
                .ToList();

            if (csprojFiles.Count != 0)
            {
                if (commandSettings.Verbose)
                {
                    Writer.WriteVerbose($"Found {csprojFiles.Count} project(s)");
                }
                
                return csprojFiles;
            }

            dir = dir.Parent;
        }

        // 4) Leta uppåt tills vi hittar en sln-fil, plocka ut csproj-filer som finns i den
        dir = new DirectoryInfo(commandSettings.Path!);
        while (dir != null)
        {
            var slnFiles = Directory.EnumerateFiles(dir.FullName, "*.sln", SearchOption.TopDirectoryOnly).ToList();
            if (slnFiles.Count != 0)
            {
                // Ta första sln-filen (om flera finns)
                var slnFile = slnFiles.First();
                var projectsInSln = ParseProjectsFromSln(slnFile);

                // Konvertera ev. relativa sökvägar i sln till absoluta sökvägar
                var csprojPaths = projectsInSln
                    .Select(projPath => Path.GetFullPath(Path.Combine(dir.FullName, projPath)))
                    .Where(path => File.Exists(path) && !IsInIgnoredDir(path))
                    .ToList();

                if (csprojPaths.Count != 0)
                {
                    if (commandSettings.Verbose)
                    {
                        Writer.WriteVerbose($"Found {csprojPaths.Count} project(s)");
                    }
                    
                    return csprojFiles;
                }
            }

            dir = dir.Parent;
        }

        if (commandSettings.Verbose)
        {
            Writer.WriteVerbose("No project(s) found");
        }

        return [];
    }

    private bool IsInIgnoredDir(string filePath)
    {
        var ignoredDirs = new[] { "bin", "obj", ".git", ".vs", ".idea" };
        var dirParts = Path.GetDirectoryName(filePath)?.Split(Path.DirectorySeparatorChar) ?? Array.Empty<string>();
        return dirParts.Any(part => ignoredDirs.Contains(part, StringComparer.OrdinalIgnoreCase));
    }

    private List<string> GetAllCsprojFilesRecursive(string rootPath)
    {
        return Directory.EnumerateFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !IsInIgnoredDir(path))
            .ToList();
    }

    private List<string> ParseProjectsFromSln(string slnFile)
    {
        var projectPaths = new List<string>();
        var lines = File.ReadAllLines(slnFile);

        // Exempelrad i .sln:
        // Project("{GUID}") = "ProjectName", "relative\path\to\project.csproj", "{GUID}"
        var regex = new Regex("^Project\\(\"{.*}\"\\) = \".*\", \"(.*\\.csproj)\", \".*\"$", RegexOptions.IgnoreCase);

        foreach (var line in lines)
        {
            var match = regex.Match(line);
            if (match.Success && match.Groups.Count > 1)
            {
                projectPaths.Add(match.Groups[1].Value.Replace('/', Path.DirectorySeparatorChar));
            }
        }

        return projectPaths;
    }

    private List<Project> GetProjectFromProjectFiles(List<string> projectFiles)
    {
        return projectFiles.Select(projectFile => new Project
        {
            Path = projectFile,
            Name = Path.GetFileNameWithoutExtension(projectFile),
        }).ToList();
    }
}