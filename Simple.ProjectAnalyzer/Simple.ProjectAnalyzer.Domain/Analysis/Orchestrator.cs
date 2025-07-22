using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Orchestrator(IConsoleOutput console, IEnumerable<IAnalyzer> analyzers)
{
    public async Task AnalyzeProjects(Context context)
    {
        console.Verbose($"{nameof(Orchestrator)}.{nameof(AnalyzeProjects)} started");

        var tasks = GetFilteredAnalyzers(context)
            .OrderBy(a => a.GetManifest().Name)
            .Select(a => a.Run(context));

        await Task.WhenAll(tasks);
    }

    private IEnumerable<IAnalyzer> GetFilteredAnalyzers(Context context)
    {
        var selectedAnalyzers = context.Analyzers?
            .Select(name => name.Trim().ToLowerInvariant())
            .ToHashSet();

        if (selectedAnalyzers is null || selectedAnalyzers.Count == 0)
        {
            return analyzers.DistinctBy(a => a.GetType());
        }

        var filtered = analyzers
            .Where(a => selectedAnalyzers.Contains(a.GetManifest().Name.ToLowerInvariant()))
            .DistinctBy(a => a.GetType())
            .ToList();

        if (filtered.Count == 0)
        {
            console.Error("None of the specified analyzers matched any available analyzers.");
            console.Line("Run command: [purple on black]analyzers --list[/] to see all available analyzers.");
        }
        else
        {
            console.Verbose($"Using the defined analyzers: {string.Join(", ", context.Analyzers!)}");
        }

        return filtered;
    }
}