using StargateAPI.Business.Data;

namespace StargateAPI.Business.Services
{
    // Database implementation of ILoggingService - stores logs in ProcessLog table
    public class DatabaseLoggingService : ILoggingService
    {
        private readonly StargateContext _context;

        // Constructor - receives database context via dependency injection
        public DatabaseLoggingService(StargateContext context)
        {
            _context = context;
        }

        // Log successful operation (Level: Info)
        public async Task LogSuccess(string action, string message, string? personName = null)
        {
            var log = new ProcessLog
            {
                Timestamp = DateTime.UtcNow,
                Level = "Info",
                Action = action,
                Message = message,
                PersonName = personName,
                StackTrace = null  // No stack trace for successes
            };

            _context.ProcessLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        //Log business logic error (Level: Error)
        public async Task LogError(string action, string message, string? personName = null)
        {
            var log = new ProcessLog
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Action = action,
                Message = message,
                PersonName = personName,
                StackTrace = null  // No stack trace for expected errors
            };

            _context.ProcessLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        // Log unexpected exception (Level: Exception)
        public async Task LogException(string action, Exception ex, string? personName = null)
        {
            var log = new ProcessLog
            {
                Timestamp = DateTime.UtcNow,
                Level = "Exception",
                Action = action,
                Message = ex.Message,  // Exception message
                PersonName = personName,
                StackTrace = ex.StackTrace  // Full stack trace for debugging
            };

            _context.ProcessLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}