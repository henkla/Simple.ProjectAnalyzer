using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;

public class LocalCommand(LocalCommandHandler localCommandHandler) : AsyncCommand<LocalCommandSettings>, ICommand
{
    public static string Name => "local";
    public static string Description => "Analyze a local folder containing one or more .NET projects";
    
    public override async Task<int> ExecuteAsync(CommandContext commandContext, LocalCommandSettings commandSettings)
    {
        if (!commandSettings.IsValid(out var validationMessage))
        {
            throw new ProjectAnalyzerException("Invalid command settings: " + validationMessage);
        }

        if (commandSettings.Verbose)
        {
            Output.EnableVerboseOutput();
        }

        var context = await localCommandHandler.HandleCommand(commandSettings);

        return (int)context.ApplicationExitCode;
    }
}