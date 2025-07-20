using Simple.ProjectAnalyzer.Domain.Analysis;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public interface ICommandHandler
{
    public Task<Context> HandleCommand(ICommandSettings commandSettings);
}