using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.Business.Models;

public class Translation
{
    [JsonPropertyName("translation_id")]
    public long Id { get; set; }

    [JsonPropertyName("segment_number")]
    public int SegmentNumber { get; set; }

    [JsonPropertyName("key_id")]
    public long KeyId { get; set; }

    [JsonPropertyName("language_iso")]
    public string LanguageIso { get; set; }

    [JsonPropertyName("translation")]
    public string TranslationText { get; set; }

    [JsonPropertyName("modified_by")]
    public int ModifiedBy { get; set; }

    [JsonPropertyName("modified_by_email")]
    public string ModifiedByEmail { get; set; }

    [JsonPropertyName("modified_at")]
    public string ModifiedAt { get; set; }

    [JsonPropertyName("modified_at_timestamp")]
    public long ModifiedAtTimestamp { get; set; }

    [JsonPropertyName("is_reviewed")]
    public bool IsReviewed { get; set; }

    [JsonPropertyName("reviewed_by")]
    public int ReviewedBy { get; set; }

    [JsonPropertyName("is_unverified")]
    public bool IsUnverified { get; set; }

    [JsonPropertyName("is_fuzzy")]
    public bool IsFuzzy { get; set; }

    [JsonPropertyName("words")]
    public int Words { get; set; }

    [JsonPropertyName("custom_translation_statuses")]
    public object[] CustomTranslationStatuses { get; set; }

    [JsonPropertyName("task_id")]
    public object TaskId { get; set; }

    [JsonPropertyName("is_untranslated")]
    public int IsUntranslated { get; set; }
}