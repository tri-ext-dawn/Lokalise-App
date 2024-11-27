using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Interfaces;

public interface IDataService
{
    Task<List<Translation>> GetTranslations(long languageId, string projectId);
    Task<Language> GetLanguages(string projectId);
    Task<List<Comment>> GetComments(string projectId);
}