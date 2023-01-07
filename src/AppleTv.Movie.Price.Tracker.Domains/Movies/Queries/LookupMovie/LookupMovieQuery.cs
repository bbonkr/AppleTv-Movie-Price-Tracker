using AppleTv.Movie.Price.Tracker.Services.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.LookupMovie;

public class LookupMovieQuery : IRequest<ITunesSearchResultItemModel>
{
    public LookupMovieQuery(long trackId, string country = "kr")
    {
        TrackId = trackId;
        Country = country;
    }
    public long TrackId { get; private set; }
    public string Country { get; private set; }
    public string Language { get; set; } = "ko_kr";
}
