using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

internal class CursorsData
{
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("after")]
    public string After { get; set; }

    [JsonPropertyName("before")]
    public string Before { get; set; }
}