using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

public class LokaliseCookieClientTests_ResolveComment : LokaliseCookieClientTests
{
    // Tests for Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    // Example url PATCH: https://app.lokalise.com/collaboration/projects/{projectId/translations/4850905278/comments/{commentId}
    // - Should return true when the comment is resolved
    // - Should return false when the comment is not resolved
    // - Should return false when the request fails
    // - Should return true when the comment is already resolved
    
    private const string ProjectId = "123456";
    private const string CommentId = "789";
    private const long TranslationId = 4850905278;

    [Test]
    public async Task ResolveComment_WhenSuccessful_ReturnsTrue()
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
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResolveComment_CallCorrectEndpoint()
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
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Patch &&
                req.RequestUri.ToString().Contains($"projects/{ProjectId}/translations/{TranslationId}/comments/{CommentId}")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task ResolveComment_CorrectBody()
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
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Content.ReadAsStringAsync().Result.Contains("\"resolved\":true")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task ResolveComment_WhenRequestFails_ReturnsFalse()
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
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        Assert.That(result, Is.False);
        VerifyLog(LogLevel.Error, $"Failed to resolve comment {CommentId} for translation {TranslationId} in Lokalise");
    }

    [Test]
    public async Task ResolveComment_WhenNetworkError_ReturnsFalse()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        // Act
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        Assert.That(result, Is.False);
        VerifyLog(LogLevel.Error, $"Failed to resolve comment {CommentId} for translation {TranslationId} in Lokalise");
    }
}
