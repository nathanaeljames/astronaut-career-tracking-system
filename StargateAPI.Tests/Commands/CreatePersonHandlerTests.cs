using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using Xunit;

namespace StargateAPI.Tests.Commands
{
    public class CreatePersonHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ValidName_CreatesPerson()
        {
            // Arrange - set up test data and dependencies
            var handler = new CreatePersonHandler(Context, LoggingService);
            var request = new CreatePerson { Name = "John Doe" };

            // Act - execute the method being tested
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert - verify the outcome using FluentAssertions syntax
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Id.Should().BeGreaterThan(0);

            // Verify person was actually saved
            var person = await Context.People.FirstOrDefaultAsync(p => p.Name == "John Doe");
            person.Should().NotBeNull();
            person!.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task Handle_ValidName_LogsSuccess()
        {
            // Arrange
            var handler = new CreatePersonHandler(Context, LoggingService);
            var request = new CreatePerson { Name = "Jane Doe" };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert - verify log was created
            var log = await Context.ProcessLogs
                .FirstOrDefaultAsync(l => l.Action == "CreatePerson" && l.Level == "Info");

            log.Should().NotBeNull();
            log!.Message.Should().Contain("Jane Doe");
            log.PersonName.Should().Be("Jane Doe");
        }

        [Fact]
        public async Task Handle_MultiplePeople_CreatesAllSuccessfully()
        {
            // Arrange
            var handler = new CreatePersonHandler(Context, LoggingService);

            // Act
            await handler.Handle(new CreatePerson { Name = "Person 1" }, CancellationToken.None);
            await handler.Handle(new CreatePerson { Name = "Person 2" }, CancellationToken.None);
            await handler.Handle(new CreatePerson { Name = "Person 3" }, CancellationToken.None);

            // Assert
            var peopleCount = await Context.People.CountAsync();
            peopleCount.Should().Be(3);
        }
    }
}