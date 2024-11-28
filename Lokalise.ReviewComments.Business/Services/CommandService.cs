using Lokalise.ReviewComments.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lokalise.ReviewComments.Business.Services;

public class CommandService : ICommandService
{
    private readonly ILogger<CommandService> _logger;
    private readonly ILokaliseApiClient _apiClient;
    private readonly ILokaliseCookieClient _cookieClient;

    public CommandService(ILogger<CommandService> logger, ILokaliseApiClient apiClient, ILokaliseCookieClient cookieClient)
    {
        _logger = logger;
        _apiClient = apiClient;
        _cookieClient = cookieClient;
    }
    
    public Task<bool> UpdateTranslation(long translationId, string translation, string projectId)
    {
        _logger.LogInformation("Updating translation {translationId} with {translation}", translationId, translation);
        return _apiClient.UpdateTranslation(translationId, translation, projectId);
    }

    public async Task<bool> ResolveComment(string commentId, long translationId, string projectId)
    {
        _logger.LogInformation("Resolving comment {commentId} for translation {translationId}", commentId, translationId);
        return await _cookieClient.ResolveComment(commentId, translationId, projectId);
    }
}