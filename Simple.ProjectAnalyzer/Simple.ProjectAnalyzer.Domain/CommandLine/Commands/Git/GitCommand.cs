using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;

public class GitCommand(GitCommandHandler gitCommandHandler) : AsyncCommand<GitCommandSettings>, ICommand
{
    public static string Name => "git";
    public static string Description => "Analyze .NET projects directly from a Git repository";

    public override async Task<int> ExecuteAsync(CommandContext commandContext, GitCommandSettings commandSettings)
    {
        if (!commandSettings.IsValid(out var validationMessage))
        {
            throw new ProjectAnalyzerException("Invalid command settings: " + validationMessage);
        }

        if (commandSettings.Verbose)
        {
            Output.EnableVerboseOutput();
        }
        
        
        var context = await gitCommandHandler.HandleCommand(commandSettings);

        return (int)context.ApplicationExitCode;
    }


}