namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class Price : IPrice
{
    public decimal CollectionPrice { get; set; }
    public decimal TrackPrice { get; set; }
    public decimal TrackRentalPrice { get; set; }
    public decimal CollectionHdPrice { get; set; }
    public decimal TrackHdPrice { get; set; }
    public decimal TrackHdRentalPrice { get; set; }
}
