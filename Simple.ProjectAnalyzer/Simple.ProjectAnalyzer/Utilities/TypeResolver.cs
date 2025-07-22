using Spectre.Console.Cli;

namespace Simple.ProjectAnalyzer.Utilities;

public sealed class TypeResolver(IServiceProvider serviceProvider) : ITypeResolver
{
    public object? Resolve(Type? type)
    {
        return type is null 
            ? null 
            : serviceProvider.GetService(type);
    }

    public IEnumerable<object> ResolveAll(Type type)
    {
        var genericEnumerableType = typeof(IEnumerable<>).MakeGenericType(type);
        var services = (IEnumerable<object>?)serviceProvider.GetService(genericEnumerableType);
        return services ?? [];
    }
}