using System.Net;
using System.Text.Json;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            var nextUrl = $"projects/{projectId}/comments?filter-resolved=0";
            while (nextUrl is not null)
            {
                var (comments, after) = await SendGetCommentsRequest(nextUrl);
                result.AddRange(comments);
                nextUrl = after is not null ? $"projects/{projectId}/comments?page%5Bafter%5D={after}&filter-resolved=0" : null;
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

    public async Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    {
        try
        {
            var url = $"projects/{projectId}/translations/{translationId}/comments/{commentId}";
            var body = $$"""{"resolved":true}""";
            var response = await _client.PatchAsync(url, new StringContent(body, System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode is false)
            {
                _logger.LogError("Failed to resolve comment {commentId} for translation {translationId} in Lokalise", commentId, translationId);
                return false;                    
            }
            
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to resolve comment {commentId} for translation {translationId} in Lokalise", commentId, translationId);
            return false;
        }
    }
}