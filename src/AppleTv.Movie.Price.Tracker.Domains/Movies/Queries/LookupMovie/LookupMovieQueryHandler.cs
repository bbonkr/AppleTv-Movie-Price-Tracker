using AppleTv.Movie.Price.Tracker.Services;
using AppleTv.Movie.Price.Tracker.Services.Models;
using AutoMapper;
using kr.bbon.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.LookupMovie;

public class LookupMovieQueryHandler : IRequestHandler<LookupMovieQuery, ITunesSearchResultItemModel>
{
    public LookupMovieQueryHandler(ITunesSearchService iTunesSearchService, IMapper mapper, ILogger<LookupMovieQueryHandler> logger)
    {
        this.iTunesSearchService = iTunesSearchService;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<ITunesSearchResultItemModel> Handle(LookupMovieQuery request, CancellationToken cancellationToken = default)
    {
        var result = await iTunesSearchService.LookupMovieAsync(request.TrackId, request.Country, request.Language, cancellationToken);

        if (result == null)
        {
            throw new ApiException(System.Net.HttpStatusCode.NotFound, "Movie not found");
        }

        return result;
    }

    private readonly ITunesSearchService iTunesSearchService;

    private readonly IMapper mapper;
    private readonly ILogger logger;
}