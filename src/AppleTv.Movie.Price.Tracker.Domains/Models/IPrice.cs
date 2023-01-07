namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public interface IPrice
{
    decimal CollectionPrice { get; }
    decimal CollectionHdPrice { get; }

    decimal TrackPrice { get; }
    decimal TrackRentalPrice { get; }

    decimal TrackHdPrice { get; }
    decimal TrackHdRentalPrice { get; }
}
