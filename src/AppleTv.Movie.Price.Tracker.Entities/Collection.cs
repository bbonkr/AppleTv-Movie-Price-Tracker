namespace AppleTv.Movie.Price.Tracker.Entities;

public class Collection
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

    public virtual ICollection<CollectionMovie> CollectionMovies { get; set; } = new HashSet<CollectionMovie>();

    public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
}
