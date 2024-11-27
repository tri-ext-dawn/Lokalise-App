using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Services.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

public class LokaliseApiClientTests
{
    private Fixture _fixture;
    private Mock<ILogger<LokaliseApiClient>> _mockLogger;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    
    private ILokaliseApiClient _sut;

    private void VerifyLog(LogLevel logLevel, string message, int times = 1) => Utilities.VerifyLog(_mockLogger, logLevel, message, times);
    private T DeepClone<T>(T source) => Utilities.DeepClone(source);
    
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

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _httpClient.BaseAddress = new Uri("https://api.lokalise.com/api2/");
        _mockLogger = new Mock<ILogger<LokaliseApiClient>>();
        _sut = new LokaliseApiClient(_mockLogger.Object, _httpClient);
    }
    
    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task GetTranslations_Success_ReturnsTranslations()
    {
        // Arrange
        var projectId = "test-project";
        var languageId = 123L;
        var expectedUrl = $"projects/{projectId}/translations?filter_lang_id={languageId}&limit=2000&page=1";
        var translation = _fixture.Create<TranslationResponse>();

        var response = new LokaliseTranslationsResponse
        {
            project_id = projectId,
            translations = new List<TranslationResponse>
            {
                translation
            }
        };

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
                Content = JsonContent.Create(response)
            });

        // Act
        var result = await _sut.GetTranslations(languageId, projectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(translation.translation_id);
        result[0].LanguageIso.Should().Be(translation.language_iso);
        result[0].TranslationText.Should().Be(translation.translation);
        result[0].IsReviewed.Should().Be(translation.is_reviewed);
        result[0].IsUnverified.Should().Be(translation.is_unverified);
        
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get && 
                req.RequestUri.ToString().Contains(expectedUrl)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task GetTranslations_WhenRequestFails_LogsErrorAndThrowsException()
    {
        // Arrange
        var projectId = "test-project";
        var languageId = 123L;

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act
        var exception = Assert.ThrowsAsync<HttpRequestException>(() => _sut.GetTranslations(languageId, projectId));

        // Assert
        VerifyLog(LogLevel.Error, "Failed to get translations from Lokalise");
    }
}