namespace StargateAPI.Business.Services
{
    //Interface for logging service - defines the contract for logging operations
    public interface ILoggingService
    {
        /*
            action: the action being performed (e.g., "CreatePerson", "UpdatePerson")
            message: Human-readable success/error/exception message
            personName: (optional) the person associated with this operation
        */
        
        //Log a successful operation
        Task LogSuccess(string action, string message, string? personName = null);
        //Log a business logic error (expected failures)
        Task LogError(string action, string message, string? personName = null);
        // Log an unexpected exception
        Task LogException(string action, Exception ex, string? personName = null);
    }
}