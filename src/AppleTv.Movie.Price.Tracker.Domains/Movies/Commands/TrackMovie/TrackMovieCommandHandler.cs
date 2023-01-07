using System.Numerics;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.LookupMovie;
using AppleTv.Movie.Price.Tracker.Services.Models;
using AutoMapper;
using kr.bbon.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.TrackMovie;

public class TrackMovieCommandHandler : IRequestHandler<TrackMovieCommand, MovieModel>
{
    public TrackMovieCommandHandler(
        AppDbContext context,
        IMapper mapper,
        IMediator mediator,
        ILogger<TrackMovieCommandHandler> logger)
    {
        this.context = context;
        this.mapper = mapper;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task<MovieModel> Handle(TrackMovieCommand request, CancellationToken cancellationToken = default)
    {
        LookupMovieQuery query = new(request.Id, request.Country)
        {
            Language = request.Language
        };

        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
        {
            throw new ApiException(System.Net.HttpStatusCode.NotFound, "Movie not found");
        }

        var movie = await context.Movies
            .Include(x => x.CollectionMovies)
            .Include(x => x.TrackingLogs)
            .Where(x => x.TrackId == request.Id && x.CountryCode == request.Country && x.LanguageCode == request.Language)
            .OrderBy(x => x.Id)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        using var transaction = context.Database.BeginTransaction();

        try
        {
            Guid movieId;

            if (movie != null)
            {
                movieId = movie.Id;

                movie.ArtistName = result.ArtistName;
                movie.ArtworkUrl100 = result.ArtworkUrl100;
                movie.ArtworkUrl30 = result.ArtworkUrl30;
                movie.ArtworkUrl60 = result.ArtworkUrl60;
                movie.ArtworkUrlBase = result.ArtworkUrlBase;
                movie.CollectionPrice = result.CollectionPrice;
                movie.CollectionHdPrice = result.CollectionHdPrice;
                movie.TrackPrice = result.TrackPrice;
                movie.TrackHdPrice = result.TrackHdPrice;
                movie.TrackRentalPrice = result.TrackRentalPrice;
                movie.TrackHdRentalPrice = result.TrackHdRentalPrice;

                await context.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Movie updated: {title} ({trackId};{country})", movie.TrackName, movie.TrackId, movie.Country);
            }
            else
            {

                movie = mapper.Map<Entities.Movie>(result);

                var added = context.Movies.Add(movie);

                movieId = added.Entity.Id;

                await context.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Movie added: {title} ({trackId};{country})", movie.TrackName, movie.TrackId, movie.Country);
            }

            if (result.CollectionId > 0)
            {
                Guid collectionId;

                var collection = await context.Collections
                    .Where(x => x.CollectionId == result.CollectionId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (collection == null)
                {
                    collection = new()
                    {
                        CollectionArtistId = result.CollectionArtistId,
                        CollectionArtistViewUrl = result.CollectionArtistViewUrl,
                        CollectionCensoredName = result.CollectionCensoredName,
                        CollectionHdPrice = result.CollectionHdPrice,
                        CollectionId = result.CollectionId,
                        CollectionName = result.CollectionName,
                        CollectionPrice = result.CollectionPrice,
                        CollectionViewUrl = result.CollectionViewUrl,
                    };

                    var addedCollection = context.Collections.Add(collection);

                    collectionId = addedCollection.Entity.Id;
                }
                else
                {
                    collection.CollectionArtistId = result.CollectionArtistId;
                    collection.CollectionArtistViewUrl = result.CollectionArtistViewUrl;
                    collection.CollectionCensoredName = result.CollectionCensoredName;
                    collection.CollectionHdPrice = result.CollectionHdPrice;
                    collection.CollectionId = result.CollectionId;
                    collection.CollectionName = result.CollectionName;
                    collection.CollectionPrice = result.CollectionPrice;
                    collection.CollectionViewUrl = result.CollectionViewUrl;

                    collectionId = collection.Id;
                }

                await context.SaveChangesAsync(cancellationToken);

                var collectionMovie = await context.CollectionMovies
                    .Where(x => x.MovieId == movieId && x.CollectionId == collectionId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (collectionMovie == null)
                {
                    context.CollectionMovies.Add(new()
                    {
                        CollectionId = collectionId,
                        MovieId = movieId,
                    });

                    await context.SaveChangesAsync(cancellationToken);
                }
            }

            var price = await context.MoviePrices
                .Where(x => x.MovieId == movieId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (price == null)
            {
                context.MoviePrices.Add(new()
                {
                    MovieId = movieId,
                    CollectionHdPrice = movie.CollectionHdPrice,
                    CollectionPrice = movie.CollectionPrice,
                    Created = DateTimeOffset.UtcNow,
                    TrackHdPrice = movie.TrackHdPrice,
                    TrackHdRentalPrice = movie.TrackHdRentalPrice,
                    TrackPrice = movie.TrackPrice,
                    TrackRentalPrice = movie.TrackRentalPrice,
                });

                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                var local = ConvertPrice(price);
                var remote = ConvertPrice(result);

                var diff = DiffBetweenSourceAndRemote(local, remote);

                if (diff.HasChanged)
                {
                    movie.TrackingLogs.Add(new()
                    {
                        MovieId = movie.Id,
                        CollectionPrice = result.CollectionPrice,
                        CollectionHdPrice = result.CollectionHdPrice,
                        TrackPrice = result.TrackPrice,
                        TrackHdPrice = result.TrackHdPrice,
                        TrackRentalPrice = result.TrackRentalPrice,
                        TrackHdRentalPrice = result.TrackHdRentalPrice,
                    });

                    logger.LogInformation(@"[{className}][{methodName}] {MovieTitle}
CollectionPrice     : {CollectionPriceBefore} {CollectionPriceAfter} {CollectionPriceStatus}
CollectionHdPrice   : {CollectionHdPriceBefore} {CollectionHdPriceAfter} {CollectionHdPriceStatus}
TrackPrice          : {TrackPriceBefore} {TrackPriceAfter} {TrackPriceStatus}
TrackHdPrice        : {TrackHdPriceBefore} {TrackHdPriceAfter} {TrackHdPriceStatus}
TrackRentalPrice    : {TrackRentalPricBefore} {TrackRentalPricAfter} {TrackRentalPricStatus}
TrackHdRentalPrice  : {TrackHdRentalPriceBefore} {TrackHdRentalPriceAfter} {TrackHdRentalPriceStatus}
",
                        typeof(TrackMovieCommandHandler),
                        nameof(Handle),
                        movie.TrackName,
                        local.CollectionPrice, remote.CollectionPrice, DiffDescription(diff.CollectionPrice),
                        local.CollectionHdPrice, remote.CollectionHdPrice, DiffDescription(diff.CollectionHdPrice),
                        local.TrackPrice, remote.TrackPrice, DiffDescription(diff.TrackPrice),
                        local.TrackHdPrice, remote.TrackHdPrice, DiffDescription(diff.TrackHdPrice),
                        local.TrackRentalPrice, remote.TrackRentalPrice, DiffDescription(diff.TrackRentalPrice),
                        local.TrackHdRentalPrice, remote.TrackHdRentalPrice, DiffDescription(diff.TrackHdRentalPrice)
                        );

                    await context.SaveChangesAsync(cancellationToken);
                }
            }

            await transaction.CommitAsync(cancellationToken);

            return mapper.Map<MovieModel>(movie);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            logger.LogError(ex, "{exception}", ex.Message);

            throw;
        }
        finally
        {

        }
    }

    private MoviePriceDiff DiffBetweenSourceAndRemote(IPrice local, IPrice remote)
    {
        return new MoviePriceDiff
        {
            CollectionPrice = Diff(local.CollectionPrice, remote.CollectionHdPrice),
            CollectionHdPrice = Diff(local.CollectionHdPrice, remote.CollectionHdPrice),
            TrackPrice = Diff(local.TrackPrice, remote.TrackPrice),
            TrackRentalPrice = Diff(local.TrackRentalPrice, remote.TrackRentalPrice),
            TrackHdPrice = Diff(local.TrackHdPrice, remote.TrackHdPrice),
            TrackHdRentalPrice = Diff(local.TrackHdRentalPrice, remote.TrackHdRentalPrice),
        };
    }

    private int Diff<T>(T local, T remote) where T : INumber<T>
    {
        if (local == remote) { return 0; }

        return local < remote ? 1 : -1;
    }

    private string DiffDescription(int diff)
    {
        if (diff > 0) { return "📈"; }
        if (diff < 0) { return "📉"; }
        return "";
    }

    private IPrice ConvertPrice(Entities.Movie movie)
    {
        return new AppleTv.Movie.Price.Tracker.Domains.Models.Price
        {
            CollectionPrice = movie.CollectionPrice,
            CollectionHdPrice = movie.CollectionHdPrice,

            TrackPrice = movie.TrackPrice,
            TrackHdPrice = movie.TrackHdPrice,

            TrackRentalPrice = movie.TrackRentalPrice,
            TrackHdRentalPrice = movie.TrackHdRentalPrice,
        };
    }

    private IPrice ConvertPrice(Entities.MoviePrice movie)
    {
        return new AppleTv.Movie.Price.Tracker.Domains.Models.Price
        {
            CollectionPrice = movie.CollectionPrice,
            CollectionHdPrice = movie.CollectionHdPrice,

            TrackPrice = movie.TrackPrice,
            TrackHdPrice = movie.TrackHdPrice,

            TrackRentalPrice = movie.TrackRentalPrice,
            TrackHdRentalPrice = movie.TrackHdRentalPrice,
        };
    }

    private IPrice ConvertPrice(ITunesSearchResultItemModel item)
    {
        return new AppleTv.Movie.Price.Tracker.Domains.Models.Price
        {
            CollectionPrice = item.CollectionPrice,
            CollectionHdPrice = item.CollectionHdPrice,

            TrackPrice = item.TrackPrice,
            TrackHdPrice = item.TrackHdPrice,

            TrackRentalPrice = item.TrackRentalPrice,
            TrackHdRentalPrice = item.TrackHdRentalPrice,
        };
    }

    private readonly AppDbContext context;
    private readonly IMapper mapper;
    private readonly IMediator mediator;
    private readonly ILogger logger;
}