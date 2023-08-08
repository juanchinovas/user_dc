using Application.Common.Interfaces;
using Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace Application;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public MemoryCacheService(IMemoryCache cache) {
        _cache = cache;
    }

    public Task<object?> Get(string key)
    {
        object? result = null;
        if (!string.IsNullOrEmpty(key) )
        {
            _cache.TryGetValue(key, out result);
        }

        return Task.FromResult(result);
    }

    public Task Save(string key, object value)
    {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(key, value, cacheEntryOptions);

        return Task.CompletedTask;
    }

    public async Task<T> GetOrSave<T>(string key, Func<Task<T>> producer)
    {
        var cachedResult = await Get(key);
        if (cachedResult is null && producer is not null)
        {
            try
            {
                semaphore.Wait();
                var resultProduced = await producer();
                if (resultProduced != null) {
                    await Save(key, resultProduced);
                    return resultProduced;
                }

                }
            finally
            {
                semaphore.Release();
            }
        }

        return (T)cachedResult;
    }
}
