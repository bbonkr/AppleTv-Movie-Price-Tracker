using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class GitHubUserModel
{
    public string Login { get; set; }
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }
    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }
    [JsonPropertyName("followers_url")]
    public string FollowersUrl { get; set; }
    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; }
    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; }
    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; }
    [JsonPropertyName("subscriptions_url")]
    public string SubscriptionsUrl { get; set; }
    [JsonPropertyName("organizations_url")]
    public string OrganizationsUrl { get; set; }
    [JsonPropertyName("repos_url")]
    public string ReposUrl { get; set; }
    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; }
    [JsonPropertyName("received_events_url")]
    public string ReceivedEventsUrl { get; set; }
    public string Type { get; set; }
    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }
}

