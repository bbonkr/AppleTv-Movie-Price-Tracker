using AppleTv.Movie.Price.Tracker.Domains.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovieById;

public class GetMovieByIdQuery : IRequest<MovieModel>
{
    public GetMovieByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
}
