using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.SearchMovies;

public class SearchMoviesQuery : IRequest<MovieSearchPagedModel>
{
    public string Term { get; set; } = string.Empty;

    public string Country { get; set; } = "kr";

    public string Language { get; set; } = "ko_kr";
}
