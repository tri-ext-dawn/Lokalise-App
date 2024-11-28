using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

public class Language : IComparable<Language>
{
    [JsonPropertyName("lang_id")] 
    public long Id { get; set; }
    [JsonPropertyName("lang_name")] 
    public string Name { get; set; }
    [JsonPropertyName("lang_iso")] 
    public string ISO { get; set; }

    public int CompareTo(Language? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }
}