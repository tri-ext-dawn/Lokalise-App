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
        throw new NotImplementedException();
    }

    public Task<Language> GetLanguages(string projectId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Comment>> GetComments(string projectId)
    {
        throw new NotImplementedException();
    }
}