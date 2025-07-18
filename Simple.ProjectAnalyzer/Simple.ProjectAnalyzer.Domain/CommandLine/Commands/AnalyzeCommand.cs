using Simple.ProjectAnalyzer.Domain.Analysis;
using Simple.ProjectAnalyzer.Domain.Models;
using Simple.ProjectAnalyzer.Domain.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public class AnalyzeCommand(
    ProjectParser projectParser,
    Orchestrator orchestrator,
    ProjectFinder projectFinder) : AsyncCommand<AnalyzeCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, AnalyzeCommandSettings commandSettings)
    {
        if (!commandSettings.AreValid(out var validationMessage))
        {
            throw new ProjectAnalyzerException("Invalid project settings: " + validationMessage);
        }

        var projectFiles = projectFinder.FindProjectFiles(commandSettings);
        var projects = projectParser.ParseMany(projectFiles);
        var analysisContext = await orchestrator.AnalyzeProjects(projects, commandSettings);

        await WriteResults(analysisContext, commandSettings);

        return (int)analysisContext.ExitCode;
    }

    private async Task WriteResults(Context context, AnalyzeCommandSettings commandSettings)
    {
        foreach (var project in context.Projects.OrderBy(p => p.Name))
        {
            AnsiConsole.WriteLine();
            var targetFrameworkAlias = project.TargetFrameworks
                .OrderByDescending(tf => tf.Version.Major)
                .FirstOrDefault()?.Alias ?? "Unknown";

            AnsiConsole.MarkupLine($"[bold]Project:[/] {project.Name} ({targetFrameworkAlias})");
            AnsiConsole.MarkupLine($"[dim]Path:[/] {project.Path}");
            AnsiConsole.WriteLine();

            foreach (var result in project.AnalysisResults)
            {
                var color = GetColorForResultCode(result.Code);
                AnsiConsole.MarkupLine($"[{color}]- {result.Source}: <{result.Code}> {result.Message}[/]");
            }

            if (project.PackageReferences.Count > 0)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[underline]Packages:[/]");
                foreach (var pkg in project.PackageReferences)
                {
                    AnsiConsole.MarkupLine($"- {pkg.Name} [dim]v{pkg.Version}[/]");

                    foreach (var packageResult in pkg.AnalysisResults)
                    {
                        var color = GetColorForResultCode(packageResult.Code);
                        AnsiConsole.MarkupLine($"[{color}]  {packageResult.Source}: <{packageResult.Code}> - {packageResult.Message}[/]");
                    }
                }
            }

            var errors = project.AnalysisResults.Count(r => r.Code == ResultCode.Error);
            var warnings = project.AnalysisResults.Count(r => r.Code == ResultCode.Warning);
            var suggestions = project.AnalysisResults.Count(r => r.Code == ResultCode.Suggestion);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Summary:[/]");

            if (errors > 0)
                AnsiConsole.MarkupLine($"[red]  {errors} error(s)[/]");
            if (warnings > 0)
                AnsiConsole.MarkupLine($"[yellow]  {warnings} warning(s)[/]");
            if (suggestions > 0)
                AnsiConsole.MarkupLine($"[cyan2]  {suggestions} suggestion(s)[/]");

            if (errors + warnings + suggestions == 0)
                AnsiConsole.MarkupLine("[green]  Project looks good![/]");

            AnsiConsole.WriteLine();
        }
        
        AnsiConsole.WriteLine("Summary of analysis:");

        var projectResultGroupedByAnalysis = context.Projects
            .SelectMany(p => p.AnalysisResults)
            .Where(r => r.Code is not ResultCode.Ok)
            .GroupBy(r => r.Source);

        foreach (var analysisGroup in projectResultGroupedByAnalysis)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"{analysisGroup.Key}");
                
            AnsiConsole.WriteLine($"{analysisGroup.First().Details}");
                
            foreach (var result in analysisGroup.OrderBy(r => r.Source))
            {
                AnsiConsole.WriteLine($"- {result.Parent.Name}");
            }
        }
    }

    private string GetColorForResultCode(ResultCode code) => code switch
    {
        ResultCode.Error => "red",
        ResultCode.Warning => "yellow",
        ResultCode.Suggestion => "cyan2",
        _ => "green"
    };
}