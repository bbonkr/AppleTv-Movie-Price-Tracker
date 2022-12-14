using System;
using System.Text.Json;

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
            builder.Services
                .AddOptions<MoviePriceCollectJobOptions>()
                .Configure<IConfiguration>((opt, cfg) => {
                    cfg.GetSection(MoviePriceCollectJobOptions.Name).Bind(opt);
                });

            builder.Services.AddRequiredServices(ServiceLifetime.Singleton);
            builder.Services.AddAppDbContext(configuration, ServiceLifetime.Singleton, ServiceLifetime.Singleton);
            builder.Services.AddJsonOptions();
            builder.Services.AddMappingProfiles();
            builder.AddJob<MoviePriceCollectJob, MoviePriceCollectJobOptions>();

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

    public static IServiceCollection AddRequiredServices(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        services.Add(new ServiceDescriptor(typeof(ITunesSearchService), sp => sp.GetRequiredService<ITunesSearchService>(), serviceLifetime));
        
        services.AddHttpClient<ITunesSearchService>();

        return services;
    }
     
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.AddOptions<JsonSerializerOptions>().Configure(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }

    public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program).Assembly);

        return services;
    }

    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<AppDbContext>(builder =>
        {
            builder.UseSqlServer(connectionString, sqlServerOptionBuilder =>
            {
                sqlServerOptionBuilder.MigrationsAssembly(typeof(Placeholder).Assembly.GetName().FullName);
            });
        }, contextLifetime, optionsLifetime);

        return services;
    }
}
