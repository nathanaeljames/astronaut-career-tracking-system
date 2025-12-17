using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;

namespace StargateAPI.Tests.Commands
{
    public class UpdatePersonHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ValidUpdate_UpdatesPersonName()
        {
            // Arrange
            var person = new Person { Name = "Old Name" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new UpdatePersonHandler(Context);
            var request = new UpdatePerson
            {
                CurrentName = "Old Name",
                NewName = "New Name"
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();

            var updatedPerson = await Context.People.FirstAsync(p => p.Id == person.Id);
            updatedPerson.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task Handle_SameName_SucceedsWithoutChange()
        {
            // Arrange
            var person = new Person { Name = "Same Name" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new UpdatePersonHandler(Context);
            var request = new UpdatePerson
            {
                CurrentName = "Same Name",
                NewName = "Same Name"
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
        }
    }
}