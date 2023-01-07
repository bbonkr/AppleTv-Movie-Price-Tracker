namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class CollectionListItemModel
{
    public Guid Id { get; set; }

    public long CollectionId { get; set; }

    public string CollectionName { get; set; } = "";

    public string CollectionCensoredName { get; set; } = "";

    public long CollectionArtistId { get; set; }
    public string CollectionArtistViewUrl { get; set; } = "";
    public string CollectionViewUrl { get; set; } = "";

    public decimal CollectionPrice { get; set; }

    public decimal CollectionHdPrice { get; set; }
}