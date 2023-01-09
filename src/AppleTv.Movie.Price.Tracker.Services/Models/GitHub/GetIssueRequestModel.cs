namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class GetIssueRequestModel : IOwnerRepoModel, IIssueNumberModel
{
    public string Owner { get; set; }
    public string Repo { get; set; }
    public long IssueNumber { get; set; }
}
