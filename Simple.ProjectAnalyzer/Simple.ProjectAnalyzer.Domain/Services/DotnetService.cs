using System.Text.Json;
using Simple.ProjectAnalyzer.Abstractions.Output;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class DotnetService(IConsoleOutput console)
{
    private const string MetadataUrl = "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json";
    private const string LtsVersionFallback = "8.0"; // todo: är detta så snyggt egentligen?
    private const string AppCacheDirectoryName = "Simple.ProjectAnalyzer.Cache";
    private const string AppCacheFileName = "dotnet-lts-cache.json";
    private const int AppCacheDurationInMinutes = 30; // todo: vad är rimligt?

    public async Task<TargetFramework> GetCurrentLtsVersion()
    {
        console.Verbose($"{nameof(DotnetService)}.{nameof(GetCurrentLtsVersion)} started");
        console.Verbose($"Metadata url: {MetadataUrl}");
        console.Verbose($"Cache directory name: {AppCacheDirectoryName}");
        console.Verbose($"Cache file name: {AppCacheFileName}");
        console.Verbose($"Cache duration: {AppCacheDurationInMinutes} minute(s)");
            
        var cacheDuration = TimeSpan.FromMinutes(AppCacheDurationInMinutes); 
        var cacheDirectory = Path.Combine(Path.GetTempPath(), AppCacheDirectoryName);
        var cacheFile = Path.Combine(cacheDirectory, AppCacheFileName);
        
        // todo: vet inte ens om det är så nödvändigt att använda cache här, men det
        // sparar i alla fall in på ett anrop
        var releaseMetadata = MetadataExistIsCache(cacheFile, cacheDuration)
            ? await GetMetadataFromCache(cacheFile)
            : await GetMetadataAndStoreInCache(MetadataUrl, cacheDirectory, cacheFile);

        var ltsVersion = ExtractLtsVersionFromMetadata(releaseMetadata);
        console.Verbose($"Latest .NET LTS version: net{ltsVersion}");
        
        return new TargetFramework
        {
            Alias = $"net{ltsVersion}",
            Type = "net",
            Version = new Version(ltsVersion)
        };
    }

    private string ExtractLtsVersionFromMetadata(string json)
    {
        using var jsonDocument = JsonDocument.Parse(json);
        var rootElement = jsonDocument.RootElement;

        var ltsChannel = rootElement.GetProperty("releases-index")
            .EnumerateArray()
            .Where(e =>
                e.TryGetProperty("release-type", out var typeProp) &&
                typeProp.GetString()?.Equals("lts", StringComparison.OrdinalIgnoreCase) == true &&
                e.TryGetProperty("support-phase", out var phaseProp) &&
                !string.Equals(phaseProp.GetString(), "eol", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(phaseProp.GetString(), "preview", StringComparison.OrdinalIgnoreCase)
            )
            .OrderByDescending(e => Version.Parse(e.GetProperty("channel-version").GetString()!))
            .FirstOrDefault();

        if (ltsChannel.ValueKind == JsonValueKind.Undefined)
        {
            console.Warning($"No LTS version found in metadata - using fallback value: {LtsVersionFallback}");
        }

        if (ltsChannel.GetProperty("channel-version").GetString() is null)
        {
            console.Warning($"No LTS version found in metadata - using fallback value: {LtsVersionFallback}");
        }

        return ltsChannel.GetProperty("channel-version").GetString()!;
    }

    private static bool MetadataExistIsCache(string cacheFile, TimeSpan cacheDuration) =>
        File.Exists(cacheFile) && DateTime.UtcNow - File.GetLastWriteTimeUtc(cacheFile) < cacheDuration;

    private async Task<string> GetMetadataAndStoreInCache(string metadataUrl, string cacheDirectory, string cacheFile)
    {
        console.Verbose("Fetching metadata and saving in cache");
        
        using var http = new HttpClient();
        var json = await http.GetStringAsync(metadataUrl);

        Directory.CreateDirectory(cacheDirectory);
        await File.WriteAllTextAsync(cacheFile, json);

        return json;
    }

    private static async Task<string> GetMetadataFromCache(string cacheFile) => 
        await File.ReadAllTextAsync(cacheFile);
}