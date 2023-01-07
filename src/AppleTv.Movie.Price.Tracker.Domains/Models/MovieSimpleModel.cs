namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class MovieSimpleModel
{
    public Guid Id { get; set; }
    public string WrapperType { get; set; } = "";
    public string Kind { get; set; } = "";
    public long TrackId { get; set; }

    public string TrackName { get; set; } = "";

    public string CountryCode { get; set; } = "";
    public string LanguageCode { get; set; } = "";

    public string ArtworkUrl100 { get; set; } = "";

    public decimal TrackPrice { get; set; }
    public decimal TrackRentalPrice { get; set; }
    public decimal TrackHdPrice { get; set; }
    public decimal TrackHdRentalPrice { get; set; }

    public DateTimeOffset? ReleaseDate { get; set; }
}