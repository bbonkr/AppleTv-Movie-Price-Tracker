using System;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using AppleTv.Movie.Price.Tracker.App.Infrastructure.Swagger;
using AppleTv.Movie.Price.Tracker.App.Infrastructure.Validations;
using AppleTv.Movie.Price.Tracker.App.Options;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Data.SqlServer;
using AppleTv.Movie.Price.Tracker.Jobs;
using AppleTv.Movie.Price.Tracker.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using kr.bbon.AspNetCore.Models;
using kr.bbon.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMoviePriceCollectJbo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScheduler((builder) =>
        {
            builder.Services
                .AddOptions<MoviePriceCollectJobOptions>()
                .Configure<IConfiguration>((opt, cfg) =>
                {
                    cfg.GetSection(MoviePriceCollectJobOptions.Name).Bind(opt);
                });

            builder.Services
                .AddRequiredServices(ServiceLifetime.Singleton)
                .AddAppDbContext(configuration, ServiceLifetime.Singleton, ServiceLifetime.Singleton)
                .AddJsonOptions()
                .AddMappingProfiles()
                .AddMappingProfiles()
                .AddValidatorIntercepter()
                .AddMediatR(new System.Reflection.Assembly[] { typeof(AppleTv.Movie.Price.Tracker.Domains.Placeholder).Assembly })
                .AddAutoMapper(new System.Reflection.Assembly[] { typeof(AppleTv.Movie.Price.Tracker.Domains.Placeholder).Assembly });

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


    public static IServiceCollection AddValidatorIntercepter(this IServiceCollection services)
    {
        services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddIdentityServerAuthentication(this IServiceCollection services)
    {
        var identityServer4Options = new IdentityServerOptions();
        // configuration.GetSection(IdentityServer4Options.Name).Bind(identityServer4Options);

        services.AddOptions<IdentityServerOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(IdentityServerOptions.Name).Bind(options);

                identityServer4Options = options;
            });

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";

            })
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = identityServer4Options.Issuer;
                options.Audience = identityServer4Options.ApiName;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier,
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        var error = new ErrorModel("You are not authorized", HttpStatusCode.Unauthorized.ToString());

                        var responseModel =
                            ApiResponseModelFactory.Create(StatusCodes.Status401Unauthorized, error.Message, error);
                        responseModel.Path = context.Request.Path;
                        // responseModel.Instance = Activity.Current.Id;
                        responseModel.Method = context.Request.Method;

                        await context.Response.WriteAsJsonAsync(responseModel);
                    }
                };
            });
        // .AddIdentityServerAuthentication("Bearer", options =>
        // {
        //     // required audience of access tokens
        //     options.ApiName = identityServer4Options.ApiName;
        //     // auth server base endpoint (this will be used to search for disco doc)
        //     options.Authority = identityServer4Options.Issuer;
        //     options.NameClaimType = ClaimTypes.NameIdentifier;
        //     options.RoleClaimType = ClaimTypes.Role;
        //     options.RequireHttpsMetadata = false;
        // });

        return services;
    }

    public static IServiceCollection AddSwaggerGenWithIdentityServer(this IServiceCollection services, ApiVersion defaultVersion, IdentityServerOptions identityServerOptions)
    {
        services.AddSingleton<IdentityServerOptions>(_ => identityServerOptions);

        services.AddApiVersioningAndSwaggerGen(defaultVersion, options =>
        {
            options.OperationFilter<AuthorizeCheckOperationFilter>();

            options.CustomOperationIds(desc => $"{desc.HttpMethod!.ToUpper()}:{desc.RelativePath}");

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{identityServerOptions.Issuer}/connect/authorize"),
                        TokenUrl = new Uri($"{identityServerOptions.Issuer}/connect/token"),
                        Scopes = identityServerOptions.Scopes.ToDictionary(x => x.Name, x => x.DisplayName),
                    }
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            if (File.Exists(xmlFilename))
            {
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            }
        });

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.AddValidatorsFromAssemblies(assemblies, serviceLifetime);

        return services;
    }

    public static IServiceCollection AddCorsPolocy(this IServiceCollection services, CorsOptions corsOptions)
    {
        services.AddSingleton<CorsOptions>(_ => corsOptions);

        var origins = corsOptions.GetOrigins();

        services.AddCors(options =>
        {
            options.AddPolicy(Constants.DEFAULT_CORS_POLICY, policy =>
            {
                policy.SetIsOriginAllowedToAllowWildcardSubdomains();
                if (corsOptions.AllowsAnyHeaders)
                {
                    policy.AllowAnyHeader();
                }
                else
                {
                    policy.WithHeaders(corsOptions.GetHeaders().ToArray());
                }

                if (corsOptions.AllowsAnyMethods)
                {
                    policy.AllowAnyMethod();
                }
                else
                {
                    policy.WithMethods(corsOptions.GetMethods().ToArray());
                }

                if (corsOptions.AllowsAnyOrigins)
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(corsOptions.GetOrigins().ToArray());
                }
            });
        });

        return services;
    }
}
