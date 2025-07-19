using System.Text.Json;
using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class CurrentLtsService
{
    private const string LtsVersionFallback = "8.0"; // todo: är detta så snyggt egentligen?
    private const string MetadataUrl = "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json";
    private static readonly string CacheDir = Path.Combine(Path.GetTempPath(), "Simple.ProjectAnalyzer.Cache");
    private static readonly string CacheFile = Path.Combine(CacheDir, "dotnet-lts-cache.json");
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<TargetFramework> GetCurrentLtsVersion()
    {
        // todo: vet inte ens om det är så nödvändigt att använda cache här, men det
        // sparar i alla fall in på ett anrop
        var releaseMetadata = MetadataExistIsCache()
            ? await GetMetadataFromCache()
            : await GetMetadataAndStoreInCache(MetadataUrl);
        
        var version = ExtractLtsVersionFromMetadata(releaseMetadata);

        return new TargetFramework
        {
            Alias = $"net{version}",
            Type = "net",
            Version = new Version(version)
        };
    }

    private static string ExtractLtsVersionFromMetadata(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var ltsChannel = root.GetProperty("releases-index")
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
            throw new ProjectAnalyzerException("No LTS version found in metadata");
        }

        return ltsChannel.GetProperty("channel-version").GetString() ?? LtsVersionFallback;
    }

    private static bool MetadataExistIsCache() 
        => File.Exists(CacheFile) && DateTime.UtcNow - File.GetLastWriteTimeUtc(CacheFile) < CacheDuration;

    private static async Task<string> GetMetadataAndStoreInCache(string metadataUrl)
    {
        using var http = new HttpClient();
        var json = await http.GetStringAsync(metadataUrl);

        Directory.CreateDirectory(CacheDir);
        await File.WriteAllTextAsync(CacheFile, json);
        
        return json;
    }

    private static async Task<string> GetMetadataFromCache()
    {
        return await File.ReadAllTextAsync(CacheFile);
    }
}