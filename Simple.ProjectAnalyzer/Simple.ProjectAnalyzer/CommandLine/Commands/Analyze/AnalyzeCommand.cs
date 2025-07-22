using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.CommandLine.Commands.Analyze;

public class AnalyzeCommand(AnalyzeCommandHandler handler, IConsoleOutput console) : AsyncCommand<AnalyzeCommandSettings>
{
    public static string Name => "analyze";
    public static string Description => "Analyze .NET projects on local machine or from a Git repository (default command)";

    public override async Task<int> ExecuteAsync(CommandContext context, AnalyzeCommandSettings settings)
    {
        if (!settings.IsValid(out var validationMessage))
        {
            throw new ProjectAnalyzerException("Invalid command settings: " + validationMessage);
        }

        if (settings.Verbose)
        {
            console.EnableVerboseOutput();
        }
        
        
        var result = await handler.HandleCommand(settings);

        return result;
    }


}