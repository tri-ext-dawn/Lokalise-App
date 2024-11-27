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
    private HttpResponseMessage SuccessResponse => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = JsonContent.Create(new { })
    };

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
        SetupHttpHandlerForSuccessfulRequest();

        // Act
        var result = await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        result.Should().BeTrue("because the API call was successful");
        VerifyApiWasCalled();
    }

    [Test]
    public async Task UpdateTranslation_WithValidRequest_ShouldSendCorrectRequestBody()
    {
        // Arrange
        SetupDefaultHttpHandler();

        // Act
        var result = await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        result.Should().BeTrue();
        VerifyRequestBody();
    }

    [Test]
    public async Task UpdateTranslation_WhenApiFails_ShouldReturnFalse()
    {
        // Arrange
        SetupDefaultHttpHandler(HttpStatusCode.BadRequest);

        // Act
        var result = await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        result.Should().BeFalse("because the API returned a failure status code");
    }

    [Test]
    public async Task UpdateTranslation_WhenApiFails_ShouldLogError()
    {
        // Arrange
        SetupDefaultHttpHandler(HttpStatusCode.BadRequest);

        // Act
        await _sut.UpdateTranslation(TestTranslationId, TestTranslation, TestProjectId);

        // Assert
        VerifyErrorWasLogged();
    }

    private void SetupHttpHandlerForSuccessfulRequest()
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put && 
                    req.RequestUri!.ToString().Contains(ExpectedUrl)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(SuccessResponse);
    }

    private void VerifyApiWasCalled()
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Put &&
                req.RequestUri!.ToString().Contains(ExpectedUrl)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private void VerifyRequestBody()
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Content.ReadAsStringAsync().Result.Equals(ExpectedJsonBody)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private void VerifyErrorWasLogged()
    {
        VerifyLog(
            LogLevel.Error, 
            $"Failed to update translation {TestTranslationId} in Lokalise with new value '{TestTranslation}'"
        );
    }
}