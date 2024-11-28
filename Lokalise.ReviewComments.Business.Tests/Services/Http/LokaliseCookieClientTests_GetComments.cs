using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Lokalise.ReviewComments.Business.Tests.Services.Http;

public class LokaliseCookieClientTests_GetComments : LokaliseCookieClientTests 
{
    private string _projectId;
    private string _baseUrl;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _projectId = "7209912665b2702bce4eb3.46260463";
        _baseUrl = $"projects/{_projectId}/comments?filter-resolved=0";
    }

    [Test]
    public async Task GetComments_Success_ReturnsComments()
    {
        // Arrange
        var response = CreateMockResponse(1, false);
        SetupHttpMessageHandler(HttpStatusCode.OK, response);

        // Act
        var result = await _sut.GetComments(_projectId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        VerifyHttpRequest(_baseUrl, HttpMethod.Get, Times.Once());
    }

    [Test]
    public async Task GetComments_WithPagination_ReturnsAllComments()
    {
        // Arrange
        var firstResponse = CreateMockResponse(1, true);
        var secondResponse = CreateMockResponse(2, false);
        
        _mockHttpMessageHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(firstResponse, Encoding.UTF8, "application/json")
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(secondResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _sut.GetComments(_projectId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(4));
        VerifyHttpRequest(_baseUrl, HttpMethod.Get, Times.Once());
        VerifyHttpRequest($"projects/{_projectId}/comments?page%5Bafter%5D={"nextPageToken"}&filter-resolved=0", HttpMethod.Get, Times.Once());
    }

    [Test]
    public async Task GetComments_WhenRequestFails_LogsError()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.InternalServerError, "");

        // Act & Assert
        var ex = Assert.ThrowsAsync<HttpRequestException>(async () => 
            await _sut.GetComments(_projectId));
        
        VerifyLog(LogLevel.Error, "Failed to get comments from Lokalise");
    }

    [Test]
    public void GetComments_WhenRequestFails_ThrowsHttpRequestException()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.InternalServerError, "");

        // Act & Assert
        Assert.ThrowsAsync<HttpRequestException>(async () => 
            await _sut.GetComments(_projectId));
    }

    private string CreateMockResponse(int page, bool hasMore)
    {
        var response = new
        {
            data = new[]
            {
                new
                {
                    id = $"id{page}1",
                    projectId = _projectId,
                    message = "Test message 1",
                    resolved = false
                },
                new
                {
                    id = $"id{page}2",
                    projectId = _projectId,
                    message = "Test message 2",
                    resolved = false
                }
            },
            meta = new
            {
                paging = new
                {
                    cursors = new
                    {
                        after = hasMore ? "nextPageToken" : null,
                    },
                    hasMore = hasMore
                }
            }
        };

        return JsonSerializer.Serialize(response);
    }
}