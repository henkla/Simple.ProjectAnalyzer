using System.Text.RegularExpressions;
using System.Xml.Linq;
using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services
{
    public partial class ProjectParser(IConsoleOutput console)
    {
        [GeneratedRegex(@"^\d{1,3}$")]
        private static partial Regex VersionPartRegex();

        [GeneratedRegex(@"^net[1-4]\d{1,2}$")]
        private static partial Regex FrameworkTypeRegex();

        public Project ParseProjectFile(string projectFile)
        {
            console.Verbose($"{nameof(ProjectParser)}.{nameof(ParseProjectFile)} started: {projectFile}");

            if (!File.Exists(projectFile))
            {
                throw new FileNotFoundException("Project file not found", projectFile);
            }

            var xDocument = XDocument.Load(projectFile);
            var xNamespace = xDocument.Root?.Name.Namespace ?? XNamespace.None;

            var name = Path.GetFileNameWithoutExtension(projectFile);
            var sdk = ParseSdk(xDocument);
            var langVersion = ParseLangVersion(xDocument, xNamespace);
            var nullableEnabled = ParseNullableProperty(xDocument, xNamespace);
            var implicitUsingsEnabled = ParseImplicitUsings(xDocument, xNamespace);
            var packageReferences = ParsePackageReferences(xDocument, xNamespace);
            var projectReferences = ParseProjectReferences(xDocument, xNamespace);
            var references = ParseReferences(xDocument, xNamespace);
            var targetFrameworks = ParseTargetFrameworks(xDocument, xNamespace, out var projectFileIsOfLegacyType);

            return new Project
            {
                Name = name,
                Path = projectFile,
                Sdk = sdk,
                LangVersion = langVersion,
                NullableEnabled = nullableEnabled,
                ImplicitUsingsEnabled = implicitUsingsEnabled,
                IsLegacy = projectFileIsOfLegacyType,
                PackageReferences = packageReferences,
                ProjectReferences = projectReferences,
                References = references,
                TargetFrameworks = targetFrameworks
            };
        }
        
        public List<Project> ParseProjectFiles(List<string> projectFiles)
        {
            return projectFiles.Select(ParseProjectFile).ToList();
        }

        private static bool? ParseNullableProperty(XDocument xDocument, XNamespace xNamespace)
        {
            var nullableElement = xDocument
                .Descendants(xNamespace + "Nullable")
                .FirstOrDefault();

            return nullableElement != null
                ? string.Equals(nullableElement.Value.Trim(), "enable", StringComparison.OrdinalIgnoreCase)
                : null;
        }

        private List<TargetFramework> ParseTargetFrameworks(XDocument xDocument, XNamespace xNamespace, out bool projectFileIsOfLegacyType)
        {
            projectFileIsOfLegacyType = false;
            var targetFrameworkRaw = xDocument.Descendants(xNamespace + "TargetFramework").FirstOrDefault();
            var targetFrameworksRaw = xDocument.Descendants(xNamespace + "TargetFrameworks").FirstOrDefault();

            var targetFrameworks = new List<TargetFramework>();

            if (targetFrameworksRaw is not null)
            {
                var targetFrameworksRawSplit = targetFrameworksRaw.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var targetFramework in targetFrameworksRawSplit)
                {
                    targetFrameworks.Add(ParseTargetFramework(targetFramework.Trim()));
                }
            }
            else if (targetFrameworkRaw is not null)
            {
                targetFrameworks.Add(ParseTargetFramework(targetFrameworkRaw.Value.Trim()));
            }
            else
            {
                // Om inte hittat TargetFramework(s), prova med UWP / äldre .NET projektelement
                projectFileIsOfLegacyType = true;

                var platformId = xDocument.Descendants(xNamespace + "TargetPlatformIdentifier").FirstOrDefault()?.Value;
                var platformVersion = xDocument.Descendants(xNamespace + "TargetPlatformVersion").FirstOrDefault()?.Value;
                var frameworkVersion = xDocument.Descendants(xNamespace + "TargetFrameworkVersion").FirstOrDefault()?.Value;

                if (!string.IsNullOrEmpty(platformId) && !string.IsNullOrEmpty(platformVersion))
                {
                    // Exempel: uap10.0, windows10.0.19041.0
                    var combinedTargetFramework = platformId.Trim() + platformVersion.Trim();
                    targetFrameworks.Add(ParseTargetFramework(combinedTargetFramework));
                }
                else if (!string.IsNullOrEmpty(frameworkVersion))
                {
                    // Exempel: v4.7.2
                    targetFrameworks.Add(ParseTargetFramework(frameworkVersion.TrimStart('v', 'V')));
                }
                else
                {
                    console.Error("Unable to parse TargetFramework or platform identifiers");
                }
            }

            return targetFrameworks;
        }

        private List<Reference> ParseReferences(XDocument xDocument, XNamespace xNamespace)
        {
            return xDocument
                .Descendants(xNamespace + "Reference")
                .Select(r =>
                {
                    var type = r.Attribute("Include")?.Value ?? string.Empty;
                    var hintPath = r.Element(xNamespace + "HintPath")?.Value ?? string.Empty;
                    var privateValue = r.Element(xNamespace + "Private")?.Value ?? "false";

                    if (bool.TryParse(privateValue, out var isPrivate))
                    {
                        console.Warning("Unable to parse private value for Reference");
                    }

                    return new Reference
                    {
                        Name = type,
                        HintPath = hintPath,
                        Private = isPrivate
                    };
                })
                .ToList();
        }

        private static List<string> ParseProjectReferences(XDocument xDocument, XNamespace xNamespace)
        {
            return xDocument
                .Descendants(xNamespace + "ProjectReference")
                .Select(pr => pr.Attribute("Include")?.Value ?? string.Empty)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        private static string? ParseSdk(XDocument xDocument)
        {
            return xDocument.Root?.Attribute("Sdk")?.Value.Trim();
        }

        private List<PackageReference> ParsePackageReferences(XDocument xDocument, XNamespace xNamespace)
        {
            return xDocument
                .Descendants(xNamespace + "PackageReference")
                .Select(pr =>
                {
                    var include = pr.Attribute("Include")?.Value ?? string.Empty;
                    var versionAttribute = pr.Attribute("Version")?.Value;
                    var versionElement = pr.Element(xNamespace + "Version")?.Value;

                    if (versionAttribute is null && versionElement is null)
                    {
                        console.Warning("Unable to parse PackageReference version");
                    }

                    return new PackageReference
                    {
                        Name = include,
                        Include = include,
                        Version = versionAttribute ?? versionElement ?? string.Empty
                    };
                })
                .ToList();
        }

        private TargetFramework ParseTargetFramework(string targetFrameworkRaw)
        {
            var version = ParseVersionFromTargetFramework(targetFrameworkRaw);
            var type = ParseFrameworkType(targetFrameworkRaw);
            return new TargetFramework
            {
                Alias = targetFrameworkRaw,
                Version = version,
                Type = type
            };
        }

        private string ParseFrameworkType(string targetFramework)
        {
            targetFramework = targetFramework.ToLowerInvariant();

            var frameworkType = targetFramework switch
            {
                not null when targetFramework.StartsWith("netstandard") => "netstandard",
                not null when targetFramework.StartsWith("netcoreapp") => "netcoreapp",
                not null when targetFramework.StartsWith("net") && FrameworkTypeRegex().IsMatch(targetFramework) => ".netframework",
                not null when targetFramework.StartsWith("net") => "net", // .NET 5+
                not null when targetFramework.StartsWith("uap") => "uwp",
                not null when targetFramework.StartsWith("windows") => "uwp",
                _ => null
            };

            if (frameworkType is null)
            {
                console.Error("Unable to parse TargetFramework type");
                return "unknown";
            }

            return frameworkType;
        }

        private Version ParseVersionFromTargetFramework(string targetFramework)
        {
            if (string.IsNullOrEmpty(targetFramework))
            {
                console.Error("Unable to parse version from null or empty TargetFramework");
                return new Version(0, 0);
            }

            var mainPart = targetFramework.Split('-')[0];
            var versionPart = mainPart switch
            {
                not null when mainPart.StartsWith("netstandard") => mainPart["netstandard".Length..],
                not null when mainPart.StartsWith("netcoreapp") => mainPart["netcoreapp".Length..],
                not null when mainPart.StartsWith("net") => mainPart["net".Length..],
                not null when mainPart.StartsWith("uap") => mainPart["uap".Length..],
                not null when mainPart.StartsWith("windows") => mainPart["windows".Length..],
                _ => mainPart
            };

            versionPart = versionPart?.Trim('.', ' ');

            // ta max 3 delar av versionen - för att undvika fel med tex 10.0.19041.0
            var versionParts = versionPart?.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (versionParts is { Length: > 3 })
            {
                versionParts = versionParts.Take(3).ToArray();
            }

            if (versionParts is { Length: 0 } or null)
            {
                console.Error("Unable to parse version from TargetFramework");
                return new Version(0, 0);
            }

            try
            {
                var major = 0;
                var minor = 0;
                var build = 0;

                if (versionParts.Length > 0)
                {
                    major = int.Parse(versionParts[0]);
                }

                if (versionParts.Length > 1)
                {
                    minor = int.Parse(versionParts[1]);
                }

                if (versionParts.Length > 2)
                {
                    build = int.Parse(versionParts[2]);
                }

                return build > 0
                    ? new Version(major, minor, build)
                    : new Version(major, minor);
            }
            catch
            {
                // fallback för specialfall, t.ex. 461 som betyder 4.6.1
                var digits = VersionPartRegex().Match(versionPart!);
                if (digits.Success)
                {
                    var value = digits.Value;
                    switch (value.Length)
                    {
                        case 3:
                            return new Version(
                                int.Parse(value[0].ToString()),
                                int.Parse(value[1].ToString()),
                                int.Parse(value[2].ToString())
                            );
                        case 2:
                            return new Version(
                                int.Parse(value[0].ToString()),
                                int.Parse(value[1].ToString())
                            );
                        case 1:
                            return new Version(int.Parse(value[0].ToString()), 0);
                    }
                }
            }

            console.Error("Unable to parse version from TargetFramework");
            return new Version(0, 0);
        }
        
        private static string? ParseLangVersion(XDocument xDocument, XNamespace xNamespace)
        {
            var langVersionElement = xDocument
                .Descendants(xNamespace + "LangVersion")
                .FirstOrDefault();

            return langVersionElement?.Value.Trim();
        }
        
        private static bool? ParseImplicitUsings(XDocument xDocument, XNamespace xNamespace)
        {
            var implicitUsingsElement = xDocument
                .Descendants(xNamespace + "ImplicitUsings")
                .FirstOrDefault();

            return implicitUsingsElement != null
                ? string.Equals(implicitUsingsElement.Value.Trim(), "enable", StringComparison.OrdinalIgnoreCase)
                : null;
        }
    }
}