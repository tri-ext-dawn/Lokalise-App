using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Services.Http;

public class LokaliseApiClient : ILokaliseApiClient
{
    private readonly HttpClient _client;

    public LokaliseApiClient(HttpClient client)
    {
        _client = client;
    }
    
    public Task<List<Translation>> GetTranslations(long languageId, string projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTranslation(long translationId, string translation, string projectId)
    {
        throw new NotImplementedException();
    }

    public Task<Language> GetLanguages(string projectId)
    {
        throw new NotImplementedException();
    }
}