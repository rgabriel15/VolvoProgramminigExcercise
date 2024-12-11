using System.Buffers;
using System.Diagnostics;
using System.Text.Json;
using Base.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace Base.Application.Services;
public sealed class CacheService : ICacheService
{
    #region Constants
    private readonly ILogger Logger;
    private readonly IDistributedCache Cache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(6);
    private static readonly List<string> Keys = [];
    #endregion

    #region Constructors
    public CacheService(ILogger logger
        , IDistributedCache cache)
    {
        Logger = logger;
        Cache = cache;
    }
    #endregion

    #region Methods
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await Cache.GetAsync(key, cancellationToken);

            if (!Keys.Contains(key))
            {
                if (bytes != null)
                {
                    await Cache.RemoveAsync(key: key, token: cancellationToken);
                    Debug.WriteLine($"CacheService Get: key [{key}] inconsistency.");
                }
                else
                {
                    Debug.WriteLine($"CacheService Get: key [{key}] not found.");
                }

                return default;
            }

            if (bytes == null)
            {
                _ = Keys.Remove(key);
                Debug.WriteLine($"CacheService Get: key [{key}] inconsistency.");
                return default;
            }

            Debug.WriteLine($"CacheService Get: key [{key}] found.");
            return Deserialize<T>(bytes);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "CacheService Get error: {Ex}");
            return default;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var removed = Keys.Remove(key);
            await Cache.RemoveAsync(key: key, token: cancellationToken);
            Debug.WriteLineIf(removed, $"CacheService Remove: key [{key}] removed.");
            Debug.WriteLineIf(!removed, $"CacheService Remove: key [{key}] not found.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "CacheService Remove error: {Ex}");
        }
    }

    public async Task SetAsync<T>(string key
        , T value
        , CancellationToken cancellationToken
        , TimeSpan? expiration = null)
    {
        try
        {
            var bytes = Serialize(value);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
            };

            await Cache.SetAsync(
                key: key
                , value: bytes
                , options: cacheOptions
                , token: cancellationToken);

            var containsKey = Keys.Contains(key);

            if (!containsKey)
            {
                Keys.Add(key);
            }

            Debug.WriteLineIf(!containsKey, $"CacheService Set: key [{key}] set.");
            Debug.WriteLineIf(containsKey, $"CacheService Set: key [{key}] updated.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "CacheService Set error: {Ex}");
        }
    }

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        var value = JsonSerializer.Deserialize<T>(bytes);
        return value!;
    }
    #endregion
}
