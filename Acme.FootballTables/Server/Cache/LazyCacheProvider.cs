using Acme.FootballTables.Server.Utils;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace Acme.FootballTables.Server.Cache
{
    public class LazyCacheProvider : ICacheProvider
    {
        private readonly IAppCache cache;

        public LazyCacheProvider(IAppCache cache)
        {
            this.cache = cache;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> addCallback, int size)
        {
            return await cache.GetOrAddAsync(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(60);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                entry.Size = size;
                return Task.FromResult(addCallback());
            });
        }

        public async Task AddAsync<T>(string key, T value, int size)
        {
            cache.Add(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(60),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),      
                Size = size
            });
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            cache.Remove(key);
            await Task.CompletedTask;
        }
    }
}
