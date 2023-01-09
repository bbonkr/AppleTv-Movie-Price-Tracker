using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class SearchResultModel
{
    [JsonPropertyName("total_count")]
    public long TotalCount { get; set; }

    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; set; }

    [JsonPropertyName("items")]
    public List<SearchResultItemModel> Items { get; set; }
}

