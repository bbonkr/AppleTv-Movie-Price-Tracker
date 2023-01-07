using AppleTv.Movie.Price.Tracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppleTv.Movie.Price.Tracker.Data.EntityTypeConfigurations;

public class CollectionMovieTypeConfiguration : IEntityTypeConfiguration<Entities.CollectionMovie>
{
    public void Configure(EntityTypeBuilder<CollectionMovie> builder)
    {
        builder.HasKey(x => new { x.CollectionId, x.MovieId });

        builder.Property(x => x.CollectionId)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.MovieId)
            .IsRequired()
            .HasConversion<string>();

        // builder.HasOne(x => x.Collection)
        // .WithMany(x => x.CollectionMovies)
        // .HasForeignKey(x => x.CollectionId);

        // builder.HasOne(x => x.Movie)
        // .WithMany(x => x.CollectionMovies)
        // .HasForeignKey(x => x.MovieId);
    }
}