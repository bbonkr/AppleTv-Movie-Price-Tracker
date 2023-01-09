namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class SearchIssueRequestModel
{
    public string? Owner { get; set; }

    public string? Repo { get; set; }

    public string Q { get; set; }

    public string? Sort { get; set; } = SearchSorts.Created;

    public string? State { get; set; }

    public string? Order { get; set; } = GetIssueSortDirection.Descending;

    public int Limit { get; set; } = 20;

    public int Page { get; set; } = 1;

    public bool SearchInBody { get; set; } = false;
    public bool SearchInComments { get; set; } = false;
}
