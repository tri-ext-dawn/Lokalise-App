using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lokalise.ReviewComments.App.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Comment
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; }

    [JsonPropertyName("author")]
    public Author Author { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("attachPointType")]
    public string AttachPointType { get; set; }

    [JsonPropertyName("attachPointId")]
    public string AttachPointId { get; set; }

    [JsonPropertyName("attachPointName")]
    public string AttachPointName { get; set; }

    [JsonPropertyName("createdAt")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    public long ModifiedAt { get; set; }

    [JsonPropertyName("mentions")]
    public List<object> Mentions { get; set; }

    [JsonPropertyName("read")]
    public bool Read { get; set; }

    [JsonPropertyName("masterProjectId")]
    public string MasterProjectId { get; set; }

    [JsonPropertyName("branchName")]
    public string BranchName { get; set; }

    [JsonPropertyName("resolved")]
    public bool Resolved { get; set; }

    [JsonPropertyName("hasUnreadReplies")]
    public bool HasUnreadReplies { get; set; }

    [JsonPropertyName("parentId")]
    public string ParentId { get; set; }

    [JsonPropertyName("threadId")]
    public string ThreadId { get; set; }

    [JsonPropertyName("keyId")]
    public long KeyId { get; set; }

    [JsonPropertyName("langId")]
    public int LangId { get; set; }
    
    public static List<Comment> Comments
    {
        get
        {
            var json = System.IO.File.ReadAllText("Data/Comments.json");
            return JsonSerializer.Deserialize<CommentsData>(json).Data;
        }
    }
}

public class CommentsData
{
    [JsonPropertyName("data")]
    public List<Comment> Data { get; set; }
}
