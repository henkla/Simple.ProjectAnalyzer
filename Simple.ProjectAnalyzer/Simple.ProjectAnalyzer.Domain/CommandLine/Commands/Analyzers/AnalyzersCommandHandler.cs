using Simple.ProjectAnalyzer.Domain.Analysis.Analyzers;
using Simple.ProjectAnalyzer.Domain.Services;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Analyzers;

public class AnalyzersCommandHandler(IEnumerable<IAnalyzer> analyzers, OutputHandler outputHandler)
{
    public async Task HandleCommand(AnalyzersCommandSettings commandSettings)
    {
        
        if (commandSettings.List)
        {
            Output.Figlet("Available analyzers");
            outputHandler.AnalyzersListToConsole(analyzers);
        }
    }
}