using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class MilestoneModel
{
    public string Url { get; set; }
    [JsonPropertyName("html_url")]
    public string Html_Url { get; set; }
    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; }
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    public long Number { get; set; }
    public string State { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public GitHubUserModel Creator { get; set; }
    [JsonPropertyName("open_issues")]
    public long OpenIssues { get; set; }
    [JsonPropertyName("closed_issues")]
    public long ClosedIssues { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }
    [JsonPropertyName("due_on")]
    public DateTime? DueOn { get; set; }
}

