// using System.Xml.Linq;
//
// namespace Simple.ProjectAnalyzer.Domain.Analyzers;
//
// public class TargetFrameworkAnalyzer : AnalyzerBase
// {
//     public override Task Run(ref AnalysisContext context)
//     {
//         foreach (var project in context.Projects)
//         {
//             var csprojPath = project.Path;
//             if (!File.Exists(csprojPath))
//             {
//                 throw new ProjectAnalyzerException($"Unable to find project file: {csprojPath}");
//             }
//
//             var xml = XDocument.Load(csprojPath);
//             var ns = xml.Root?.GetDefaultNamespace() ?? XNamespace.None;
//
//             // 1) SDK-stil
//             var targetFramework = xml.Root?
//                 .Elements(ns + "PropertyGroup")
//                 .Elements()
//                 .FirstOrDefault(e => e.Name.LocalName == "TargetFramework" || e.Name.LocalName == "TargetFrameworks")
//                 ?.Value;
//
//             if (!string.IsNullOrWhiteSpace(targetFramework))
//             {
//                 var version = targetFramework.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
//                 project.TargetFrameworks = version;
//                 continue;
//             }
//
//             // 2) Legacy platform identifier + version
//             var targetPlatformIdentifier = xml.Root?
//                 .Elements(ns + "PropertyGroup")
//                 .Elements()
//                 .FirstOrDefault(e => e.Name.LocalName == "TargetPlatformIdentifier")
//                 ?.Value;
//
//             var targetPlatformVersion = xml.Root?
//                 .Elements(ns + "PropertyGroup")
//                 .Elements()
//                 .FirstOrDefault(e => e.Name.LocalName == "TargetPlatformVersion")
//                 ?.Value;
//
//             if (!string.IsNullOrWhiteSpace(targetPlatformIdentifier) && !string.IsNullOrWhiteSpace(targetPlatformVersion))
//             {
//                 project.TargetFrameworks = targetPlatformIdentifier + targetPlatformVersion;
//                 continue;
//             }
//
//             // 3) Prova TargetFrameworkVersion (för .NET Framework-projekt)
//             var targetFrameworkVersionRaw = xml.Root?
//                 .Elements(ns + "PropertyGroup")
//                 .Elements()
//                 .FirstOrDefault(e => e.Name.LocalName == "TargetFrameworkVersion")
//                 ?.Value;
//
//             if (!string.IsNullOrWhiteSpace(targetFrameworkVersionRaw))
//             {
//                 var cleaned = targetFrameworkVersionRaw.TrimStart('>', ' '); // Rensa felaktig '>'
//                 project.TargetFrameworks = cleaned;
//                 continue;
//             }
//
//             // 4) Om ProjectTypeGuids finns kan vi försöka markera t.ex. VS SDK
//             var projectTypeGuids = xml.Root?
//                 .Elements(ns + "PropertyGroup")
//                 .Elements()
//                 .FirstOrDefault(e => e.Name.LocalName == "ProjectTypeGuids")
//                 ?.Value;
//
//             if (!string.IsNullOrWhiteSpace(projectTypeGuids))
//             {
//                 project.TargetFrameworks = $"ProjectTypeGuids: {projectTypeGuids}";
//                 continue;
//             }
//
//             throw new ProjectAnalyzerException($"Could not determine project version for: {csprojPath}");
//         }
//
//         return Task.CompletedTask;
//     }
// }