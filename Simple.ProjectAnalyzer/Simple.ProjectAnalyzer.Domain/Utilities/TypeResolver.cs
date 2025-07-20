using Simple.ProjectAnalyzer.Domain.CommandLine;
using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Domain.Utilities;

public sealed class TypeResolver(IServiceProvider serviceProvider) : ITypeResolver
{
    public object? Resolve(Type? type)
    {
        if (type is null)
        {
            Output.Error("TypeResolver.Resolve: type is null");
            return null;
        }
        
        return serviceProvider.GetService(type);
    }

    public IEnumerable<object> ResolveAll(Type type)
    {
        var genericEnumerableType = typeof(IEnumerable<>).MakeGenericType(type);
        var services = (IEnumerable<object>?)serviceProvider.GetService(genericEnumerableType);
        return services ?? [];
    }
}