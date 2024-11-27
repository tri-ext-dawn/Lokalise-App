using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Services.Http;

public class LokaliseCookieClient : ILokaliseCookieClient
{
    private readonly HttpClient _client;

    public LokaliseCookieClient(HttpClient client)
    {
        _client = client;
    }

    public Task<List<Comment>> GetComments(string projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    {
        throw new NotImplementedException();
    }
}