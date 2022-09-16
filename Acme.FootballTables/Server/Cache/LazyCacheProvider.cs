namespace Acme.FootballTables.Server.Cache
{
    public class LazyCacheProvider : ICacheProvider
    {
        public T GetOrAdd<T>(string key, Func<T> addCallback)
        {
            throw new NotImplementedException();
        }

        public void Add<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
