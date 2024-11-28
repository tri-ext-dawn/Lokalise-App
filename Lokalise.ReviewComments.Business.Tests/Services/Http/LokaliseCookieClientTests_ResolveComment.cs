using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

public class LokaliseCookieClientTests_ResolveComment : LokaliseCookieClientTests
{
    private const string ProjectId = "123456";
    private const string CommentId = "789";
    private const long TranslationId = 4850905278;

    [Test]
    public async Task ResolveComment_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.NoContent);

        // Act
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResolveComment_CallCorrectEndpoint()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.NoContent);

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
        SetupHttpMessageHandler(HttpStatusCode.NoContent);

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
        SetupHttpMessageHandler(HttpStatusCode.BadRequest);

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
        SetupHttpMessageHandlerToThrow(new HttpRequestException());

        // Act
        var result = await _sut.ResolveComment(CommentId, TranslationId, ProjectId);

        // Assert
        Assert.That(result, Is.False);
        VerifyLog(LogLevel.Error, $"Failed to resolve comment {CommentId} for translation {TranslationId} in Lokalise");
    }

    private void SetupHttpMessageHandler(HttpStatusCode statusCode, string responseBody = "")
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
                StatusCode = statusCode,
                Content = new StringContent(responseBody)
            });
    }

    private void SetupHttpMessageHandlerToThrow(Exception exception)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(exception);
    }
}
