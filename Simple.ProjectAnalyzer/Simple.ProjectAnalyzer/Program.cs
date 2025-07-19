using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands;
using Simple.ProjectAnalyzer.Domain.Extensions;
using Simple.ProjectAnalyzer.Domain.Utilities;
using Spectre.Console.Cli;

var services = new ServiceCollection()
    .RegisterDomainServices();

var registrar = new TypeRegistrar(services);
var application = new CommandApp(registrar);

application.SetDefaultCommand<AnalyzeCommand>()
    .WithDescription(AnalyzeCommand.Description);
    
application.Configure(config =>
{
    config.SetApplicationName(Constants.AppName);
    config.SetExceptionHandler(ExceptionHandler.OnException);
});

return application.Run(args);