using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class CurrentLtsService
{
    public async Task<CurrentLtsVersion> GetCurrentLtsVersion()
    {
        // todo: fallback från Simple.ProjectAnalyzer.Domain.Constants.CurrentLtsVersionFallback
        return new CurrentLtsVersion
        {
            Alias = "net8.0",
            Version = new Version("8.0"),
        };
    }
}