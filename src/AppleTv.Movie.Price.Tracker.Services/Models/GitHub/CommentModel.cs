using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class CommentModel
{
    [JsonPropertyName("id")]
    public long Id { get; set; }


    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }


    [JsonPropertyName("url")]
    public string Url { get; set; }


    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }


    [JsonPropertyName("body")]
    public string Body { get; set; }


    [JsonPropertyName("user")]
    public GitHubUserModel? User { get; set; }


    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }


    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }


    [JsonPropertyName("issue_url")]
    public string IssueUrl { get; set; }


    [JsonPropertyName("author_association")]
    public string AuthorAssociation { get; set; }
}

