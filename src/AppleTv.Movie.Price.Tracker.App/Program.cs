using AppleTv.Movie.Price.Tracker.App;
using AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;
using AppleTv.Movie.Price.Tracker.App.Infrastructure.Filters;
using AppleTv.Movie.Price.Tracker.App.Options;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using kr.bbon.Services.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Mvc;

ApiVersion apiVersion = new(1, 0);
IdentityServerOptions identityServerOptions = new();
CorsOptions corsOptions = new();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection(CorsOptions.Name).Bind(corsOptions);
builder.Configuration.GetSection(IdentityServerOptions.Name).Bind(identityServerOptions);

// Add services to the container.

builder.Services.ConfigureAppOptions();

builder.Services.AddControllers(mvcOptions =>
{
    mvcOptions.Filters.Add<ApiExceptionHandlerWithGitHubIssueFilter>();
})
    .ConfigureCustomApiBehaviorOptions()
    .ConfigureDefaultXmlOptions();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services
    .AddCorsPolocy(corsOptions)
    .AddRequiredServices()
    .AddAppDbContext(builder.Configuration)
    .AddMoviePriceCollectJbo(builder.Configuration)
    .AddRequiredOptions()
    .AddEndpointsApiExplorer()
    .AddIdentityServerAuthentication()
    .AddSwaggerGenWithIdentityServer(apiVersion, identityServerOptions)
    .AddMappingProfiles()
    .AddValidatorIntercepter()
    .AddMediatR(new System.Reflection.Assembly[] { typeof(AppleTv.Movie.Price.Tracker.Domains.Placeholder).Assembly })
    .AddAutoMapper(new System.Reflection.Assembly[] { typeof(AppleTv.Movie.Price.Tracker.Domains.Placeholder).Assembly })
    .AddGitHubService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwaggerUIWithIdentityServer()
    .UseDatabaseMigration();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(Constants.DEFAULT_CORS_POLICY);

app.MapControllers()
    .RequireAuthorization();

app.Run();