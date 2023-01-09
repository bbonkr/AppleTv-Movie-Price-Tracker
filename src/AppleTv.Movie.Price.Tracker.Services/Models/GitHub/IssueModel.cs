using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class IssueModel
{
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("repository_url")]
    public string RepositoryUrl { get; set; }
    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; }
    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; }
    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }
    public long Number { get; set; }
    public string State { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public GitHubUserModel? User { get; set; }
    public List<LabelModel> Labels { get; set; } = new();
    public GitHubUserModel? Assignee { get; set; }
    public List<GitHubUserModel> Assignees { get; set; } = new();
    public MilestoneModel? Milestone { get; set; }
    public bool Locked { get; set; }
    [JsonPropertyName("active_lock_reason")]
    public string ActiveLockReason { get; set; }
    public long Comments { get; set; }
    [JsonPropertyName("pull_request")]
    public PullRequestModel? PullRequest { get; set; }
    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [JsonPropertyName("closed_by")]
    public GitHubUserModel? ClosedBy { get; set; }
    [JsonPropertyName("author_association")]
    public string AuthorAssociation { get; set; }
    [JsonPropertyName("state_reason")]
    public string StateReason { get; set; }
}

