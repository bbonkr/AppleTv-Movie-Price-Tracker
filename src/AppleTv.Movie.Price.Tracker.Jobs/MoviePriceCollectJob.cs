using AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.TrackMovie;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovies;
using CronScheduler.Extensions.Scheduler;
using Microsoft.Extensions.Logging;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Jobs;

public class MoviePriceCollectJob : IScheduledJob
{
    public string Name => nameof(MoviePriceCollectJob);

    public MoviePriceCollectJob(IMediator mediator, ILogger<MoviePriceCollectJob> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            GetMoviesQuery? query = null;
            MoviesPagedModel? queryResult = null;
            int page = 1;
            int limit = 20;

            do
            {
                query = new()
                {
                    Page = page,
                    Limit = limit,
                };

                queryResult = await mediator.Send(query, cancellationToken);

                if (queryResult?.Items.Any() ?? false)
                {
                    foreach (var item in queryResult.Items)
                    {

                        TrackMovieCommand command = new()
                        {
                            Id = item.TrackId,
                            Country = item.CountryCode,
                            Language = item.LanguageCode,
                        };

                        var trackResult = await mediator.Send(command, cancellationToken);
                    }
                }

                page += 1;
            } while (queryResult?.HasNextPage ?? false);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message);
            throw;
        }
        finally
        {
            logger.LogInformation("[{className}][{methodName}] Done at {now:yyyy-MM-ddTHH:mm:sszzz}", nameof(MoviePriceCollectJob), nameof(ExecuteAsync), DateTimeOffset.UtcNow);
        }
    }

    private readonly IMediator mediator;
    private readonly ILogger logger;
}

