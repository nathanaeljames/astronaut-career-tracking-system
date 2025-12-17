using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StargateAPI.Business.Data;
using Xunit;

namespace StargateAPI.Tests.Integration
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly WebApplicationFactory<Program> Factory;
        private readonly SqliteConnection _connection;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            // Create SQLite in-memory connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Configure factory to use in-memory SQLite for tests
            Factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database registration
                    services.RemoveAll(typeof(DbContextOptions<StargateContext>));

                    // Add SQLite in-memory database for testing
                    services.AddDbContext<StargateContext>(options =>
                    {
                        options.UseSqlite(_connection);
                    });

                    // Build service provider and create database
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
                    db.Database.EnsureCreated();
                });
            });

            Client = Factory.CreateClient();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            Client.Dispose();
        }
    }
}