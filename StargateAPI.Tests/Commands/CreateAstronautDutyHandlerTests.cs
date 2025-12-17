using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;

namespace StargateAPI.Tests.Commands
{
    public class CreateAstronautDutyHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_FirstDuty_CreatesAstronautDetailAndDuty()
        {
            // Arrange - Create a person first
            var person = new Person { Name = "John Doe" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new CreateAstronautDutyHandler(Context, LoggingService);
            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Pilot",
                DutyStartDate = new DateTime(2024, 1, 15)
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Id.Should().NotBeNull();

            // Verify AstronautDetail was created (Rule 2 & 3)
            var detail = await Context.AstronautDetails
                .FirstOrDefaultAsync(d => d.PersonId == person.Id);

            detail.Should().NotBeNull();
            detail!.CurrentRank.Should().Be("Captain");
            detail!.CurrentDutyTitle.Should().Be("Pilot");
            detail.CareerStartDate.Should().Be(new DateTime(2024, 1, 15));
            detail.CareerEndDate.Should().BeNull(); // Not retired

            // Verify AstronautDuty was created
            var duty = await Context.AstronautDuties
                .FirstOrDefaultAsync(d => d.PersonId == person.Id);

            duty.Should().NotBeNull();
            duty!.Rank.Should().Be("Captain");
            duty.DutyTitle.Should().Be("Pilot");
            duty.DutyStartDate.Should().Be(new DateTime(2024, 1, 15));
            duty.DutyEndDate.Should().BeNull(); // Current duty has no end date (Rule 4)
        }

        [Fact]
        public async Task Handle_SecondDuty_UpdatesCurrentAndClosesPrevious()
        {
            // Arrange - Create person with first duty
            var person = new Person { Name = "Jane Smith" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new CreateAstronautDutyHandler(Context, LoggingService);

            // Add first duty
            await handler.Handle(new CreateAstronautDuty
            {
                Name = "Jane Smith",
                Rank = "Lieutenant",
                DutyTitle = "Engineer",
                DutyStartDate = new DateTime(2024, 1, 1)
            }, CancellationToken.None);

            // Act - Add second duty
            await handler.Handle(new CreateAstronautDuty
            {
                Name = "Jane Smith",
                Rank = "Captain",
                DutyTitle = "Chief Engineer",
                DutyStartDate = new DateTime(2024, 6, 1)
            }, CancellationToken.None);

            // Assert
            var detail = await Context.AstronautDetails
                .FirstOrDefaultAsync(d => d.PersonId == person.Id);

            // Current detail should reflect new duty (Rule 3)
            detail!.CurrentRank.Should().Be("Captain");
            detail.CurrentDutyTitle.Should().Be("Chief Engineer");
            detail.CareerStartDate.Should().Be(new DateTime(2024, 1, 1)); // Original start

            // Check duties
            var duties = await Context.AstronautDuties
                .Where(d => d.PersonId == person.Id)
                .OrderBy(d => d.DutyStartDate)
                .ToListAsync();

            duties.Should().HaveCount(2);

            // First duty should be closed (Rule 5)
            var firstDuty = duties[0];
            firstDuty.DutyTitle.Should().Be("Engineer");
            firstDuty.DutyEndDate.Should().Be(new DateTime(2024, 5, 31)); // Day before new duty

            // Second duty should be current (Rule 4)
            var secondDuty = duties[1];
            secondDuty.DutyTitle.Should().Be("Chief Engineer");
            secondDuty.DutyEndDate.Should().BeNull(); // Current duty has no end date
        }

        [Fact]
        public async Task Handle_RetiredDuty_SetsCareerEndDate()
        {
            // Arrange - Create person with active duty
            var person = new Person { Name = "Bob Johnson" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new CreateAstronautDutyHandler(Context, LoggingService);

            // Add active duty
            await handler.Handle(new CreateAstronautDuty
            {
                Name = "Bob Johnson",
                Rank = "Commander",
                DutyTitle = "Mission Specialist",
                DutyStartDate = new DateTime(2020, 1, 1)
            }, CancellationToken.None);

            // Act - Retire
            await handler.Handle(new CreateAstronautDuty
            {
                Name = "Bob Johnson",
                Rank = "Commander",
                DutyTitle = "RETIRED",
                DutyStartDate = new DateTime(2024, 12, 1)
            }, CancellationToken.None);

            // Assert
            var detail = await Context.AstronautDetails
                .FirstOrDefaultAsync(d => d.PersonId == person.Id);

            // Rule 6 & 7: Career end date is one day before retirement
            detail!.CurrentDutyTitle.Should().Be("RETIRED");
            detail.CareerEndDate.Should().Be(new DateTime(2024, 11, 30)); // Day before retirement
        }

        [Fact]
        public async Task Handle_ValidDuty_LogsSuccess()
        {
            // Arrange
            var person = new Person { Name = "Test Person" };
            Context.People.Add(person);
            await Context.SaveChangesAsync();

            var handler = new CreateAstronautDutyHandler(Context, LoggingService);
            var request = new CreateAstronautDuty
            {
                Name = "Test Person",
                Rank = "Ensign",
                DutyTitle = "Cadet",
                DutyStartDate = new DateTime(2024, 1, 1)
            };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert - verify logging
            var log = await Context.ProcessLogs
                .FirstOrDefaultAsync(l => l.Action == "CreateAstronautDuty" && l.Level == "Info");

            log.Should().NotBeNull();
            log!.Message.Should().Contain("Cadet");
            log.PersonName.Should().Be("Test Person");
        }
    }
}