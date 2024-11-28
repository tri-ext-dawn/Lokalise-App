using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Interfaces;

public interface IWorkflowService
{
    Task<string> SelectProject();
    Task<Language> SelectLanguage(List<Language> languages);
    Task<bool> ProcessComment(Comment comment, Translation translation, string projectId);
}