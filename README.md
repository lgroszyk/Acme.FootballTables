The app is designed to learn different cache usage options in ASP.NET Core - both in-memory and distributed cache. Three options are implemented:
- MemoryCacheProvider (using IMemoryCache built into ASP.NET Core)
- LazyCacheProvider (using the LazyCache.AspNetCore library)
- RedisCacheProvider (using the Microsoft.Extensions.Caching.StackExchangeRedis library)

The subject of the application are football league tables - data rarely modified by admin user, and often read by standard users.

The application uses ASP.NET Identity. In order to use all app options, you must create a user account and log in to it.

The application operates on the data of the local SQL Server database. The defined data migrations must be deployed for the application to work.
