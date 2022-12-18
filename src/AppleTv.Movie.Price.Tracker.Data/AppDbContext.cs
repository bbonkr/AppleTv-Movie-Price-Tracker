using Microsoft.EntityFrameworkCore;

namespace AppleTv.Movie.Price.Tracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<AppleTv.Movie.Price.Tracker.Entities.Movie> Movies { get; set; }

    public DbSet<AppleTv.Movie.Price.Tracker.Entities.MoviePrice> MoviePrices { get; set; }

    public DbSet<AppleTv.Movie.Price.Tracker.Entities.Collection> Collections { get; set; }

    public DbSet<AppleTv.Movie.Price.Tracker.Entities.CollectionMovie> CollectionMovies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
