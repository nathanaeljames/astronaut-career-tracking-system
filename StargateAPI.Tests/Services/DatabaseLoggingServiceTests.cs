using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Services;
using Xunit;

namespace StargateAPI.Tests.Services
{
    public class DatabaseLoggingServiceTests : TestBase
    {
        [Fact]
        public async Task LogSuccess_SavesLogToDatabase()
        {
            // Arrange
            var service = new DatabaseLoggingService(Context);

            // Act
            await service.LogSuccess("TestAction", "Test message", "TestPerson");

            // Assert
            var log = await Context.ProcessLogs.FirstOrDefaultAsync();
            log.Should().NotBeNull();
            log!.Level.Should().Be("Info");
            log.Action.Should().Be("TestAction");
            log.Message.Should().Be("Test message");
            log.PersonName.Should().Be("TestPerson");
            log.StackTrace.Should().BeNull();
        }

        [Fact]
        public async Task LogError_SavesLogToDatabase()
        {
            // Arrange
            var service = new DatabaseLoggingService(Context);

            // Act
            await service.LogError("TestAction", "Error message", "TestPerson");

            // Assert
            var log = await Context.ProcessLogs.FirstOrDefaultAsync();
            log.Should().NotBeNull();
            log!.Level.Should().Be("Error");
            log.Message.Should().Be("Error message");
        }

        [Fact]
        public async Task LogException_SavesExceptionWithStackTrace()
        {
            // Arrange
            var service = new DatabaseLoggingService(Context);
            Exception exception;

            // Create an exception with a real stack trace by throwing and catching
            try
            {
                throw new Exception("Test exception");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Act
            await service.LogException("TestAction", exception, "TestPerson");

            // Assert
            var log = await Context.ProcessLogs.FirstOrDefaultAsync();
            log.Should().NotBeNull();
            log!.Level.Should().Be("Exception");
            log.Message.Should().Be("Test exception");
            log.StackTrace.Should().NotBeNull();
            log.StackTrace.Should().Contain("LogException_SavesExceptionWithStackTrace");
        }

        [Fact]
        public async Task LogSuccess_WithoutPersonName_SavesCorrectly()
        {
            // Arrange
            var service = new DatabaseLoggingService(Context);

            // Act
            await service.LogSuccess("GetPeople", "Retrieved all people");

            // Assert
            var log = await Context.ProcessLogs.FirstOrDefaultAsync();
            log!.PersonName.Should().BeNull();
        }
    }
}