using System.Text.Json;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;

namespace Lokalise.ReviewComments.Business.Services.Http;

public class LokaliseApiClient : ILokaliseApiClient
{
    private readonly ILogger<LokaliseApiClient> _logger;
    private readonly HttpClient _client;

    public LokaliseApiClient(ILogger<LokaliseApiClient> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    public async Task<List<Translation>> GetTranslations(long languageId, string projectId)
    {
        try
        {
            var url = $"projects/{projectId}/translations?filter_lang_id={languageId}&limit=2000&page=1";
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var translations = JsonSerializer.Deserialize<TranslationsData>(content).Translations;
            return translations;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get translations from Lokalise");
            throw new HttpRequestException("Failed to get translations", e);
        }
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