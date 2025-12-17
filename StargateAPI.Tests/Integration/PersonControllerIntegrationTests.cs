using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StargateAPI.Tests.Integration
{
    public class PersonControllerIntegrationTests : IntegrationTestBase
    {
        public PersonControllerIntegrationTests(WebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreatePerson_ValidName_ReturnsSuccess()
        {
            // Arrange
            var content = new StringContent("\"John Doe\"", Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/Person", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CreatePersonResponse>();
            result!.Success.Should().BeTrue();
            result.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreatePerson_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var content = new StringContent("\"\"", Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/Person", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            result!.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(400);
            result.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public async Task CreatePerson_DuplicateName_ReturnsBadRequest()
        {
            // Arrange - Create person first time
            var content = new StringContent("\"Jane Doe\"", Encoding.UTF8, "application/json");
            await Client.PostAsync("/Person", content);

            // Act - Try to create same person again
            var response = await Client.PostAsync("/Person", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            result!.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(400);
            result.Message.Should().Contain("already exists");
        }

        [Fact]
        public async Task GetPeople_AfterCreatingPeople_ReturnsAll()
        {
            // Arrange - Create multiple people
            await Client.PostAsync("/Person", new StringContent("\"Alice\"", Encoding.UTF8, "application/json"));
            await Client.PostAsync("/Person", new StringContent("\"Bob\"", Encoding.UTF8, "application/json"));

            // Act
            var response = await Client.GetAsync("/Person");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPeopleResponse>();
            result!.People.Should().HaveCountGreaterThanOrEqualTo(2);
            result.People.Should().Contain(p => p.Name == "Alice");
            result.People.Should().Contain(p => p.Name == "Bob");
        }

        [Fact]
        public async Task UpdatePerson_ValidUpdate_ReturnsSuccess()
        {
            // Arrange - Create person
            await Client.PostAsync("/Person", new StringContent("\"Old Name\"", Encoding.UTF8, "application/json"));

            // Act - Update person
            var content = new StringContent("\"New Name\"", Encoding.UTF8, "application/json");
            var response = await Client.PutAsync("/Person/Old%20Name", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdatePersonResponse>();
            result!.Success.Should().BeTrue();
        }

        // Response DTOs for deserialization
        private class CreatePersonResponse
        {
            public bool Success { get; set; }
            public int Id { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        private class ErrorResponse
        {
            public bool Success { get; set; }
            public int ResponseCode { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        private class GetPeopleResponse
        {
            public bool Success { get; set; }
            public List<PersonDto> People { get; set; } = new();
        }

        private class PersonDto
        {
            public int PersonId { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class UpdatePersonResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}