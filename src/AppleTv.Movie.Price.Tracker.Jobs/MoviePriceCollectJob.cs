using System.Numerics;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Entities;
using AppleTv.Movie.Price.Tracker.Services;
using AppleTv.Movie.Price.Tracker.Services.Models;
using CronScheduler.Extensions.Scheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Jobs;

public class MoviePriceCollectJob : IScheduledJob
{
    public string Name => nameof(MoviePriceCollectJob);

    public MoviePriceCollectJob(AppDbContext appDbContext, ITunesSearchService iTunesSearchService, ILogger<MoviePriceCollectJob> logger)
    {
        this.appDbContext = appDbContext;
        this.iTunesSearchService = iTunesSearchService;
        this.logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var movies = await appDbContext.Movies
            .Include(x => x.TrackingLogs.OrderByDescending(log => log.Created).Take(1))
            .ToListAsync(cancellationToken);

        var hasChanged = false;
        using var transaction = appDbContext.Database.BeginTransaction();
        try
        {


            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    var result = await iTunesSearchService.LookupAsync(movie.TrackId, movie.CountryCode, movie.LanguageCode, cancellationToken);

                    foreach (var item in result.results)
                    {
                        var local = movie.TrackingLogs.Any() ? ConvertPrice(movie.TrackingLogs.First()) : ConvertPrice(movie);
                        var remote = ConvertPrice(item);

                        var diff = DiffBetweenSourceAndRemote(local, remote);

                        if (diff.HasChanged)
                        {
                            // insert
                            hasChanged = true;

                            movie.TrackingLogs.Add(new MoviePrice
                            {
                                MovieId = movie.Id,
                                CollectionPrice = item.CollectionPrice,
                                CollectionHdPrice = item.CollectionHdPrice,
                                TrackPrice = item.TrackPrice,
                                TrackHdPrice = item.TrackHdPrice,
                                TrackRentalPrice = item.TrackRentalPrice,
                                TrackHdRentalPrice = item.TrackHdRentalPrice,
                            });

                            logger.LogInformation(@"[{className}][{methodName}] {MovieTitle}
CollectionPrice     : {CollectionPriceBefore} {CollectionPriceAfter} {CollectionPriceStatus}
CollectionHdPrice   : {CollectionHdPriceBefore} {CollectionHdPriceAfter} {CollectionHdPriceStatus}
TrackPrice          : {TrackPriceBefore} {TrackPriceAfter} {TrackPriceStatus}
TrackHdPrice        : {TrackHdPriceBefore} {TrackHdPriceAfter} {TrackHdPriceStatus}
TrackRentalPrice    : {TrackRentalPricBefore} {TrackRentalPricAfter} {TrackRentalPricStatus}
TrackHdRentalPrice  : {TrackHdRentalPriceBefore} {TrackHdRentalPriceAfter} {TrackHdRentalPriceStatus}
",
                                typeof(MoviePriceCollectJob),
                                nameof(ExecuteAsync),
                                movie.TrackName,
                                local.CollectionPrice, remote.CollectionPrice, DiffDescription(diff.CollectionPrice),
                                local.CollectionHdPrice, remote.CollectionHdPrice, DiffDescription(diff.CollectionHdPrice),
                                local.TrackPrice, remote.TrackPrice, DiffDescription(diff.TrackPrice),
                                local.TrackHdPrice, remote.TrackHdPrice, DiffDescription(diff.TrackHdPrice),
                                local.TrackRentalPrice, remote.TrackRentalPrice, DiffDescription(diff.TrackRentalPrice),
                                local.TrackHdRentalPrice, remote.TrackHdRentalPrice, DiffDescription(diff.TrackHdRentalPrice)
                                );

                            await appDbContext.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                if (hasChanged)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            logger.LogError(ex, ex.Message);
        }
        finally
        {
            logger.LogInformation("[{className}][{methodName}] Done at {now:yyyy-MM-ddTHH:mm:sszzz}", nameof(MoviePriceCollectJob), nameof(ExecuteAsync), DateTimeOffset.UtcNow);
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
        return new Price
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
        return new Price
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
        return new Price
        {
            CollectionPrice = item.CollectionPrice,
            CollectionHdPrice = item.CollectionHdPrice,

            TrackPrice = item.TrackPrice,
            TrackHdPrice = item.TrackHdPrice,

            TrackRentalPrice = item.TrackRentalPrice,
            TrackHdRentalPrice = item.TrackHdRentalPrice,
        };
    }

    private readonly AppDbContext appDbContext;
    private readonly ITunesSearchService iTunesSearchService;
    private readonly ILogger logger;
}

public class MoviePriceDiff
{
    public int CollectionPrice { get; set; }
    public int CollectionHdPrice { get; set; }
    public int TrackPrice { get; set; }
    public int TrackRentalPrice { get; set; }
    public int TrackHdPrice { get; set; }
    public int TrackHdRentalPrice { get; set; }

    public bool HasChanged
    {
        get => CollectionPrice != 0 ||
            CollectionHdPrice != 0 ||
            TrackPrice != 0 ||
            TrackRentalPrice != 0 ||
            TrackHdPrice != 0 ||
            TrackHdRentalPrice != 0;
    }
}

public interface IPrice
{
    decimal CollectionPrice { get; }
    decimal CollectionHdPrice { get; }

    decimal TrackPrice { get; }
    decimal TrackRentalPrice { get; }

    decimal TrackHdPrice { get; }
    decimal TrackHdRentalPrice { get; }
}

public class Price : IPrice
{
    public decimal CollectionPrice { get; set; }
    public decimal TrackPrice { get; set; }
    public decimal TrackRentalPrice { get; set; }
    public decimal CollectionHdPrice { get; set; }
    public decimal TrackHdPrice { get; set; }
    public decimal TrackHdRentalPrice { get; set; }
}
