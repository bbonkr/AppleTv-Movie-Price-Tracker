using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AppleTv.Movie.Price.Tracker.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.SearchMovies;

public class SearchMoviesQueryHandler : IRequestHandler<SearchMoviesQuery, MovieSearchPagedModel>
{
    public SearchMoviesQueryHandler(ITunesSearchService iTunesSearchService, ILogger<SearchMoviesQueryHandler> logger)
    {
        this.iTunesSearchService = iTunesSearchService;
        this.logger = logger;
    }

    public async Task<MovieSearchPagedModel> Handle(SearchMoviesQuery request, CancellationToken cancellationToken = default)
    {
        var result = await iTunesSearchService.SearchMoviesAsync(
            request.Term, request.Country, request.Language,
            cancellationToken: cancellationToken);

        MovieSearchPagedModel model = new();
        model.SetInformation(1, ITunesSearchService.LIMIT, (ulong)result.ResultCount, 1);
        model.SetItems(result.results);

        return model;
    }

    private readonly ITunesSearchService iTunesSearchService;

    private readonly ILogger logger;
}