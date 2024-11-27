namespace Lokalise.ReviewComments.Business.Interfaces;

public interface ICommandService
{
    Task<bool> UpdateTranslation(long translationId, string translation, string projectId);
    Task<bool> ResolveComment(string commentId, long translationId, string projectId);
}