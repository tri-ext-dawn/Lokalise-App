using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

internal class CommentsData
{
    [JsonPropertyName("data")]
    public List<Comment> Comments { get; set; }

    [JsonPropertyName("meta")]
    public MetaData Meta { get; set; }
}