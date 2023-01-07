using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AutoMapper;
using kr.bbon.EntityFrameworkCore.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovies;

public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, MoviesPagedModel?>
{
    public GetMoviesQueryHandler(
        AppDbContext context,
        IMapper mapper,
        ILogger<GetMoviesQueryHandler> logger)
    {
        this.context = context;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<MoviesPagedModel?> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
    {
        var keyword = request.Keyword?.Trim() ?? string.Empty;

        var result = await context.Movies
            .Include(x => x.Collections)
            .WhereDependsOn(
                !string.IsNullOrWhiteSpace(keyword),
                x => x.TrackName.Contains(keyword) ||
                        x.TrackCensoredName.Contains(keyword) ||
                        x.ArtistName.Contains(keyword) ||
                        x.CollectionCensoredName.Contains(keyword) ||
                        x.CollectionName.Contains(keyword))
            .OrderByDescending(x => x.ReleaseDate)
                .ThenBy(x => x.TrackName)
            .Select(x => mapper.Map<MovieListItemModel>(x))
            .AsNoTracking()
            .ToPagedModelAsync<MovieListItemModel, MoviesPagedModel>(page: request.Page, request.Limit, cancellationToken);

        return result;
    }

    private readonly AppDbContext context;
    private readonly IMapper mapper;
    private readonly ILogger logger;
}