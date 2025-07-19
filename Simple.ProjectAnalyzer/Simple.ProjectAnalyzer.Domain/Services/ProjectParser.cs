using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services
{
    public class ProjectParser
    {
        public Project Parse(string projectFile)
        {
            if (!File.Exists(projectFile))
                throw new FileNotFoundException("Project file not found", projectFile);

            var xdoc = XDocument.Load(projectFile);
            var ns = xdoc.Root?.Name.Namespace ?? XNamespace.None;

            var name = Path.GetFileNameWithoutExtension(projectFile);

            var sdk = xdoc.Root?.Attribute("Sdk")?.Value?.Trim();

            var packageReferences = xdoc
                .Descendants(ns + "PackageReference")
                .Select(pr =>
                {
                    var include = pr.Attribute("Include")?.Value ?? string.Empty;
                    var versionAttr = pr.Attribute("Version")?.Value;
                    var versionElem = pr.Element(ns + "Version")?.Value;

                    return new PackageReference
                    {
                        Name = include,
                        Include = include,
                        Version = versionAttr ?? versionElem ?? string.Empty
                    };
                })
                .ToList();

            var projectReferences = xdoc
                .Descendants(ns + "ProjectReference")
                .Select(pr => pr.Attribute("Include")?.Value ?? string.Empty)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var references = xdoc
                .Descendants(ns + "Reference")
                .Select(r =>
                {
                    var type = r.Attribute("Include")?.Value ?? string.Empty;
                    var hintPath = r.Element(ns + "HintPath")?.Value ?? string.Empty;
                    var privateStr = r.Element(ns + "Private")?.Value ?? "false";

                    bool.TryParse(privateStr, out bool isPrivate);

                    return new Reference
                    {
                        Name = type,
                        HintPath = hintPath,
                        Private = isPrivate
                    };
                })
                .ToList();

            // Först leta efter TargetFrameworks / TargetFramework
            var tfElem = xdoc.Descendants(ns + "TargetFramework").FirstOrDefault();
            var tfsElem = xdoc.Descendants(ns + "TargetFrameworks").FirstOrDefault();

            List<TargetFramework> targetFrameworks = new();
            var projectFileIsOfLegacyType = false;
            
            if (tfsElem != null)
            {
                var rawTfs = tfsElem.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var rawTf in rawTfs)
                {
                    targetFrameworks.Add(ParseTargetFramework(rawTf.Trim()));
                }
            }
            else if (tfElem != null)
            {
                targetFrameworks.Add(ParseTargetFramework(tfElem.Value.Trim()));
            }
            else
            {
                // Om inte hittat TargetFramework(s), prova med UWP / äldre .NET projektelement
                projectFileIsOfLegacyType = true;
                var platformId = xdoc.Descendants(ns + "TargetPlatformIdentifier").FirstOrDefault()?.Value;
                var platformVersion = xdoc.Descendants(ns + "TargetPlatformVersion").FirstOrDefault()?.Value;
                var frameworkVersion = xdoc.Descendants(ns + "TargetFrameworkVersion").FirstOrDefault()?.Value;

                if (!string.IsNullOrEmpty(platformId) && !string.IsNullOrEmpty(platformVersion))
                {
                    // Exempel: uap10.0, windows10.0.19041.0
                    var combinedTf = platformId.Trim() + platformVersion.Trim();

                    targetFrameworks.Add(ParseTargetFramework(combinedTf));
                }
                else if (!string.IsNullOrEmpty(frameworkVersion))
                {
                    // Exempel: v4.7.2
                    targetFrameworks.Add(ParseTargetFramework(frameworkVersion.TrimStart('v', 'V')));
                }
                else
                {
                    throw new ProjectAnalyzerException("Unable to parse TargetFramework or platform identifiers");
                }
            }

            return new Project
            {
                Name = name,
                Path = projectFile,
                Sdk = sdk,
                IsLegacy = projectFileIsOfLegacyType,
                PackageReferences = packageReferences,
                ProjectReferences = projectReferences,
                References = references,
                TargetFrameworks = targetFrameworks
            };
        }

        public List<Project> ParseMany(List<string> projectFiles)
        {
            return projectFiles.Select(Parse).ToList();
        }

        private TargetFramework ParseTargetFramework(string rawTf)
        {
            var alias = rawTf;
            var version = ParseVersionFromTargetFramework(rawTf);
            var type = ParseFrameworkType(rawTf);
            return new TargetFramework { Alias = alias, Version = version, Type = type };
        }

        private string ParseFrameworkType(string tf)
        {
            tf = tf.ToLowerInvariant();

            if (tf.StartsWith("netstandard"))
                return "netstandard";

            if (tf.StartsWith("netcoreapp"))
                return "netcoreapp";

            if (tf.StartsWith("net") && Regex.IsMatch(tf, @"^net[1-4]\d{1,2}$"))
                return ".netframework";

            if (tf.StartsWith("net"))
                return "net"; // .NET 5+

            if (tf.StartsWith("uap"))
                return "uwp";

            if (tf.StartsWith("windows"))
                return "uwp";

            return "unknown";
        }

        private Version ParseVersionFromTargetFramework(string tf)
        {
            if (string.IsNullOrEmpty(tf))
                return new Version(0, 0);

            var mainPart = tf.Split('-')[0];

            // Ta bort prefix som "netstandard", "netcoreapp", "net", "uap", "windows"
            string versionPart = mainPart;
            if (mainPart.StartsWith("netstandard"))
                versionPart = mainPart.Substring("netstandard".Length);
            else if (mainPart.StartsWith("netcoreapp"))
                versionPart = mainPart.Substring("netcoreapp".Length);
            else if (mainPart.StartsWith("net"))
                versionPart = mainPart.Substring("net".Length);
            else if (mainPart.StartsWith("uap"))
                versionPart = mainPart.Substring("uap".Length);
            else if (mainPart.StartsWith("windows"))
                versionPart = mainPart.Substring("windows".Length);

            versionPart = versionPart.Trim('.', ' ');

            // Ta max 3 delar av versionen (för att undvika fel med tex 10.0.19041.0)
            var versionParts = versionPart.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (versionParts.Length > 3)
                versionParts = versionParts.Take(3).ToArray();

            if (versionParts.Length == 0)
                return new Version(0, 0);

            try
            {
                // Bygg version utifrån tillgängliga delar
                int major = 0, minor = 0, build = 0;

                if (versionParts.Length > 0)
                    major = int.Parse(versionParts[0]);
                if (versionParts.Length > 1)
                    minor = int.Parse(versionParts[1]);
                if (versionParts.Length > 2)
                    build = int.Parse(versionParts[2]);

                if (build > 0)
                    return new Version(major, minor, build);
                else
                    return new Version(major, minor);
            }
            catch
            {
                // fallback för specialfall, t.ex. 461 som betyder 4.6.1
                var digits = Regex.Match(versionPart, @"^\d{1,3}$");
                if (digits.Success)
                {
                    var val = digits.Value;
                    if (val.Length == 3)
                    {
                        return new Version(
                            int.Parse(val[0].ToString()),
                            int.Parse(val[1].ToString()),
                            int.Parse(val[2].ToString())
                        );
                    }
                    else if (val.Length == 2)
                    {
                        return new Version(
                            int.Parse(val[0].ToString()),
                            int.Parse(val[1].ToString())
                        );
                    }
                    else if (val.Length == 1)
                    {
                        return new Version(int.Parse(val[0].ToString()), 0);
                    }
                }
            }

            return new Version(0, 0);
        }
    }
}
