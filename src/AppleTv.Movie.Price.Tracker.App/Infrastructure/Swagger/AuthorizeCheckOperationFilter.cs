using System;
using AppleTv.Movie.Price.Tracker.App.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppleTv.Movie.Price.Tracker.App.Infrastructure.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public AuthorizeCheckOperationFilter(IOptionsMonitor<IdentityServerOptions> identityServerOptionsAccessor)
    {
        identityServer4Options = identityServerOptionsAccessor.CurrentValue;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ?? false)
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecurityScheme {Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"}
                    }
                ] = identityServer4Options.Scopes.Select(x=>x.Name).ToArray(),
            }
        };

        }
    }

    private readonly IdentityServerOptions identityServer4Options;
}

