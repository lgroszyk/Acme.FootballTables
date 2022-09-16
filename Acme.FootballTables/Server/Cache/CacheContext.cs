namespace Acme.FootballTables.Server.Cache
{
    public class CacheContext
    {
        private readonly ICacheProvider cacheProvider;

        public CacheContext(ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        public T GetOrAdd<T>(string key, Func<T> addCallback)
        {
            return cacheProvider.GetOrAdd<T>(key, addCallback);
        }

        public void Add<T>(string key, T value)
        {
            cacheProvider.Add<T>(key, value);
        }

        public void Remove(string key)
        {
            cacheProvider.Remove(key);
        }
    }
}
