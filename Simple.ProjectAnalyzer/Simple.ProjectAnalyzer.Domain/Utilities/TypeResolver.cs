using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.Utilities;

public sealed class TypeResolver(ServiceProvider provider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type) => type == null
        ? null
        : provider.GetService(type);

    public void Dispose()
    {
        provider.Dispose();
    }
}