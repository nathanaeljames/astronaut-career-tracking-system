using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Services;

namespace StargateAPI.Tests
{
    // Base class for all tests - provides fresh SQLite in-memory database for each test
    // Using SQLite In-Memory instead of EF Core InMemory because Dapper only works with real databases
    // Not just C# objects
    public class TestBase : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected StargateContext Context { get; private set; }
        protected ILoggingService LoggingService { get; private set; }

        public TestBase()
        {
            // Create SQLite in-memory connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Create context with SQLite
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new StargateContext(options);

            // Create database schema
            Context.Database.EnsureCreated();

            // Create logging service for tests
            LoggingService = new DatabaseLoggingService(Context);
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}