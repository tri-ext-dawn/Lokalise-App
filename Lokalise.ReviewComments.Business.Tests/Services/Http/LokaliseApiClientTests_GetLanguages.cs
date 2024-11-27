using System.Net;
using System.Net.Http.Json;
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
        SetupHttpResponseWithContent(_expectedResponse);

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
        SetupFailedHttpResponse(HttpStatusCode.InternalServerError);

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
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(_expectedResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        await _sut.GetLanguages(_projectId);

        // Assert
        VerifyHttpRequest();
    }

    private void SetupFailedHttpResponse(HttpStatusCode statusCode)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode
            });
    }

    private void SetupHttpResponseWithContent<T>(T content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(content)
            });
    }

    private void VerifyLanguage(Language result, Language expectedItem)
    {
        result.Should().NotBeNull();
        result.ISO.Should().Be(expectedItem.ISO);
        result.Name.Should().Be(expectedItem.Name);
    }

    private void VerifyHttpRequest()
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get && 
                req.RequestUri.ToString().Contains($"projects/{_projectId}/languages")),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}