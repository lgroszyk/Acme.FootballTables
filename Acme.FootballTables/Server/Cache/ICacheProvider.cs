namespace Acme.FootballTables.Server.Cache
{
    public interface ICacheProvider
    {
        T GetOrAdd<T>(string key, Func<T> addCallback);
        void Add<T>(string key, T value);
        void Remove(string key);
    }
}
