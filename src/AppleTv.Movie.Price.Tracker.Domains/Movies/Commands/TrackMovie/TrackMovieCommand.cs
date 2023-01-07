using AppleTv.Movie.Price.Tracker.Domains.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.TrackMovie;

public class TrackMovieCommand : IRequest<MovieModel>
{
    public long Id { get; set; }

    public string Country { get; set; } = "kr";

    public string Language { get; set; } = "ko_kr";
}
