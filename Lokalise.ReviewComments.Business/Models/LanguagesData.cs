using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

internal class LanguagesData
{
    [JsonPropertyName("languages")] 
    public List<Language> Languages { get; set; }
}