using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

public class Language
{
    [JsonPropertyName("lang_id")] 
    public long Id { get; set; }
    [JsonPropertyName("lang_name")] 
    public string Name { get; set; }
    [JsonPropertyName("lang_iso")] 
    public string ISO { get; set; }
}