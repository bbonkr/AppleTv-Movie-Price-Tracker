using System;

namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class MovieModel : MovieListItemModel
{
    public long CollectionId { get; set; }
    public string CollectionName { get; set; } = "";

    public string CollectionCensoredName { get; set; } = "";
    public string TrackCensoredName { get; set; } = "";
    public long CollectionArtistId { get; set; }
    public string CollectionArtistViewUrl { get; set; } = "";
    public string CollectionViewUrl { get; set; } = "";

    public decimal CollectionPrice { get; set; }

    public decimal CollectionHdPrice { get; set; }

    public string CollectionExplicitness { get; set; } = "";

    public string LongDescription { get; set; } = "";

    public IEnumerable<MoviePriceModel> TrackingLogs { get; set; } = Enumerable.Empty<MoviePriceModel>();

    public IEnumerable<CollectionListItemModel> Collections { get; set; } = Enumerable.Empty<CollectionListItemModel>();
}
