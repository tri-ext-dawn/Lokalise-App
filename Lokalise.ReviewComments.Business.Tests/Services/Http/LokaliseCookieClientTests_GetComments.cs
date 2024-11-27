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
        SetupMockHandler(response, HttpStatusCode.OK);

        // Act
        var result = await _sut.GetComments(_projectId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        VerifyHttpRequest(_baseUrl, Times.Once());
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
        VerifyHttpRequest(_baseUrl, Times.Once());
        VerifyHttpRequest($"projects/{_projectId}/comments?page%5Bafter%5D={"nextPageToken"}&filter-resolved=0", Times.Once());
    }

    [Test]
    public async Task GetComments_WhenRequestFails_LogsError()
    {
        // Arrange
        SetupMockHandler("", HttpStatusCode.InternalServerError);

        // Act & Assert
        var ex = Assert.ThrowsAsync<HttpRequestException>(async () => 
            await _sut.GetComments(_projectId));
        
        VerifyLog(LogLevel.Error, "Failed to get comments from Lokalise");
    }

    [Test]
    public void GetComments_WhenRequestFails_ThrowsHttpRequestException()
    {
        // Arrange
        SetupMockHandler("", HttpStatusCode.InternalServerError);

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

    private void SetupMockHandler(string content, HttpStatusCode statusCode)
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

    private void VerifyHttpRequest(string url, Times times)
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            times,
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get && 
                req.RequestUri.ToString().Contains(url)),
            ItExpr.IsAny<CancellationToken>()
        );
    }
    
    
    
    // Tests for Task<List<Comment>> GetComments(string projectId)
    // - Should return comments
    // - Should follow the after link on path: https://app.lokalise.com/collaboration/projects/{{project_id}}/comments?page%5Bafter%5D={after}MTczMDkwNTI4OV9lMzVhZDg0OC03YTc4LTQ5OGItYjdjMy1jZDFiOThiODUyN2I}&filter-resolved=0
    // - Should log error when request fails
    // - Should throw HttpRequestException when request fails
    
    // Request URL GET: https://app.lokalise.com/collaboration/projects/{{project_id}}/comments?filter-resolved=0
    /* Response Example
{
       "data": [
               {
                   "id": "f521c5c2-d3fe-4234-b8c7-cd2414d7b73e",
                   "projectId": "7209912665b2702bce4eb3.46260463",
                   "author": {
                       "userId": 571187,
                       "name": "thomas.martinelli",
                       "gravatarHash": "c5ffbc2baee0f88cd0507ecb89d5d26c"
                   },
                   "message": "<p>Percentile de taille rapporté par la clinique</p>",
                   "attachPointType": "translations",
                   "attachPointId": "4708536431",
                   "attachPointName": "hcp.printReportClinicReportedHeightPercTitleText:French (France)",
                   "createdAt": 1732531069,
                   "modifiedAt": 1732531069,
                   "mentions": [],
                   "read": true,
                   "masterProjectId": "7209912665b2702bce4eb3.46260463",
                   "branchName": null,
                   "resolved": false,
                   "hasUnreadReplies": false,
                   "parentId": null,
                   "threadId": null,
                   "keyId": 555423205,
                   "langId": 706
               },
               {
                   "id": "e35ad848-7a78-498b-b7c3-cd1b98b8527b",
                   "projectId": "7209912665b2702bce4eb3.46260463",
                   "author": {
                       "userId": 571187,
                       "name": "thomas.martinelli",
                       "gravatarHash": "c5ffbc2baee0f88cd0507ecb89d5d26c"
                   },
                   "message": "<p>Aucun dispositif d'injection attribué</p>",
                   "attachPointType": "translations",
                   "attachPointId": "4683423607",
                   "attachPointName": "hcp.patientOverview.default.noDeviceAssignedText:French (France)",
                   "createdAt": 1730905289,
                   "modifiedAt": 1730905289,
                   "mentions": [],
                   "read": true,
                   "masterProjectId": "7209912665b2702bce4eb3.46260463",
                   "branchName": null,
                   "resolved": false,
                   "hasUnreadReplies": false,
                   "parentId": null,
                   "threadId": null,
                   "keyId": 457471925,
                   "langId": 706
               }
           ],
           "meta": {
               "paging": {
                   "count": 100,
                   "cursors": {
                       "size": 100,
                       "after": "MTczMDkwNTI4OV9lMzVhZDg0OC03YTc4LTQ5OGItYjdjMy1jZDFiOThiODUyN2I",
                       "before": "MTczMjUzMTA2OV9mNTIxYzVjMi1kM2ZlLTQyMzQtYjhjNy1jZDI0MTRkN2I3M2U"
                   },
                   "hasMore": true
               }
           }
       }
    */
}