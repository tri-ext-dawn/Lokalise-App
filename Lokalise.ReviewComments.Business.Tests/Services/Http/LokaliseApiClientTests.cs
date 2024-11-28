using System.Net;
using System.Net.Http.Json;
using System.Text;
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
    protected Fixture _fixture;
    protected Mock<ILogger<LokaliseApiClient>> _mockLogger;
    protected Mock<HttpMessageHandler> _mockHttpMessageHandler;
    protected HttpClient _httpClient;
    protected ILokaliseApiClient _sut;
    protected void VerifyLog(LogLevel logLevel, string message, int times = 1) => Utilities.VerifyLog(_mockLogger, logLevel, message, times);
    protected T DeepClone<T>(T source) => Utilities.DeepClone(source);

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

    protected void SetupHttpMessageHandler(HttpStatusCode statusCode, string content = "")
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    protected void VerifyHttpRequest(string url, HttpMethod method, Times times)
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            times,
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == method && 
                req.RequestUri.ToString().Contains(url)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    protected void VerifyRequestBody(string expectedJsonBody)
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Content.ReadAsStringAsync().Result.Equals(expectedJsonBody)),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}