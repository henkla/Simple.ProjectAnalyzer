using System.Text.RegularExpressions;
using Simple.ProjectAnalyzer.Domain.CommandLine;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class ProjectFinder
{
    public List<string> FindProjectFiles(string path, bool verbose)
    {
        if (verbose)
        {
            Writer.WriteVerbose("Looking for project files...");
            Writer.WriteVerbose($"Path: {path}");
        }

        // 1) Kolla om det finns csproj-filer i just startPath (icke rekursivt), exkludera bin/obj
        var csprojFiles = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly)
            .Where(p => !IsInIgnoredDir(p))
            .ToList();

        if (csprojFiles.Count != 0)
        {
            if (verbose)
            {
                Writer.WriteVerbose($"Found {csprojFiles.Count} project(s)");
            }

            return csprojFiles;
        }

        // 2) Leta rekursivt neråt i alla barn-mappar, exkludera bin/obj
        csprojFiles = GetAllCsprojFilesRecursive(path);
        if (csprojFiles.Count != 0)
        {
            if (verbose)
            {
                Writer.WriteVerbose($"Found {csprojFiles.Count} project(s)");
            }

            return csprojFiles;
        }

        // 3) Leta uppåt tills vi hittar en csproj-fil, returnera alla csproj i den mappen
        var dir = new DirectoryInfo(path);
        while (dir != null)
        {
            csprojFiles = Directory.EnumerateFiles(dir.FullName, "*.csproj", SearchOption.TopDirectoryOnly)
                .Where(p => !IsInIgnoredDir(p))
                .ToList();

            if (csprojFiles.Count != 0)
            {
                if (verbose)
                {
                    Writer.WriteVerbose($"Found {csprojFiles.Count} project(s)");
                }
                
                return csprojFiles;
            }

            dir = dir.Parent;
        }

        // 4) Leta uppåt tills vi hittar en sln-fil, plocka ut csproj-filer som finns i den
        dir = new DirectoryInfo(path);
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
                    .Where(p => File.Exists(p) && !IsInIgnoredDir(p))
                    .ToList();

                if (csprojPaths.Count != 0)
                {
                    if (verbose)
                    {
                        Writer.WriteVerbose($"Found {csprojPaths.Count} project(s)");
                    }
                    
                    return csprojFiles;
                }
            }

            dir = dir.Parent;
        }

        if (verbose)
        {
            Writer.WriteVerbose("No project(s) found");
        }

        return [];
    }
    
    public List<string> FindProjectFiles(ICommandable commandSettings)
    {
        return FindProjectFiles(commandSettings.Path, commandSettings.Verbose);
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
}