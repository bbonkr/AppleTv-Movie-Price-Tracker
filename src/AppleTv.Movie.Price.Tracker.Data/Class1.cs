using Microsoft.EntityFrameworkCore;

namespace AppleTv.Movie.Price.Tracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<AppleTv.Movie.Price.Tracker.Entities.Movie> Movies { get; set; }

    public DbSet<AppleTv.Movie.Price.Tracker.Entities.MoviePrice> MoviePrices { get; set; }
}
