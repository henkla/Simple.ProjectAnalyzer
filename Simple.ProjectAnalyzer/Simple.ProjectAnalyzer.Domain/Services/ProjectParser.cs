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

            // Sdk-attributet på Project-taggen
            var sdk = xdoc.Root?.Attribute("Sdk")?.Value?.Trim();

            // PackageReferences
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

            // ProjectReferences (lista av Include-attribut)
            var projectReferences = xdoc
                .Descendants(ns + "ProjectReference")
                .Select(pr => pr.Attribute("Include")?.Value ?? string.Empty)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            // References - komplex typ, vi hämtar Type, HintPath och Private
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
                        Type = type,
                        HintPath = hintPath,
                        Private = isPrivate
                    };
                })
                .ToList();

            // TargetFrameworks (kan finnas som TargetFramework eller TargetFrameworks med semikolonseparerad lista)
            var tfElem = xdoc.Descendants(ns + "TargetFramework").FirstOrDefault();
            var tfsElem = xdoc.Descendants(ns + "TargetFrameworks").FirstOrDefault();

            List<TargetFramework> targetFrameworks = new();

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

            return new Project
            {
                Name = name,
                Path = projectFile,
                Sdk = sdk,
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
            return new TargetFramework { Alias = alias, Version = version };
        }

        private Version ParseVersionFromTargetFramework(string tf)
        {
            if (string.IsNullOrEmpty(tf))
                return new Version(0, 0);

            // Ta bort eventuella suffix efter bindestreck, t.ex. net7.0-ios -> net7.0
            var mainPart = tf.Split('-')[0];

            // Ta bort prefix som "net", "netstandard", "netcoreapp"
            string versionPart = mainPart;
            if (mainPart.StartsWith("netstandard"))
                versionPart = mainPart.Substring("netstandard".Length);
            else if (mainPart.StartsWith("netcoreapp"))
                versionPart = mainPart.Substring("netcoreapp".Length);
            else if (mainPart.StartsWith("net"))
                versionPart = mainPart.Substring("net".Length);

            versionPart = versionPart.Trim('.', ' ');

            // Om versionPart innehåller punkt
            if (versionPart.Contains('.'))
            {
                var parts = versionPart.Split('.');
                int major = 0, minor = 0, build = 0;

                if (parts.Length > 0 && int.TryParse(parts[0], out int maj))
                    major = maj;
                if (parts.Length > 1 && int.TryParse(parts[1], out int min))
                    minor = min;
                if (parts.Length > 2 && int.TryParse(parts[2], out int bld))
                    build = bld;

                if (build > 0)
                    return new Version(major, minor, build);
                else
                    return new Version(major, minor);
            }
            else
            {
                // Om inga punkter, kan vara som 461 = 4.6.1
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

                if (int.TryParse(versionPart, out int majorOnly))
                    return new Version(majorOnly, 0);
            }

            return new Version(0, 0);
        }
    }
}
