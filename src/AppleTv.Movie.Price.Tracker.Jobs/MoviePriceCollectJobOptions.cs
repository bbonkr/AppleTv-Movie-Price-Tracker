using CronScheduler.Extensions.Scheduler;

namespace AppleTv.Movie.Price.Tracker.Jobs;

public class MoviePriceCollectJobOptions : SchedulerOptions
{
    public const string Name = nameof(MoviePriceCollectJob);
}