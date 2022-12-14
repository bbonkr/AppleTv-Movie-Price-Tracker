using System;
using AppleTv.Movie.Price.Tracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppleTv.Movie.Price.Tracker.Data.EntityTypeConfigurations;

public class MovieEntityTypeConfiguration : IEntityTypeConfiguration<AppleTv.Movie.Price.Tracker.Entities.Movie>
{
    public void Configure(EntityTypeBuilder<Entities.Movie> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion<string>();
    }
}

public class MoviePriceTypeConfiguration : IEntityTypeConfiguration<Entities.MoviePrice>
{
    public void Configure(EntityTypeBuilder<MoviePrice> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.TrackingLogs)
            .HasForeignKey(x => x.MovieId);
    }
}