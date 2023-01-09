using System.Text.Json.Serialization;

namespace AppleTv.Movie.Price.Tracker.Services.Models.GitHUb;

public class LabelModel
{
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    [JsonPropertyName("@default")]
    public bool Default { get; set; }
}

