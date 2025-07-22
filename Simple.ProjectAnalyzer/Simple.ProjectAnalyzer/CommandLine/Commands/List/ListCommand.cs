using Simple.ProjectAnalyzer.Domain.Models;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.CommandLine.Commands.List;

public class ListCommand(ListCommandHandler commandHandler) : AsyncCommand<ListCommandSettings>
{
    public static string Name => "list";
    public static string Description => "Perform operations on the available analyzers.";
    
    public override async Task<int> ExecuteAsync(CommandContext context, ListCommandSettings settings)
    {
        await commandHandler.HandleCommand(settings);

        return (int)ApplicationExitCode.Ok;
    }
}