namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class MovieListItemModel
{
    public Guid Id { get; set; }

    public string WrapperType { get; set; } = "";
    public string Kind { get; set; } = "";
    public long TrackId { get; set; }
    public string ArtistName { get; set; } = "";
    public string TrackName { get; set; } = "";
    public string TrackViewUrl { get; set; } = "";
    public string PreviewUrl { get; set; } = "";

    public string ArtworkUrl30 { get; set; } = "";
    public string ArtworkUrl60 { get; set; } = "";
    public string ArtworkUrl100 { get; set; } = "";
    public string ArtworkUrlBase { get; set; } = "";
    public decimal TrackPrice { get; set; }
    public decimal TrackRentalPrice { get; set; }
    public decimal TrackHdPrice { get; set; }
    public decimal TrackHdRentalPrice { get; set; }
    /// <summary>
    /// UTC
    /// </summary>
    public DateTimeOffset? ReleaseDate { get; set; }
    public string TrackExplicitness { get; set; } = "";
    public int DiscCount { get; set; }
    public int DiscNumber { get; set; }
    public int TrackCount { get; set; }
    public int TrackNumber { get; set; }
    public long TrackTimeMillis { get; set; }
    public string Country { get; set; } = "";
    public string Currency { get; set; } = "";
    public string PrimaryGenreName { get; set; } = "";
    public string ContentAdvisoryRating { get; set; } = "";
    public bool HasITunesExtras { get; set; } = false;
    public string CountryCode { get; set; } = "";
    public string LanguageCode { get; set; } = "";
}