using System;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using kr.bbon.Core.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovies;

public class GetMoviesQuery : IRequest<MoviesPagedModel?>
{
    public string Keyword { get; set; } = "";

    public int Page { get; set; } = 1;

    public int Limit { get; set; } = 10;
}
