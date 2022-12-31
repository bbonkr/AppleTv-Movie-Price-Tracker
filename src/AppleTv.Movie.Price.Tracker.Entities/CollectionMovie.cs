

namespace AppleTv.Movie.Price.Tracker.Entities;

public class CollectionMovie
{
    public Guid CollectionId { get; set; }

    public Guid MovieId { get; set; }


    public virtual Collection Collection { get; set; }
    public virtual Movie Movie { get; set; }

}
