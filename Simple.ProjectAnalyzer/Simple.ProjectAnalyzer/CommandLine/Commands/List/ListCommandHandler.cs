using Simple.ProjectAnalyzer.Abstractions.CommandLine;
using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.CommandLine.Commands.List;

public class ListCommandHandler(
    IEnumerable<IAnalyzer> analyzers,
    IConsoleOutput console,
    OutputHandler outputHandler
    )
{
    public async Task HandleCommand(ListCommandSettings commandSettings)
    {
        
        if (commandSettings.Analyzers)
        {
            console.Figlet("Available analyzers");
            outputHandler.AnalyzersListToConsole(analyzers);
        }
    }
}