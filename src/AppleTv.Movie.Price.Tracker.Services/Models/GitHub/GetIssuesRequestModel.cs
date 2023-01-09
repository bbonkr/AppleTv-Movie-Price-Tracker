namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class GetIssuesRequestModel : IOwnerRepoModel
{
    public string Owner { get; set; }
    public string Repo { get; set; }

    /// <summary>
    /// <para>
    /// If an integer is passed, it should refer to a milestone by its number field. 
    /// </para>
    /// <para>
    /// If the string `*` is passed, issues with any milestone are accepted. 
    /// </para>
    /// <para>
    /// If the string `none` is passed, issues without milestones are returned.
    /// </para>
    /// Please <see cref="GetIssueMilestones"/> fields
    /// </summary>
    public string? Milestone { get; set; }

    /// <summary>
    /// Indicates the state of the issues to return.
    /// 
    /// <para>
    /// Please <see cref="GetIssueStates"/> fields
    /// </para>
    /// </summary>
    public string State { get; set; } = GetIssueStates.Open;

    /// <summary>
    /// <para>
    /// Can be the name of a user. 
    /// </para>
    /// <para>
    /// Pass in none for issues with no assigned user, and * for issues assigned to any user.
    /// </para>
    /// <para>
    /// Please <see cref="GetIssueAssginees"/> fields
    /// </para>
    /// </summary>
    public string? Assignee { get; set; }

    /// <summary>
    /// The user that created the issue.
    /// </summary>
    public string? Creator { get; set; }

    /// <summary>
    /// A user that's mentioned in the issue.
    /// </summary>
    public string? Mentioned { get; set; }

    /// <summary>
    /// <para>
    /// A list of comma separated label names. 
    /// </para>
    /// </summary>
    public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// What to sort results by.
    /// </summary>
    public string Sort { get; set; } = GetIssueSorts.Created;

    /// <summary>
    /// <para>
    /// The direction to sort the results by.
    /// </para>
    /// <para>
    /// Please <see cref="GetIssueSortDirection"/> fields
    /// </para>
    /// </summary>
    public string Direction { get; set; } = GetIssueSortDirection.Descending;

    /// <summary>
    /// Only show notifications updated after the given time
    /// </summary>
    public DateTimeOffset? Since { get; set; }

    /// <summary>
    /// The number of results per page (max 100).
    /// </summary>
    public int Limit { get; set; } = 20;

    /// <summary>
    /// Page number of the results to fetch.
    /// </summary>
    public long Page { get; set; } = 1;
}
