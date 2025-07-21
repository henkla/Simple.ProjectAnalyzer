using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.CommandLine;

namespace Simple.ProjectAnalyzer.Domain.Analysis;

public class Orchestrator(IEnumerable<IAnalyzer> analyzers)
{
    public async Task AnalyzeProjects(Context context)
    {
        Output.Verbose($"{nameof(Orchestrator)}.{nameof(AnalyzeProjects)} started");

        var tasks = GetFilteredAnalyzers(context)
            .OrderBy(a => a.Name)
            .Select(a => a.Run(context));

        await Task.WhenAll(tasks);
    }

    private IEnumerable<IAnalyzer> GetFilteredAnalyzers(Context context)
    {
        var selectedAnalyzers = context.AnalyzeCommandSettings.Analyzers?
            .Select(name => name.Trim().ToLowerInvariant())
            .ToHashSet();

        if (selectedAnalyzers is null || selectedAnalyzers.Count == 0)
        {
            return analyzers.DistinctBy(a => a.GetType());
        }

        var filtered = analyzers
            .Where(a => selectedAnalyzers.Contains(a.Name.ToLowerInvariant()))
            .DistinctBy(a => a.GetType())
            .ToList();

        if (filtered.Count == 0)
        {
            Output.Error("None of the specified analyzers matched any available analyzers.");
            Output.Line("Run command: [purple on black]analyzers --list[/] to see all available analyzers.");
        }
        else
        {
            Output.Verbose($"Using the defined analyzers: {string.Join(", ", context.AnalyzeCommandSettings.Analyzers!)}");
        }

        return filtered;
    }
}