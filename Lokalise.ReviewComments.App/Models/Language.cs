using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.App.Models;

public class Language
{
    [JsonPropertyName("lang_id")] public long Id { get; set; }
    [JsonPropertyName("lang_name")] public string Name { get; set; }
    [JsonPropertyName("lang_iso")] public string ISO { get; set; }

    public static List<Language> Languages => JsonSerializer.Deserialize<LanguagesClass>(_languagesJsonString).languages;

    class LanguagesClass
    {
        public List<Language> languages { get; set; }
    }
    
    private static string _languagesJsonString = @"{
    ""project_id"": ""3174716666ba0500034d17.77744948"",
    ""languages"": [
        {
            ""lang_id"": 716,
            ""lang_iso"": ""ar_BH"",
            ""lang_name"": ""Arabic (Bahrain)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 717,
            ""lang_iso"": ""ar_EG"",
            ""lang_name"": ""Arabic (Egypt)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 720,
            ""lang_iso"": ""ar_KW"",
            ""lang_name"": ""Arabic (Kuwait)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 723,
            ""lang_iso"": ""ar_MA"",
            ""lang_name"": ""Arabic (Morocco)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 725,
            ""lang_iso"": ""ar_QA"",
            ""lang_name"": ""Arabic (Qatar)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 726,
            ""lang_iso"": ""ar_SA"",
            ""lang_name"": ""Arabic (Saudi Arabia)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 730,
            ""lang_iso"": ""ar_AE"",
            ""lang_name"": ""Arabic (United Arab Emirates)"",
            ""is_rtl"": true,
            ""plural_forms"": [
                ""zero"",
                ""one"",
                ""two"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 602,
            ""lang_iso"": ""zh_CN"",
            ""lang_name"": ""Chinese Simplified (Hong Kong)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""other""
            ]
        },
        {
            ""lang_id"": 601,
            ""lang_iso"": ""zh_TW"",
            ""lang_name"": ""Chinese Traditional (Taiwan)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""other""
            ]
        },
        {
            ""lang_id"": 603,
            ""lang_iso"": ""zh_HK"",
            ""lang_name"": ""Chinese Traditional (Hong Kong)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""other""
            ]
        },
        {
            ""lang_id"": 10167,
            ""lang_iso"": ""cs_CZ"",
            ""lang_name"": ""Czech (Czech Republic)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 612,
            ""lang_iso"": ""en_CA"",
            ""lang_name"": ""English (Canada)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 610,
            ""lang_iso"": ""en_GB"",
            ""lang_name"": ""English"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 10156,
            ""lang_iso"": ""fi_FI"",
            ""lang_name"": ""Finnish (Finland)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 674,
            ""lang_iso"": ""fr_CA"",
            ""lang_name"": ""French (Canada)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 706,
            ""lang_iso"": ""fr_FR"",
            ""lang_name"": ""French (France)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 10181,
            ""lang_iso"": ""fr_MA"",
            ""lang_name"": ""French (Morocco)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 675,
            ""lang_iso"": ""fr_CH"",
            ""lang_name"": ""French (Switzerland)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 670,
            ""lang_iso"": ""de_AT"",
            ""lang_name"": ""German (Austria)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 672,
            ""lang_iso"": ""de_DE"",
            ""lang_name"": ""German (Germany)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 667,
            ""lang_iso"": ""de_CH"",
            ""lang_name"": ""German (Switzerland)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 763,
            ""lang_iso"": ""el_GR"",
            ""lang_name"": ""Greek (Greece)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 10171,
            ""lang_iso"": ""hu_HU"",
            ""lang_name"": ""Hungarian (Hungary)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 736,
            ""lang_iso"": ""it_IT"",
            ""lang_name"": ""Italian (Italy)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 10173,
            ""lang_iso"": ""ko_KR"",
            ""lang_name"": ""Korean (South Korea​)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""other""
            ]
        },
        {
            ""lang_id"": 751,
            ""lang_iso"": ""ms_MY"",
            ""lang_name"": ""Malay (Malaysia)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""other""
            ]
        },
        {
            ""lang_id"": 708,
            ""lang_iso"": ""pt_BR"",
            ""lang_name"": ""Portuguese (Brazil)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 707,
            ""lang_iso"": ""pt_PT"",
            ""lang_name"": ""Portuguese (Portugal)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 770,
            ""lang_iso"": ""ro"",
            ""lang_name"": ""Romanian (Romanian)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""few"",
                ""other""
            ]
        },
        {
            ""lang_id"": 783,
            ""lang_iso"": ""sr"",
            ""lang_name"": ""Serbian (Serbia)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""few"",
                ""other""
            ]
        },
        {
            ""lang_id"": 10176,
            ""lang_iso"": ""sk_SK"",
            ""lang_name"": ""Slovak (Slovakia)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""few"",
                ""many"",
                ""other""
            ]
        },
        {
            ""lang_id"": 649,
            ""lang_iso"": ""es_AR"",
            ""lang_name"": ""Spanish (Argentina)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 646,
            ""lang_iso"": ""es_CL"",
            ""lang_name"": ""Spanish (Chile)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 656,
            ""lang_iso"": ""es_EC"",
            ""lang_name"": ""Spanish (Ecuador)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 659,
            ""lang_iso"": ""es_GT"",
            ""lang_name"": ""Spanish (Guatemala)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 648,
            ""lang_iso"": ""es_MX"",
            ""lang_name"": ""Spanish (Mexico)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 652,
            ""lang_iso"": ""es_PE"",
            ""lang_name"": ""Spanish (Peru)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 665,
            ""lang_iso"": ""es_ES"",
            ""lang_name"": ""Spanish (Spain)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 756,
            ""lang_iso"": ""sv_SE"",
            ""lang_name"": ""Swedish (Sweden)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""one"",
                ""other""
            ]
        },
        {
            ""lang_id"": 10178,
            ""lang_iso"": ""th_TH"",
            ""lang_name"": ""Thai (Thailand)"",
            ""is_rtl"": false,
            ""plural_forms"": [
                ""other""
            ]
        }
    ]
}";
}