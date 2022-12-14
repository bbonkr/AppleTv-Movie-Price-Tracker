using AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

var apiVersion = new ApiVersion(1, 0);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(mvcOptions =>
{
    mvcOptions.Filters.Add<kr.bbon.AspNetCore.Filters.ApiExceptionHandlerFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioningAndSwaggerGen(apiVersion);

builder.Services.AddRequiredServices();
builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddMoviePriceCollectJbo(builder.Configuration);
builder.Services.AddJsonOptions();
builder.Services.AddMappingProfiles();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUIWithApiVersioning();
}

app.UseDatabaseMigration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//await app.RunStartupJobsAync();

app.Run();
