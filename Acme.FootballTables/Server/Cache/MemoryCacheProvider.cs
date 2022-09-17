using Acme.FootballTables.Server.Utils;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace Acme.FootballTables.Server.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache cache;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public MemoryCacheProvider(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> addCallback, int size)
        {
            await semaphore.WaitAsync();
            try
            {
                return await cache.GetOrCreateAsync<T>(key, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromSeconds(60);
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    entry.Size = size;
                    return Task.FromResult(addCallback());
                });
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task AddAsync<T>(string key, T value, int size)
        {
            await semaphore.WaitAsync();
            try
            {
                cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(60),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    Size = size,
                });
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task RemoveAsync(string key)
        {
            await semaphore.WaitAsync();
            try
            {
                cache.Remove(key);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
