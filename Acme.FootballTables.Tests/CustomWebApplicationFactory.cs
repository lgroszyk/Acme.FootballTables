using Acme.FootballTables.Server.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace Acme.FootballTables.Tests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services
                    .AddAuthentication(options => { options.DefaultAuthenticateScheme = "TestsScheme"; })
                    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("TestsScheme", options => { });

                var physicalDatabaseContext = services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(physicalDatabaseContext);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestsDatabase");
                });

                using var servicesScope = services.BuildServiceProvider().CreateScope();
                var inMemoryDatabaseContext = servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                inMemoryDatabaseContext.Database.EnsureDeleted();
                inMemoryDatabaseContext.Database.EnsureCreated();
                SampleDatabaseSeeder.ClearAndSeedFootballTables(inMemoryDatabaseContext);
            });
        }
    }
}
