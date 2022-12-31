using AppleTv.Movie.Price.Tracker.Domains.Collections.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Collections.Queries.GetCollections;

public class GetCollectionsQuery : IRequest<CollectionsPagedModel>
{
    public int page { get; set; } = 1;
    public int limit { get; set; } = 10;
    public string keyword { get; set; } = string.Empty;
}
