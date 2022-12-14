using System;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Data.SqlServer;
using AppleTv.Movie.Price.Tracker.Jobs;
using AppleTv.Movie.Price.Tracker.Services;
using Microsoft.EntityFrameworkCore;

namespace AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMoviePriceCollectJbo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScheduler((builder) =>
        {
            builder.Services.AddRequiredServices(configuration);

            builder.AddJob<MoviePriceCollectJob>(nameof(MoviePriceCollectJob), nameof(MoviePriceCollectJob));

            // register a custom error processing for internal errors
            builder.AddUnobservedTaskExceptionHandler(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("CronJobs");

                return (sender, args) =>
                    {
                        logger?.LogError(args.Exception?.Message);
                        args.SetObserved();
                    };
            });

        });

        return services;
    }

    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<AppDbContext>(builder =>
        {
            builder.UseSqlServer(connectionString, sqlServerOptionBuilder =>
            {
                sqlServerOptionBuilder.MigrationsAssembly(typeof(Placeholder).Assembly.GetName().FullName);
            });
        });

        services.AddTransient<ITunesSearchService>();
        services.AddHttpClient<ITunesSearchService>();

        return services;
    }
}
