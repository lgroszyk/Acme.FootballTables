using Microsoft.Extensions.Caching.Memory;

namespace Acme.FootballTables.Server.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache memoryCache;
        private readonly object cacheLockObject;

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            cacheLockObject = new object();
        }

        public T GetOrAdd<T>(string key, Func<T> addCallback)
        {
            T value;
            memoryCache.TryGetValue(key, out value);
            if (value != null)
            {
                return value;
            }

            lock (cacheLockObject)
            {
                memoryCache.TryGetValue(key, out value);
                if (value == null)
                {
                    value = addCallback();
                    memoryCache.Set(key, value, new MemoryCacheEntryOptions
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
                memoryCache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(60)
                });
            }
        }
    }
}
