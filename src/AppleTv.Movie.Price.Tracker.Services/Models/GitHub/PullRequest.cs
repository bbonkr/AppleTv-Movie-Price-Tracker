using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class PullRequestModel
{
    public string Url { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }
    [JsonPropertyName("diff_url")]
    public string? DiffUrl { get; set; }
    [JsonPropertyName("patch_url")]
    public string? PatchUrl { get; set; }
}

