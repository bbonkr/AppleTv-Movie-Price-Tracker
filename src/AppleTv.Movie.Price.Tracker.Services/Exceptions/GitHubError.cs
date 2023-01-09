using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Exceptions;

public class GitHubError
{
    public string Message { get; set; }

    [JsonPropertyName("documentation_url")]
    public string DocumentationUrl { get; set; }
}
