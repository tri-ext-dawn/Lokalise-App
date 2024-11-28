using Lokalise.ReviewComments.Business.Interfaces;

namespace Lokalise.ReviewComments.Business.Services;

public class CommandService : ICommandService
{
    private readonly ILokaliseApiClient _apiClient;
    private readonly ILokaliseCookieClient _cookieClient;

    public CommandService(ILokaliseApiClient apiClient, ILokaliseCookieClient cookieClient)
    {
        _apiClient = apiClient;
        _cookieClient = cookieClient;
    }
    
    public Task<bool> UpdateTranslation(long translationId, string translation, string projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    {
        return await _cookieClient.ResolveComment(commentId, translationId, projectId);
    }
}