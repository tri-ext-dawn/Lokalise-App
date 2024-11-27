using System.Text.Json;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Lokalise.ReviewComments.Business.Services.Http;

public class LokaliseCookieClient : ILokaliseCookieClient
{
    private readonly ILogger<LokaliseCookieClient> _logger;
    private readonly LokaliseSettings _options;

    public LokaliseCookieClient(ILogger<LokaliseCookieClient> logger, IOptions<LokaliseSettings> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<Comment>> GetComments(string projectId)
    {
        try
        {
            var result = new List<Comment>();
            var nextUrl = $"{_options.BaseCookieUrl}projects/{projectId}/comments?filter-resolved=0";
            while (nextUrl is not null)
            {
                var (comments, after) = await SendGetCommentsRequest(nextUrl);
                result.AddRange(comments);
                nextUrl = after is not null ? $"{_options.BaseCookieUrl}projects/{projectId}/comments?page%5Bafter%5D={after}&filter-resolved=0" : null;
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
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "/*");
        foreach (var cookie in _options.Cookies.Reverse())
        {
            request.AddCookie(cookie.Key, cookie.Value, "", _options.AppDomain);
        }            
        
        var response = await client.GetAsync(request);
        var content = response.Content;
        var result = JsonSerializer.Deserialize<CommentsData>(content);
        return (result.Comments, result.Meta.Paging.Cursors.After);
    }

    public Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    {
        throw new NotImplementedException();
    }
}