namespace AppleTv.Movie.Price.Tracker.Services.Models;

public class ITunesSearchResultItemModel
{
    public string WrapperType { get; set; } = "";
    public string Kind { get; set; } = "";
    public int CollectionId { get; set; }
    public int TrackId { get; set; }
    public string ArtistName { get; set; } = "";
    public string CollectionName { get; set; } = "";
    public string TrackName { get; set; } = "";
    public string CollectionCensoredName { get; set; } = "";
    public string TrackCensoredName { get; set; } = "";
    public int CollectionArtistId { get; set; }
    public string CollectionArtistViewUrl { get; set; } = "";
    public string CollectionViewUrl { get; set; } = "";
    public string TrackViewUrl { get; set; } = "";
    public string PreviewUrl { get; set; } = "";

    public string ArtworkUrl30 { get; set; } = "";
    public string ArtworkUrl60 { get; set; } = "";
    public string ArtworkUrl100 { get; set; } = "";
    public string ArtworkUrlBase
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ArtworkUrl100))
            {
                return string.Empty;
            }
            var tokens = ArtworkUrl100.Split('/');
            return string.Join("/", tokens.Take(tokens.Length - 1));
        }
    }

    public decimal CollectionPrice { get; set; }
    public decimal TrackPrice { get; set; }
    public decimal TrackRentalPrice { get; set; }
    public decimal CollectionHdPrice { get; set; }
    public decimal TrackHdPrice { get; set; }
    public decimal TrackHdRentalPrice { get; set; }
    /// <summary>
    /// UTC
    /// </summary>
    public DateTimeOffset? ReleaseDate { get; set; }
    public string CollectionExplicitness { get; set; } = "";
    public string TrackExplicitness { get; set; } = "";
    public int DiscCount { get; set; }
    public int DiscNumber { get; set; }
    public int TrackCount { get; set; }
    public int TrackNumber { get; set; }
    public int TrackTimeMillis { get; set; }
    public string Country { get; set; } = "";
    public string Currency { get; set; } = "";
    public string PrimaryGenreName { get; set; } = "";
    public string ContentAdvisoryRating { get; set; } = "";
    public string LongDescription { get; set; } = "";
    public bool HasITunesExtras { get; set; } = false;

    public string CountryCode { get; set; } = "";
    public string LanguageCode { get; set; } = "";
}

