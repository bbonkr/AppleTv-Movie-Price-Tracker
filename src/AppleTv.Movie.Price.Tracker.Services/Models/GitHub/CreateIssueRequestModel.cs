using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class CreateIssueRequestModel : IOwnerRepoModel
{
    [JsonIgnore]
    public string Owner { get; set; }
    [JsonIgnore]
    public string Repo { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public IEnumerable<string> Assignees { get; set; }

    public string? Milestone { get; set; }

    public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();
}
