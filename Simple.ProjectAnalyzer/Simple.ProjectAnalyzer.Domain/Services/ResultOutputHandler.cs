using Simple.CaseConverter.Extensions;
using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Models;
using Spectre.Console;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class ResultOutputHandler
{
    public void PrintResultToConsole(Context context)
    {
        foreach (var project in context.Projects.OrderBy(p => p.Name))
        {
            AnsiConsole.WriteLine();
            var targetFrameworkAlias = project.TargetFrameworks
                .OrderByDescending(tf => tf.Version.Major)
                .FirstOrDefault()?.Alias ?? "Unknown";

            //      PROJECT
            
            var projectRoot = new Tree(project.Name);
            if (project.Sdk is not null)
            {
                projectRoot.AddNode("[dim]" + project.Sdk + "[/]");
            }
            projectRoot.AddNode("[dim]" + targetFrameworkAlias + "[/]");
            projectRoot.AddNode("[dim]" + project.Path + "[/]");
            

            var projectAnalysisRoot = new Tree("PROJECT ANALYSIS");
            foreach (var result in project.AnalysisResults)
            {
                var color = GetColorForResultCode(result.Code);
                projectAnalysisRoot.AddNode($"[dim]{result.Source,-45}[/][{color}]{result.Code.ToString(),-10}[/] {result.Message}");
            }

            projectRoot.AddNode(projectAnalysisRoot);

            //      PACKAGE REFERENCES 
            
            if (project.PackageReferences.Count > 0)
            {
                var packageAnalysisRoot = new Tree("PACKAGE ANALYSIS");
                foreach (var pkg in project.PackageReferences)
                {
                    var packageNode = packageAnalysisRoot.AddNode($"{pkg.Name} [dim]{pkg.Version}[/]");

                    foreach (var packageResult in pkg.AnalysisResults)
                    {
                        var color = GetColorForResultCode(packageResult.Code);
                        packageNode.AddNode($"[dim]{packageResult.Source,-40}[/] [{color}]{packageResult.Code.ToString(),-10}[/] {packageResult.Message}");
                    }
                }

                projectRoot.AddNode(packageAnalysisRoot);
            }
            
            //      REFERENCES 

            if (project.References.Count > 0)
            {
                var referenceAnalysisRoot = new Tree("REFERENCE ANALYSIS");
                foreach (var reference in project.References)
                {
                    var packageNode = referenceAnalysisRoot.AddNode($"{reference.Name} [dim]{reference.HintPath}[/]");

                    foreach (var referenceResult in reference.AnalysisResults)
                    {
                        var color = GetColorForResultCode(referenceResult.Code);
                        packageNode.AddNode($"[dim]{referenceResult.Source,-40}[/] [{color}]{referenceResult.Code.ToString(),-10}[/] {referenceResult.Message}");
                    }
                }

                projectRoot.AddNode(referenceAnalysisRoot);
            }
            
            AnsiConsole.Write(projectRoot);

            var errors = project.AnalysisResults.Count(r => r.Code == AnalysisResultCode.Error);
            var warnings = project.AnalysisResults.Count(r => r.Code == AnalysisResultCode.Warning);
            var suggestions = project.AnalysisResults.Count(r => r.Code == AnalysisResultCode.Hint);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]SUMMARY:[/]");

            if (errors > 0)
                AnsiConsole.MarkupLine($"[red]  {errors} error(s)[/]");
            if (warnings > 0)
                AnsiConsole.MarkupLine($"[yellow]  {warnings} warning(s)[/]");
            if (suggestions > 0)
                AnsiConsole.MarkupLine($"[blue]  {suggestions} hint(s)[/]");
            if (errors + warnings + suggestions == 0)
                AnsiConsole.MarkupLine("[green]  No errors, warnings or hints[/]");

            AnsiConsole.WriteLine();
        }

        AnsiConsole.WriteLine("Summary of analysis:");

        var projectResultGroupedByAnalysis = context.Projects
            .SelectMany(p => p.AnalysisResults)
            .Where(r => r.Code is not AnalysisResultCode.Ok)
            .GroupBy(r => r.Title)
            .ToList();

        var resultGroupedByAnalysis = projectResultGroupedByAnalysis.ToList();
        var projectResultGroupedByAnalysisCount = resultGroupedByAnalysis.Count;

        foreach (var (analysisGroup, index) in resultGroupedByAnalysis.Select((g, i) => (g, i)))
        {
            var color = GetColorForResultCode(analysisGroup.First().Code);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold {color}]{analysisGroup.Key.ToUpper()}[/] ({index + 1}/{projectResultGroupedByAnalysisCount})");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[underline]Details[/]:");
            AnsiConsole.MarkupLine($"{analysisGroup.First().Details}");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[underline]Recommendation[/]:");
            AnsiConsole.MarkupLine($"{analysisGroup.First().Recommendation}");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[underline]Project(s)[/]:");

            foreach (var result in analysisGroup.OrderBy(r => r.Source))
            {
                AnsiConsole.WriteLine($"- {result.Parent.Name}");
            }
        }
    }

    private string GetColorForResultCode(AnalysisResultCode code) => code switch
    {
        AnalysisResultCode.Error => "red",
        AnalysisResultCode.Warning => "yellow",
        AnalysisResultCode.Hint => "blue",
        _ => "green"
    };
}