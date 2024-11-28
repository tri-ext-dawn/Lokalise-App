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
        var content = new LokaliseTranslationsResponse
        {
            project_id = DefaultProjectId,
            translations = new List<TranslationResponse> { _defaultTranslation }
        };
        SetupHttpMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(content));

        // Act
        var result = await _sut.GetTranslations(DefaultLanguageId, DefaultProjectId);

        // Assert
        VerifyTranslationResponse(result, _defaultTranslation);
        VerifyHttpRequest(_expectedUrl, HttpMethod.Get, Times.Once());
    }

    [Test]
    [Category("Error Handling")]
    public async Task GetTranslations_WhenHttpRequestFails_LogsErrorAndThrowsException()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.InternalServerError);

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
        SetupHttpMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(emptyResponse));

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