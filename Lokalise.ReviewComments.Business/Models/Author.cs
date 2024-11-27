using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

public class Author
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("gravatarHash")]
    public string GravatarHash { get; set; }
}