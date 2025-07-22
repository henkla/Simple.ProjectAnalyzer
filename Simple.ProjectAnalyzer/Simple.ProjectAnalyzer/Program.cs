using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer.CommandLine.Commands.Analyze;
using Simple.ProjectAnalyzer.CommandLine.Commands.List;
using Simple.ProjectAnalyzer.Domain.Extensions;
using Simple.ProjectAnalyzer.Extensions;
using Simple.ProjectAnalyzer.Utilities;
using Spectre.Console.Cli;

var services = new ServiceCollection()
    .AddDomainServices()
    .AddCommandHandlers()
    .AddConsoleOutput();

var typeRegistrar = new TypeRegistrar(services);
var application = new CommandApp(typeRegistrar);

application.SetDefaultCommand<AnalyzeCommand>()
    .WithDescription(ApplicationSettings.ApplicationDescription);

application.Configure(config =>
{
    config.AddCommand<ListCommand>(ListCommand.Name)
        .WithDescription(ListCommand.Description);    
    
    config.AddCommand<AnalyzeCommand>(AnalyzeCommand.Name)
        .WithDescription(AnalyzeCommand.Description);

    config.TrimTrailingPeriods(true);
    config.UseAssemblyInformationalVersion();
    config.CaseSensitivity(CaseSensitivity.None);
    config.AddExample(ApplicationSettings.ApplicationExample);
    config.SetApplicationName(ApplicationSettings.ApplicationName);
    config.SetApplicationCulture(new CultureInfo(ApplicationSettings.ApplicationCulture));
});

return application.Run(args);