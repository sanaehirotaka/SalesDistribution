using SalesDistribution.Models;
using System.IO.Compression;
using System.Text.Json;

namespace SalesDistribution.Services;

public class Serializer
{
    private readonly OptionsModel options;

    public Serializer(OptionsModel options)
    {
        this.options = options;
    }

    public async Task<T?> ReadAsync<T>() where T : IModelBase
    {
        string path = GetPath<T>();
        if (!File.Exists(path))
        {
            return default;
        }
        try
        {
            using var stream = new GZipStream(File.OpenRead(path), CompressionMode.Decompress);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
        catch
        {
            return default;
        }
    }

    public async Task<bool> WriteAsync<T>(T value) where T : IModelBase
    {
        var nowVersion = await ReadAsync<T>();
        if (nowVersion != null && nowVersion.Version != value.Version)
        {
            return false;
        }
        try
        {
            value.Version ^= Random.Shared.Next();
            using var stream = new GZipStream(File.OpenWrite(GetPath<T>()), CompressionMode.Compress);
            JsonSerializer.Serialize(stream, value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetPath<T>()
    {
        return Path.Combine(options.DataLocation, $"{typeof(T).Name}.json.gz");
    }
}
