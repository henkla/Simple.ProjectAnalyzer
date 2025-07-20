using Simple.ProjectAnalyzer.Domain.Analysis;

namespace Simple.ProjectAnalyzer.Domain.CommandLine.Commands;

public interface IAnalyzeCommandHandler
{
    public Task<Context> HandleCommand(IAnalyzeCommandSettings analyzeCommandSettings);
}