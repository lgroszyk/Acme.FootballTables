using Acme.FootballTables.Server.Utils;

namespace Acme.FootballTables.Server.Cache
{
    public class CacheContext
    {
        private readonly ICacheProvider cacheProvider;

        public CacheContext(ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> addCallback)
        {
            return await GetOrAddAsync(key, addCallback, 1);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> addCallback, int size)
        {
            return await cacheProvider.GetOrAddAsync<T>(key, addCallback, size);
        }

        public async Task AddAsync<T>(string key, T value)
        {
            await AddAsync(key, value, 1);
        }

        public async Task AddAsync<T>(string key, T value, int size)
        {
            await cacheProvider.AddAsync<T>(key, value, size);
        }

        public async Task RemoveAsync(string key)
        {
            await cacheProvider.RemoveAsync(key);
        }
    }
}
