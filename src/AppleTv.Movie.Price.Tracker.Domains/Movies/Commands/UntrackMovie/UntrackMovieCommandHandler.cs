using AppleTv.Movie.Price.Tracker.Data;
using kr.bbon.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.UntrackMovie;

public class UntrackMovieCommandHandler : IRequestHandler<UntrackMovieCommand, bool>
{

    public UntrackMovieCommandHandler(
        AppDbContext context,
        ILogger<UntrackMovieCommandHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<bool> Handle(UntrackMovieCommand request, CancellationToken cancellationToken = default)
    {
        var movie = await context.Movies
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (movie == null)
        {
            throw new ApiException(System.Net.HttpStatusCode.NotFound, "Movie not found");
        }

        using var transaction = context.Database.BeginTransaction();

        try
        {
            var collectionMovies = context.CollectionMovies
                .Where(x => x.MovieId == movie.Id);

            if (collectionMovies.Any())
            {
                foreach (var item in collectionMovies)
                {
                    context.CollectionMovies.Remove(item);
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            var priceLogs = context.MoviePrices
                .Where(x => x.MovieId == movie.Id);

            if (priceLogs.Any())
            {
                foreach (var item in priceLogs)
                {
                    context.MoviePrices.Remove(item);
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            context.Movies.Remove(movie);

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            logger.LogError(ex, "{exception}", ex.Message);

            throw;
        }
    }

    private readonly AppDbContext context;

    private readonly ILogger logger;
}