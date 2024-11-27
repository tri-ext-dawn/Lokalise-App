using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

internal class MetaData
{
    [JsonPropertyName("paging")]
    public PagingData Paging { get; set; }
}