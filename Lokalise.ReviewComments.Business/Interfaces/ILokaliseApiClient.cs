using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Interfaces;

public interface ILokaliseApiClient
{
    Task<List<Translation>> GetTranslations(long languageId, string projectId);
    Task<bool> UpdateTranslation(long translationId, string translation, string projectId);
    Task<List<Language>> GetLanguages(string projectId);
}