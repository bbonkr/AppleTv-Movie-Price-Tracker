using AppleTv.Movie.Price.Tracker.App.Options;
using AppleTv.Movie.Price.Tracker.Data;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
    {
        using var appDbContext = app.ApplicationServices.GetRequiredService<AppDbContext>();

        appDbContext.Database.Migrate();

        return app;
    }

    public static IApplicationBuilder UseSwaggerUIWithIdentityServer(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var identityServerOptions = scope.ServiceProvider.GetRequiredService<IdentityServerOptions>();

        builder.UseSwaggerUIWithApiVersioning(options =>
        {
            if (identityServerOptions.SwaggerClient != null)
            {
                options.OAuthClientId(identityServerOptions.SwaggerClient.Id);
                options.OAuthAppName(identityServerOptions.SwaggerClient.Name);
            }

            options.OAuthScopeSeparator(" ");
            options.OAuthUsePkce();
        });

        return builder;
    }
}
