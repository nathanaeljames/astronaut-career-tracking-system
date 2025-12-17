using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;
using Xunit;

namespace StargateAPI.Tests.Queries
{
    public class GetAstronautDutiesByNameQueryTests : TestBase
    {
        [Fact]
        public async Task Handle_PersonWithDuties_ReturnsPersonAndDuties()
        {
            // Arrange
            var person = new Person { Name = "Astronaut Bob" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var detail = new AstronautDetail
            {
                PersonId = person.Id,
                CurrentRank = "Commander",
                CurrentDutyTitle = "Mission Specialist",
                CareerStartDate = new DateTime(2020, 1, 1)
            };
            Context.AstronautDetails.Add(detail);

            var duties = new[]
            {
                new AstronautDuty
                {
                    PersonId = person.Id,
                    Rank = "Lieutenant",
                    DutyTitle = "Engineer",
                    DutyStartDate = new DateTime(2020, 1, 1),
                    DutyEndDate = new DateTime(2022, 12, 31)
                },
                new AstronautDuty
                {
                    PersonId = person.Id,
                    Rank = "Commander",
                    DutyTitle = "Mission Specialist",
                    DutyStartDate = new DateTime(2023, 1, 1),
                    DutyEndDate = null // Current duty
                }
            };
            Context.AstronautDuties.AddRange(duties);
            await Context.SaveChangesAsync();

            var handler = new GetAstronautDutiesByNameHandler(Context);
            var request = new GetAstronautDutiesByName { Name = "Astronaut Bob" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Person.Should().NotBeNull();
            result.Person.Name.Should().Be("Astronaut Bob");
            result.AstronautDuties.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_PersonNotFound_ReturnsFailure()
        {
            // Arrange
            var handler = new GetAstronautDutiesByNameHandler(Context);
            var request = new GetAstronautDutiesByName { Name = "NonExistent" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(404);
            result.Message.Should().Contain("not found");
        }

        [Fact]
        public async Task Handle_DutiesInCorrectOrder_OrdersByStartDateDescending()
        {
            // Arrange
            var person = new Person { Name = "Test Person" };
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

            // Add duties in random order
            var duties = new[]
            {
                new AstronautDuty
                {
                    PersonId = person.Id,
                    Rank = "Captain",
                    DutyTitle = "Third Duty",
                    DutyStartDate = new DateTime(2024, 1, 1),
                    DutyEndDate = null
                },
                new AstronautDuty
                {
                    PersonId = person.Id,
                    Rank = "Lieutenant",
                    DutyTitle = "First Duty",
                    DutyStartDate = new DateTime(2020, 1, 1),
                    DutyEndDate = new DateTime(2021, 12, 31)
                },
                new AstronautDuty
                {
                    PersonId = person.Id,
                    Rank = "Lieutenant Commander",
                    DutyTitle = "Second Duty",
                    DutyStartDate = new DateTime(2022, 1, 1),
                    DutyEndDate = new DateTime(2023, 12, 31)
                }
            };
            Context.AstronautDuties.AddRange(duties);
            await Context.SaveChangesAsync();

            var handler = new GetAstronautDutiesByNameHandler(Context);
            var request = new GetAstronautDutiesByName { Name = "Test Person" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert - duties should be ordered by start date DESCENDING (newest first)
            result.AstronautDuties[0].DutyTitle.Should().Be("Third Duty");
            result.AstronautDuties[1].DutyTitle.Should().Be("Second Duty");
            result.AstronautDuties[2].DutyTitle.Should().Be("First Duty");
        }
    }
}