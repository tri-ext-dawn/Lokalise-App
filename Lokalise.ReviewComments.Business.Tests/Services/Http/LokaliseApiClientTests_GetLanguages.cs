using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

public class LokaliseApiClientTests_GetLanguages : LokaliseApiClientTests
{
    // Tests for Task<Language> GetLanguages(string projectId)
    // - Should return a list of languages when a valid request is made
    // - Should log an error and throw an exception when the HTTP request fails
    // - Should call correct endpoint

    private string _projectId;
    private LanguagesResponse _expectedResponse;

    public class LanguagesResponse
    {
        public string project_id { get; set; }
        public List<Language> languages { get; set; }
    }

    [SetUp]
    public void SetUp()
    {
        _projectId = _fixture.Create<string>();
        _expectedResponse = _fixture.Create<LanguagesResponse>();
    }
    
    [Test]
    public async Task GetLanguages_ValidRequest_ReturnsLanguageList()
    {
        // Arrange
        var expectedItem = DeepClone(_expectedResponse.languages.First());
        SetupHttpMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(_expectedResponse));

        // Act
        var result = await _sut.GetLanguages(_projectId);

        // Assert
        result.Should().NotBeEmpty();
        VerifyLanguage(result.FirstOrDefault(x => x.Id == expectedItem.Id), expectedItem);
    }

    [Test]
    public async Task GetLanguages_FailedRequest_ThrowsAndLogsError()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.InternalServerError);

        // Act
        var act = () => _sut.GetLanguages(_projectId);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Failed to get languages");
        VerifyLog(LogLevel.Error, "Failed to get languages from Lokalise");
    }

    [Test]
    public async Task GetLanguages_ValidRequest_CallsCorrectEndpoint()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(_expectedResponse));

        // Act
        await _sut.GetLanguages(_projectId);

        // Assert
        VerifyHttpRequest($"projects/{_projectId}/languages", HttpMethod.Get, Times.Once());
    }

    private void VerifyLanguage(Language result, Language expectedItem)
    {
        result.Should().NotBeNull();
        result.ISO.Should().Be(expectedItem.ISO);
        result.Name.Should().Be(expectedItem.Name);
    }
}