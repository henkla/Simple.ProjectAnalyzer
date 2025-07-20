using System.Net.Mime;
using Simple.ProjectAnalyzer.Domain.Models;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Analyzers;

public class AnalyzersCommand(AnalyzersCommandHandler commandHandler) : AsyncCommand<AnalyzersCommandSettings>, ICommand
{
    public static string Name => "analyzers";
    public static string Description => "Perform operations on the available analyzers.";
    
    public override async Task<int> ExecuteAsync(CommandContext context, AnalyzersCommandSettings settings)
    {
        await commandHandler.HandleCommand(settings);

        return (int)ApplicationExitCode.Ok;
    }
}