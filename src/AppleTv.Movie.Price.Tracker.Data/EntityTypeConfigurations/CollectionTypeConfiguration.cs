using AppleTv.Movie.Price.Tracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppleTv.Movie.Price.Tracker.Data.EntityTypeConfigurations;

public class CollectionTypeConfiguration : IEntityTypeConfiguration<Entities.Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
        .IsRequired()
        .HasConversion<string>()
        .ValueGeneratedOnAdd();


        builder.Property(x => x.CollectionPrice)
        .IsRequired()
        .HasColumnType("decimal(18,5)")
        .HasConversion<decimal>()
        .HasDefaultValue(0);

        builder.Property(x => x.CollectionHdPrice)
        .IsRequired()
        .HasColumnType("decimal(18,5)")
        .HasConversion<decimal>()
        .HasDefaultValue(0);


        builder.HasMany(x => x.Movies)
            .WithMany(x => x.Collections)
            .UsingEntity<CollectionMovie>(
                x => x.HasOne(x => x.Movie).WithMany(x => x.CollectionMovies).HasForeignKey(x => x.MovieId),
                x => x.HasOne(x => x.Collection).WithMany(x => x.CollectionMovies).HasForeignKey(x => x.CollectionId),
                x =>
                {
                    x.HasKey(collectionMovie => new { collectionMovie.MovieId, collectionMovie.CollectionId });
                }
            );
    }
}
