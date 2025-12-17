using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;
using Xunit;

namespace StargateAPI.Tests.Queries
{
    public class GetPersonByNameQueryTests : TestBase
    {
        [Fact]
        public async Task Handle_PersonExists_ReturnsPerson()
        {
            // Arrange
            var person = new Person { Name = "John Doe" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new GetPersonByNameHandler(Context);
            var request = new GetPersonByName { Name = "John Doe" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Person.Should().NotBeNull();
            result.Person!.PersonId.Should().Be(person.Id);
            result.Person.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task Handle_PersonDoesNotExist_ReturnsNull()
        {
            // Arrange
            var handler = new GetPersonByNameHandler(Context);
            var request = new GetPersonByName { Name = "NonExistent" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().BeNull();
        }

        [Fact]
        public async Task Handle_PersonWithAstronautDetails_ReturnsCompleteInfo()
        {
            // Arrange
            var person = new Person { Name = "Captain Jane" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var detail = new AstronautDetail
            {
                PersonId = person.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Pilot",
                CareerStartDate = new DateTime(2020, 1, 1)
            };
            Context.AstronautDetails.Add(detail);
            await Context.SaveChangesAsync();

            var handler = new GetPersonByNameHandler(Context);
            var request = new GetPersonByName { Name = "Captain Jane" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Person.Should().NotBeNull();
            result.Person!.PersonId.Should().Be(person.Id);
            result.Person.CurrentRank.Should().Be("Captain");
            result.Person.CurrentDutyTitle.Should().Be("Pilot");
        }
    }
}