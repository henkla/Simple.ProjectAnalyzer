using System.Xml.Linq;

namespace Simple.ProjectAnalyzer.Domain.Analyzers
{
    public class ProjectTypeAnalyzer : AnalyzerBase
    {
        public override Task Run(ref AnalysisContext context)
        {
            foreach (var project in context.Projects)
            {
                var csprojPath = project.Path;
                if (!File.Exists(csprojPath))
                {
                    project.Sdk = "File not found";
                    continue;
                }

                try
                {
                    var xml = XDocument.Load(csprojPath);
                    var root = xml.Root;

                    if (root == null)
                    {
                        project.Sdk = "Invalid XML";
                        continue;
                    }

                    // 1. Kontrollera om det är SDK style (har "Sdk" attribut på Project-taggen)
                    var sdkAttribute = root.Attribute("Sdk");
                    if (sdkAttribute != null)
                    {
                        // Kan också kolla om det innehåller t.ex. Microsoft.NET.Sdk eller andra SDKs
                        project.Sdk = "SDK-style project";
                        continue;
                    }

                    // 2. Kolla namespace, om det är legacy (oftast: http://schemas.microsoft.com/developer/msbuild/2003)
                    XNamespace ns = root.Name.Namespace;
                    if (ns == "http://schemas.microsoft.com/developer/msbuild/2003")
                    {
                        // Kolla om det är VS Extension / VS SDK projekt genom att kolla ProjectTypeGuids
                        var projectTypeGuids = root
                            .Elements(ns + "PropertyGroup")
                            .Elements(ns + "ProjectTypeGuids")
                            .FirstOrDefault()?.Value;

                        if (!string.IsNullOrEmpty(projectTypeGuids))
                        {
                            project.Sdk = "Legacy VS SDK / VS Extension project";
                            continue;
                        }

                        // Kolla om det ser ut som legacy Web Application (exempelvis om det har <ProjectGuid> och <OutputType>)
                        var outputType = root
                            .Elements(ns + "PropertyGroup")
                            .Elements(ns + "OutputType")
                            .FirstOrDefault()?.Value;

                        if (!string.IsNullOrEmpty(outputType))
                        {
                            project.Sdk = "Legacy .NET Framework project";
                            continue;
                        }

                        // Default för legacy utan särskild ProjectTypeGuids
                        project.Sdk = "Legacy non-SDK project";
                        continue;
                    }

                    // Om det inte är någon av ovan, fallback
                    project.Sdk = "Unknown project type";

                }
                catch (Exception ex)
                {
                    project.Sdk = "Error reading project file: " + ex.Message;
                }
            }

            return Task.CompletedTask;
        }
    }
}
