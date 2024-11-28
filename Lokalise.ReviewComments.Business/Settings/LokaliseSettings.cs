namespace Lokalise.ReviewComments.Business;

public class LokaliseSettings
{
    public string ApiToken { get; set; }
    public string BaseApiUrl { get; set; }
    public string BaseCookieUrl { get; set; }
    public string AppDomain { get; set; }
    public string XCsrfToken { get; set; }
    public Dictionary<string, string> Cookies { get; set; }
}