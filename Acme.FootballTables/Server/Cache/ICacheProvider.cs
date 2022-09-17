namespace Acme.FootballTables.Server.Cache
{
    public interface ICacheProvider
    {
        Task<T> GetOrAddAsync<T>(string key, Func<T> addCallback, int size);
        Task AddAsync<T>(string key, T value, int size);
        Task RemoveAsync(string key);
    }
}
