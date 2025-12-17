using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace StargateAPI.Tests.Integration
{
    public class AstronautDutyControllerIntegrationTests : IntegrationTestBase
    {
        public AstronautDutyControllerIntegrationTests(WebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreateAstronautDuty_ValidRequest_ReturnsSuccess()
        {
            // Arrange - Create person first
            await Client.PostAsync("/Person", new StringContent("\"Astronaut Alice\"", Encoding.UTF8, "application/json"));

            var dutyRequest = new
            {
                name = "Astronaut Alice",
                rank = "Captain",
                dutyTitle = "Commander",
                dutyStartDate = "2024-01-15"
            };
            var content = JsonContent.Create(dutyRequest);

            // Act
            var response = await Client.PostAsync("/AstronautDuty", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DutyResponse>();
            result!.Success.Should().BeTrue();
            result.Id.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAstronautDuty_PersonNotFound_ReturnsError()
        {
            // Arrange
            var dutyRequest = new
            {
                name = "NonExistent Person",
                rank = "Captain",
                dutyTitle = "Commander",
                dutyStartDate = "2024-01-15"
            };
            var content = JsonContent.Create(dutyRequest);

            // Act
            var response = await Client.PostAsync("/AstronautDuty", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            result!.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(400);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ExistingPerson_ReturnsDuties()
        {
            // Arrange - Create person and duty
            await Client.PostAsync("/Person", new StringContent("\"Test Astronaut\"", Encoding.UTF8, "application/json"));

            var dutyRequest = new
            {
                name = "Test Astronaut",
                rank = "Lieutenant",
                dutyTitle = "Pilot",
                dutyStartDate = "2024-01-01"
            };
            await Client.PostAsync("/AstronautDuty", JsonContent.Create(dutyRequest));

            // Act
            var response = await Client.GetAsync("/AstronautDuty/Test%20Astronaut");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetDutiesResponse>();
            result!.Success.Should().BeTrue();
            result.Person.Should().NotBeNull();
            result.AstronautDuties.Should().HaveCount(1);
        }

        private class DutyResponse
        {
            public bool Success { get; set; }
            public int? Id { get; set; }
        }

        private class ErrorResponse
        {
            public bool Success { get; set; }
            public int ResponseCode { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        private class GetDutiesResponse
        {
            public bool Success { get; set; }
            public PersonDto Person { get; set; } = new();
            public List<DutyDto> AstronautDuties { get; set; } = new();
        }

        private class PersonDto
        {
            public int PersonId { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class DutyDto
        {
            public string DutyTitle { get; set; } = string.Empty;
        }
    }
}