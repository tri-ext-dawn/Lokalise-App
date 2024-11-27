using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

/// <summary>
/// Tests for the translation retrieval functionality of the Lokalise API client
/// </summary>
[TestFixture]
public class LokaliseApiClientTests_GetTranslations : LokaliseApiClientTests
{
    private const string DefaultProjectId = "test-project-id";
    private const long DefaultLanguageId = 12345L;
    private const string DefaultLanguageIso = "en-US";
    private const string DefaultTranslationText = "Test Translation";
    
    private TranslationResponse _defaultTranslation;
    private string _expectedUrl;

    [SetUp]
    public void SetUp()
    {
        _defaultTranslation = CreateDefaultTranslation();
        _expectedUrl = $"projects/{DefaultProjectId}/translations?filter_lang_id={DefaultLanguageId}&limit=2000&page=1";
    }

    [Test]
    [Category("Happy Path")]
    public async Task GetTranslations_WhenValidRequestMade_ReturnsExpectedTranslations()
    {
        // Arrange
        SetupSuccessfulHttpResponse(_defaultTranslation);

        // Act
        var result = await _sut.GetTranslations(DefaultLanguageId, DefaultProjectId);

        // Assert
        VerifyTranslationResponse(result, _defaultTranslation);
        VerifyHttpRequest();
    }

    [Test]
    [Category("Error Handling")]
    public async Task GetTranslations_WhenHttpRequestFails_LogsErrorAndThrowsException()
    {
        // Arrange
        SetupFailedHttpResponse(HttpStatusCode.InternalServerError);

        // Act & Assert
        var exception = Assert.ThrowsAsync<HttpRequestException>(
            async () => await _sut.GetTranslations(DefaultLanguageId, DefaultProjectId));
        
        exception.Should().NotBeNull();
        VerifyLog(LogLevel.Error, "Failed to get translations from Lokalise");
    }

    [Test]
    [Category("Error Handling")]
    public async Task GetTranslations_WhenEmptyResponseReceived_ReturnsEmptyList()
    {
        // Arrange
        var emptyResponse = new LokaliseTranslationsResponse
        {
            project_id = DefaultProjectId,
            translations = new List<TranslationResponse>()
        };

        SetupHttpResponseWithContent(emptyResponse);

        // Act
        var result = await _sut.GetTranslations(DefaultLanguageId, DefaultProjectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    private TranslationResponse CreateDefaultTranslation()
    {
        return new TranslationResponse
        {
            translation_id = _fixture.Create<long>(),
            language_iso = DefaultLanguageIso,
            translation = DefaultTranslationText,
            is_reviewed = true,
            is_unverified = false
        };
    }

    private void SetupSuccessfulHttpResponse(TranslationResponse translation)
    {
        var response = new LokaliseTranslationsResponse
        {
            project_id = DefaultProjectId,
            translations = new List<TranslationResponse> { translation }
        };

        SetupHttpResponseWithContent(response);
    }

    private void SetupHttpResponseWithContent<T>(T content)
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
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(content)
            });
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

    private void VerifyTranslationResponse(IReadOnlyList<Translation> result, TranslationResponse expected)
    {
        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        var translation = result[0];
        translation.Should().BeEquivalentTo(new
        {
            Id = expected.translation_id,
            LanguageIso = expected.language_iso,
            TranslationText = expected.translation,
            IsReviewed = expected.is_reviewed,
            IsUnverified = expected.is_unverified
        });
    }

    private void VerifyHttpRequest()
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().Contains(_expectedUrl)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private class LokaliseTranslationsResponse
    {
        public string project_id { get; set; }
        public List<TranslationResponse> translations { get; set; }
    }

    private class TranslationResponse
    {
        public long translation_id { get; set; }
        public string language_iso { get; set; }
        public string translation { get; set; }
        public bool is_reviewed { get; set; }
        public bool is_unverified { get; set; }
    }
}