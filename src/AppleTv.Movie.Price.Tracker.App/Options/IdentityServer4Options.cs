using System;
namespace AppleTv.Movie.Price.Tracker.App.Options
{
    public class IdentityServerOptions
    {
        public const string Name = "IdentityServer";

        public string Issuer { get; set; } = "";

        public string ApiName { get; set; } = "";

        public IEnumerable<Scope> Scopes { get; set; } = Enumerable.Empty<Scope>();

        public string ClientSecret { get; set; } = "";

        public SwaggerClient SwaggerClient { get; set; }
    }
}


public class Scope
{
    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";
}

public class SwaggerClient
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";
}