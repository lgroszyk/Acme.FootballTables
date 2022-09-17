using Acme.FootballTables.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acme.FootballTables.Server.Cache
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IDistributedCache cache;

        public RedisCacheProvider(IDistributedCache cache)
        {
            this.cache = cache;
        }

        // TODO remove size param as it's used only in other providers
        public async Task<T> GetOrAddAsync<T>(string key, Func<T> addCallback, int size)
        {
            var serializedValue = await cache.GetStringAsync(key);
            if (serializedValue != null)
            {
                return JsonSerializer.Deserialize<T>(serializedValue, 
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    })!;
            }

            var value = addCallback();
            await cache.SetStringAsync(key, JsonSerializer.Serialize(value, 
                new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                }), 
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(60),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                });

            return value;
        }

        public async Task AddAsync<T>(string key, T value, int size)
        {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(value, 
                new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                }), 
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(60),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                });
        }

        public async Task RemoveAsync(string key)
        {
            await cache.RemoveAsync(key);
        }
    }
}
