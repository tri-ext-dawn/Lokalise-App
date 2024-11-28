using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

[TestFixture]
[Category("Integration")]
public class LokaliseApiClientTests_UpdateTranslation : LokaliseApiClientTests
{
    private const string TestProjectId = "123456";
    private const long TestTranslationId = 789;
    private const string TestTranslation = "Updated translation";
    private const string ExpectedJsonBody = """{"translation":"Updated translation","is_unverified":false,"is_reviewed":true}""";

    private string ExpectedUrl => $"projects/{TestProjectId}/translations/{TestTranslationId}";

    [SetUp]
    public void SetUp()
    {
        SetupDefaultHttpHandler();
    }

    private void SetupDefaultHttpHandler(HttpStatusCode statusCode = HttpStatusCode.OK)
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
                Content = JsonContent.Create(new { })
            });
    }

    [Test]
    public async Task UpdateTranslation_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.OK);

        // Act
        var result = await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        result.Should().BeTrue();
        VerifyHttpRequest(ExpectedUrl, HttpMethod.Put, Times.Once());
    }

    [Test]
    public async Task UpdateTranslation_WithValidRequest_ShouldSendCorrectRequestBody()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.OK);

        // Act
        var result = await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        result.Should().BeTrue();
        VerifyRequestBody(ExpectedJsonBody);
    }

    [Test]
    public async Task UpdateTranslation_WhenApiFails_ShouldReturnFalse()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.BadRequest);

        // Act
        var result = await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task UpdateTranslation_WhenApiFails_ShouldLogError()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.BadRequest);

        // Act
        await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        VerifyLog(LogLevel.Error, $"Failed to update translation {TestTranslationId} in Lokalise with new value '{TestTranslation}'");
    }
}