namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class MoviePriceDiff
{
    public int CollectionPrice { get; set; }
    public int CollectionHdPrice { get; set; }
    public int TrackPrice { get; set; }
    public int TrackRentalPrice { get; set; }
    public int TrackHdPrice { get; set; }
    public int TrackHdRentalPrice { get; set; }

    public bool HasChanged
    {
        get => CollectionPrice != 0 ||
            CollectionHdPrice != 0 ||
            TrackPrice != 0 ||
            TrackRentalPrice != 0 ||
            TrackHdPrice != 0 ||
            TrackHdRentalPrice != 0;
    }
}
