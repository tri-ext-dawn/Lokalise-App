using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Services;

public class DataService : IDataService
{
    private readonly ILokaliseApiClient _apiClient;
    private readonly ILokaliseCookieClient _cookieClient;

    public DataService(ILokaliseApiClient apiClient, ILokaliseCookieClient cookieClient)
    {
        _apiClient = apiClient;
        _cookieClient = cookieClient;
    }
    
    public Task<List<Translation>> GetTranslations(long languageId, string projectId)
    {
        return _apiClient.GetTranslations(languageId, projectId);
    }

    public async Task<List<Language>> GetLanguages(string projectId)
    {
        return await _apiClient.GetLanguages(projectId);
    }

    public async Task<List<Comment>> GetComments(string projectId)
    {
        return await _cookieClient.GetComments(projectId);
    }
}