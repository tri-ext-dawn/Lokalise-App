using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

internal class TranslationsData
{
    [JsonPropertyName("translations")]
    public List<Translation> Translations { get; set; }
}