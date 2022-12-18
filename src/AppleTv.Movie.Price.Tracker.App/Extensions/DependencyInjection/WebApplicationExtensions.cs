using AppleTv.Movie.Price.Tracker.App.Options;
using AppleTv.Movie.Price.Tracker.Data;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        appDbContext.Database.Migrate();
        return app;
    }

    public static IApplicationBuilder UseSwaggerUIWithIdentityServer(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var identityServerOptions = scope.ServiceProvider.GetRequiredService<IdentityServerOptions>();

        builder.UseSwaggerUIWithApiVersioning(options =>
        {

            options.OAuthClientId(identityServerOptions.SwaggerClient.Id);
            options.OAuthAppName(identityServerOptions.SwaggerClient.Name);

            options.OAuthScopeSeparator(" ");
            options.OAuthUsePkce();
        });

        return builder;
    }
}
