using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;

namespace StargateAPI.Tests.Commands
{
    public class PreprocessorTests : TestBase
    {
        [Fact]
        public async Task CreatePersonPreProcessor_NullName_ThrowsException()
        {
            // Arrange
            var preprocessor = new CreatePersonPreProcessor(Context);
            var request = new CreatePerson { Name = null! };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreatePersonPreProcessor_EmptyName_ThrowsException()
        {
            // Arrange
            var preprocessor = new CreatePersonPreProcessor(Context);
            var request = new CreatePerson { Name = "" };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreatePersonPreProcessor_DuplicateName_ThrowsException()
        {
            // Arrange
            Context.People.Add(new Person { Name = "Existing" });
            await Context.SaveChangesAsync();

            var preprocessor = new CreatePersonPreProcessor(Context);
            var request = new CreatePerson { Name = "Existing" };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdatePersonPreProcessor_EmptyCurrentName_ThrowsException()
        {
            // Arrange
            var preprocessor = new UpdatePersonPreProcessor(Context);
            var request = new UpdatePerson { CurrentName = "", NewName = "New" };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdatePersonPreProcessor_EmptyNewName_ThrowsException()
        {
            // Arrange
            var preprocessor = new UpdatePersonPreProcessor(Context);
            var request = new UpdatePerson { CurrentName = "Current", NewName = "" };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdatePersonPreProcessor_PersonNotFound_ThrowsException()
        {
            // Arrange
            var preprocessor = new UpdatePersonPreProcessor(Context);
            var request = new UpdatePerson { CurrentName = "NonExistent", NewName = "New" };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreateAstronautDutyPreProcessor_EmptyName_ThrowsException()
        {
            // Arrange
            var preprocessor = new CreateAstronautDutyPreProcessor(Context);
            var request = new CreateAstronautDuty
            {
                Name = "",
                Rank = "Captain",
                DutyTitle = "Pilot",
                DutyStartDate = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreateAstronautDutyPreProcessor_PersonNotFound_ThrowsException()
        {
            // Arrange
            var preprocessor = new CreateAstronautDutyPreProcessor(Context);
            var request = new CreateAstronautDuty
            {
                Name = "NonExistent",
                Rank = "Captain",
                DutyTitle = "Pilot",
                DutyStartDate = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdatePersonPreProcessor_NewNameAlreadyTaken_ThrowsException()
        {
            // Arrange
            Context.People.Add(new Person { Name = "Existing1" });
            Context.People.Add(new Person { Name = "Existing2" });
            await Context.SaveChangesAsync();

            var preprocessor = new UpdatePersonPreProcessor(Context);
            var request = new UpdatePerson
            {
                CurrentName = "Existing1",
                NewName = "Existing2"  // Try to rename to existing name
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreateAstronautDutyPreProcessor_EmptyRank_ThrowsException()
        {
            // Arrange
            Context.People.Add(new Person { Name = "Test" });
            await Context.SaveChangesAsync();

            var preprocessor = new CreateAstronautDutyPreProcessor(Context);
            var request = new CreateAstronautDuty
            {
                Name = "Test",
                Rank = "",  // Empty rank
                DutyTitle = "Pilot",
                DutyStartDate = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreateAstronautDutyPreProcessor_EmptyDutyTitle_ThrowsException()
        {
            // Arrange
            Context.People.Add(new Person { Name = "Test" });
            await Context.SaveChangesAsync();

            var preprocessor = new CreateAstronautDutyPreProcessor(Context);
            var request = new CreateAstronautDuty
            {
                Name = "Test",
                Rank = "Captain",
                DutyTitle = "",  // Empty title
                DutyStartDate = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await preprocessor.Process(request, CancellationToken.None)
            );
        }
    }
}