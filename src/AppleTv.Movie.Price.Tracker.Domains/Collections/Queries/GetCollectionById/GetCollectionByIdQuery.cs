using AppleTv.Movie.Price.Tracker.Domains.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Collections.Queries.GetCollectionById;

public class GetCollectionByIdQuery : IRequest<CollectionModel>
{
    public GetCollectionByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; private set; }
}
