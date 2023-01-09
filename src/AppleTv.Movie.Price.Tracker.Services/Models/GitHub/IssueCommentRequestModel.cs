namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class IssueCommentRequestModel : IOwnerRepoModel, IIssueNumberModel
{
    public string Owner { get; set; }

    public string Repo { get; set; }

    public long IssueNumber { get; set; }

    public string Body { get; set; }
}