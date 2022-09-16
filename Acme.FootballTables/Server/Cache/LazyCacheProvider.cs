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

        public T GetOrAdd<T>(string key, Func<T> addCallback)
        {
            return cache.GetOrAdd(key, addCallback, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(60)
            });
        }

        public void Add<T>(string key, T value)
        {
            cache.Add(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(60)
            });
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}
