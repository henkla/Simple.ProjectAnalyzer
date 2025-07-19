using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class CurrentLtsService
{
    public async Task<TargetFramework> GetCurrentLtsVersion()
    {
        // todo: handle fallback somehow if fetching of real LTS doesn't work 
        return new TargetFramework
        {
            Alias = "net8.0",
            Type = "net",
            Version = new Version("8.0"),
        };
    }
}