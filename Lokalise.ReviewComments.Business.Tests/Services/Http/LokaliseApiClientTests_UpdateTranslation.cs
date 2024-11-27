using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

public class LokaliseApiClientTests_UpdateTranslation : LokaliseApiClientTests
{
    // Tests for Task<bool> UpdateTranslation(long translationId, string translation, string projectId)
    //  - Should call Lokalise API to update translation
    //  - Should return true if translation was updated successfully
    //  - Should return false if translation was not updated successfully
    //  - Should log error if translation was not updated successfully
    //  - Should not throw HttpRequestException if translation was not updated successfully
    
    private readonly string _projectId = "123456";
    private readonly long _translationId = 789;
    private readonly string _translation = "Updated translation";

    [Test]
    public async Task UpdateTranslation_ShouldCallApiAndReturnTrue_WhenSuccessful()
    {
        // Arrange
        var expectedUrl = $"projects/{_projectId}/translations/{_translationId}";
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    // req.Content.Equals($$"""{"translation":"{{_translation}}","is_unverified":false,"is_reviewed":true}""")
                    req.RequestUri!.ToString().Contains(expectedUrl)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { })
            });

        // Act
        var result = await _sut.UpdateTranslation(_translationId, _translation, _projectId);

        // Assert
        result.Should().BeTrue();
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Put &&
                req.RequestUri!.ToString().Contains(expectedUrl)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task UpdateTranslation_ShouldCallApi_WithCorrectBody()
    {
        // Arrange
        var expectedBody = $$"""{"translation":"{{_translation}}","is_unverified":false,"is_reviewed":true}""";
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
                Content = JsonContent.Create(new { })
            });

        // Act
        var result = await _sut.UpdateTranslation(_translationId, _translation, _projectId);

        // Assert
        result.Should().BeTrue();
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Content.ReadAsStringAsync().Result.Equals(expectedBody)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task UpdateTranslation_ShouldReturnFalse_WhenUpdateFails()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        // Act
        var result = await _sut.UpdateTranslation(_translationId, _translation, _projectId);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task UpdateTranslation_ShouldLogError_WhenUpdateFails()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        // Act
        await _sut.UpdateTranslation(_translationId, _translation, _projectId);

        // Assert
        VerifyLog(LogLevel.Error, $"Failed to update translation {_translationId} in Lokalise with new value '{_translation}'");
    }
}