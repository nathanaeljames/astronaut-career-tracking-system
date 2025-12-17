using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;
using Xunit;

namespace StargateAPI.Tests.Queries
{
    public class GetPeopleQueryTests : TestBase
    {
        [Fact]
        public async Task Handle_NoPeople_ReturnsEmptyList()
        {
            // Arrange
            var handler = new GetPeopleHandler(Context);
            var request = new GetPeople();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.People.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_MultiplePeople_ReturnsAllPeople()
        {
            // Arrange
            Context.People.AddRange(
                new Person { Name = "Alice" },
                new Person { Name = "Bob" },
                new Person { Name = "Charlie" }
            );
            await Context.SaveChangesAsync();

            var handler = new GetPeopleHandler(Context);
            var request = new GetPeople();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.People.Should().HaveCount(3);
            result.People.Select(p => p.Name).Should().Contain(new[] { "Alice", "Bob", "Charlie" });
        }

        [Fact]
        public async Task Handle_PeopleWithAstronautDetails_IncludesAstronautInfo()
        {
            // Arrange
            var person = new Person { Name = "Astronaut Jane" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var detail = new AstronautDetail
            {
                PersonId = person.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander",
                CareerStartDate = new DateTime(2020, 1, 1)
            };
            Context.AstronautDetails.Add(detail);
            await Context.SaveChangesAsync();

            var handler = new GetPeopleHandler(Context);
            var request = new GetPeople();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            var astronaut = result.People.First(p => p.Name == "Astronaut Jane");
            astronaut.CurrentRank.Should().Be("Captain");
            astronaut.CurrentDutyTitle.Should().Be("Commander");
        }
    }
}