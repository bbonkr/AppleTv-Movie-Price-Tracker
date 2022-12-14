namespace AppleTv.Movie.Price.Tracker.Entities;

public class MoviePrice
{
    public Guid Id { get; set; }

    public Guid MovieId { get; set; }

    public decimal CollectionPrice { get; set; }
    public decimal CollectionHdPrice { get; set; }

    public decimal TrackPrice { get; set; }
    public decimal TrackRentalPrice { get; set; }

    public decimal TrackHdPrice { get; set; }
    public decimal TrackHdRentalPrice { get; set; }

    /// <summary>
    /// UTC
    /// </summary>
    public DateTimeOffset Created { get; set; }

    public virtual Movie? Movie { get; set; }
}