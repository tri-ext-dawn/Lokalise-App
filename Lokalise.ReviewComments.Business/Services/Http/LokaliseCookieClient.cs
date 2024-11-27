using System.Text.Json;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;

namespace Lokalise.ReviewComments.Business.Services.Http;

public class LokaliseCookieClient : ILokaliseCookieClient
{
    private readonly ILogger<LokaliseCookieClient> _logger;
    private readonly HttpClient _client;

    public LokaliseCookieClient(ILogger<LokaliseCookieClient> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<List<Comment>> GetComments(string projectId)
    {
        try
        {
            var result = new List<Comment>();
            var nextUrl = $"collaboration/projects/{projectId}/comments?filter-resolved=0";
            while (nextUrl is not null)
            {
                var (comments, after) = await SendGetCommentsRequest(nextUrl);
                result.AddRange(comments);
                nextUrl = after;
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get comments from Lokalise");
            throw new HttpRequestException("Failed to get comments", e);
        }
    }

    private async Task<(List<Comment>, string? after)> SendGetCommentsRequest(string url)
    {
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CommentsData>(content);
        return (result.Comments, result.Meta.Paging.Cursors.After);
    }

    public Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    {
        throw new NotImplementedException();
    }
}