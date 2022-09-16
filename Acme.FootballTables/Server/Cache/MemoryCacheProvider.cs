using Microsoft.Extensions.Caching.Memory;

namespace Acme.FootballTables.Server.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache cache;
        private readonly object cacheLockObject;

        public MemoryCacheProvider(IMemoryCache cache)
        {
            this.cache = cache;
            cacheLockObject = new object();
        }

        public T GetOrAdd<T>(string key, Func<T> addCallback)
        {
            T value;
            cache.TryGetValue(key, out value);
            if (value != null)
            {
                return value;
            }

            lock (cacheLockObject)
            {
                cache.TryGetValue(key, out value);
                if (value == null)
                {
                    value = addCallback();
                    cache.Set(key, value, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(60)
                    });
                }
            }

            return value;
        }

        public void Add<T>(string key, T value)
        {
            lock (cacheLockObject)
            {
                cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(60)
                });
            }
        }

        public void Remove(string key)
        {
            lock (cacheLockObject)
            {
                cache.Remove(key);
            }
        }
    }
}
