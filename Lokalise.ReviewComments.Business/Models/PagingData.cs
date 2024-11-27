using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

internal class PagingData
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("cursors")]
    public CursorsData Cursors { get; set; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }
}