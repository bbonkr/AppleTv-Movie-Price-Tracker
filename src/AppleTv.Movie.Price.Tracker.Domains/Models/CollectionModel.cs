namespace AppleTv.Movie.Price.Tracker.Domains.Models;

public class CollectionModel : CollectionListItemModel
{
    public IEnumerable<MovieListItemModel> Movies { get; set; } = Enumerable.Empty<MovieListItemModel>();
}
