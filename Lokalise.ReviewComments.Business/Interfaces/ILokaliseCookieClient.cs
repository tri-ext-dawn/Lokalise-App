using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business.Interfaces;

public interface ILokaliseCookieClient
{
    Task<List<Comment>> GetComments(string projectId);
    Task<bool> ResolveComment(string commentId, long translationId, string projectId);
}