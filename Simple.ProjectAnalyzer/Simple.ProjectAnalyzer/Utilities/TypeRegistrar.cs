using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Utilities;

public sealed class TypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
    private IServiceProvider? _serviceProvider;

    public ITypeResolver Build()
    {
        if (_serviceProvider == null)
        {
            _serviceProvider = builder.BuildServiceProvider();
        }

        return new TypeResolver(_serviceProvider);
    }

    public void Register(Type service, Type implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        builder.AddSingleton(service, _ => factory());
    }
}