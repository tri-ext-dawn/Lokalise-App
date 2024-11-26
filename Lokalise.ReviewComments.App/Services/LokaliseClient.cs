using System.Text.Json;
using System.Transactions;
using Lokalise.ReviewComments.App.Models;
using RestSharp;

namespace Lokalise.ReviewComments.App.Services;

public class LokaliseClient
{
    private string _projectId = "3174716666ba0500034d17.77744948";
    private string _apiToken = "b0b811c7096c17c3151771a42d3cc63ef2080812";

    private string _xCsrfToken = "7YuA05Q-_IftP9EtQ-O-Jka-CvktQNhJavT6LS1KxUo";
    private Dictionary<string, string> cookies = new Dictionary<string, string>
    {
        { "PHPSESSID", "bsbkpnoqdckc215ve1o8esrvst33h8pp" },
        { "csrf_token", "7YuA05Q-_IftP9EtQ-O-Jka-CvktQNhJavT6LS1KxUo"}
    };

    public async Task<List<Translation>> GetTranslations()
    {
        var options = new RestClientOptions($"https://api.lokalise.com/api2/projects/{_projectId}/translations?filter_lang_id=665&limit=2000&page=1");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("X-Api-Token", _apiToken);
        request.AddHeader("content-type", "application/json");
        var response = await client.GetAsync(request);
        var res = JsonSerializer.Deserialize<TranslationsData>(response.Content).Translations;
        return res;
    }

    public async Task<bool> UpdateTranslation(long translationId, string translation)
    {
        try
        {
            var options =
                new RestClientOptions($"https://api.lokalise.com/api2/projects/{_projectId}/translations/{translationId}");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("X-Api-Token", _apiToken);
            request.AddJsonBody($$"""{"translation":"{{translation}}","is_unverified":false,"is_reviewed":true}""",
                false);
            var response = await client.PutAsync(request);
            if (response.IsSuccessful)
                return true;
            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> ResolveComment(string commentId, long translationId)
    {
        try
        {
            var options = new RestClientOptions($"https://app.lokalise.com/collaboration/projects/{_projectId}/translations/{translationId}/comments/{commentId}");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "/*");
            request.AddHeader("x-csrf-token", _xCsrfToken);
            foreach (var cookie in cookies)
            {
                request.AddCookie(cookie.Key, cookie.Value, "", "app.lokalise.com");
            }            
            request.AddJsonBody($$"""{"resolved":true}""", false);
            var response = await client.PatchAsync(request);
            if (response.IsSuccessful)
                return true;
            return false;
        }
        catch (Exception e)
        {
            if (e.Message.Equals("Request failed with status code UnprocessableEntity", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Comment already resolved");
                return true;
            }
            Console.WriteLine($"Message: {e.Message}");
            return false;
        }
    }

    public async Task<List<Comment>> GetComments()
    {
        try
        {
            var options = new RestClientOptions($"https://app.lokalise.com/collaboration/projects/{_projectId}/comments?filter-resolved=0");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "/*");
            foreach (var cookie in cookies)
            {
                request.AddCookie(cookie.Key, cookie.Value, "", "app.lokalise.com");
            }            
            var response = await client.GetAsync(request);
            var res = JsonSerializer.Deserialize<CommentsData>(response.Content);
            return res.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Message: {e.Message}");
            return new List<Comment>();
        }
    }
}