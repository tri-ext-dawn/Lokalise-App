using System.Net.Http.Json;
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

    public async Task<bool> UpdateTranslation(long translationId, string translation, string projectId)
    {
        var payload = new
        {
            translation = translation,
            is_unverified = false,
            is_reviewed = true
        };
        
        var url = $"projects/{projectId}/translations/{translationId}";
        var response = await _client.PutAsync(url, JsonContent.Create(payload));
        if (response.IsSuccessStatusCode is false)
        {
            _logger.LogError("Failed to update translation {translationId} in Lokalise with new value '{translation}'", translationId, translation);
            return false;
        }
        return true;
    }

    public async Task<List<Language>> GetLanguages(string projectId)
    {
        try
        {
            var url = $"projects/{projectId}/languages";
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var languages = JsonSerializer.Deserialize<LanguagesData>(content).Languages;
            return languages;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get languages from Lokalise");
            throw new HttpRequestException("Failed to get languages", e);
        }
    }
}