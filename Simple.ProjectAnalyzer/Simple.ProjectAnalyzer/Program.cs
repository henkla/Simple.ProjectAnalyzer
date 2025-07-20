using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Simple.ProjectAnalyzer;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Git;
using Simple.ProjectAnalyzer.Domain.CommandLine.Commands.Local;
using Simple.ProjectAnalyzer.Domain.Extensions;
using Simple.ProjectAnalyzer.Domain.Utilities;
using Spectre.Console.Cli;

var services = new ServiceCollection().RegisterDomainServices();
var typeRegistrar = new TypeRegistrar(services);
var application = new CommandApp(typeRegistrar);

application.SetDefaultCommand<LocalCommand>()
    .WithDescription(Settings.ApplicationDescription);

application.Configure(config =>
{
    config.AddCommand<GitCommand>(GitCommand.Name)
        .WithDescription(GitCommand.Description);

    config.AddCommand<LocalCommand>(LocalCommand.Name)
        .WithDescription(LocalCommand.Description);

    config.TrimTrailingPeriods(true);
    config.UseAssemblyInformationalVersion();
    config.CaseSensitivity(CaseSensitivity.None);
    config.AddExample(Settings.ApplicationExample);
    config.SetApplicationName(Settings.ApplicationName);
    config.SetApplicationCulture(new CultureInfo(Settings.ApplicationCulture));
    config.SetExceptionHandler(ExceptionHandler.OnException);
});

return application.Run(args);