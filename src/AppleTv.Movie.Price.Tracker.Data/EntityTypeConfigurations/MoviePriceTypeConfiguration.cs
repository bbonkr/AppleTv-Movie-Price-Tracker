using AppleTv.Movie.Price.Tracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppleTv.Movie.Price.Tracker.Data.EntityTypeConfigurations;

public class MoviePriceTypeConfiguration : IEntityTypeConfiguration<Entities.MoviePrice>
{
    public void Configure(EntityTypeBuilder<MoviePrice> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion<string>()
            .ValueGeneratedOnAdd();

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.TrackingLogs)
            .HasForeignKey(x => x.MovieId);

        builder.Property(x => x.TrackPrice)
        .IsRequired()
        .HasColumnType("decimal(18,5)")
        .HasConversion<decimal>()
        .HasDefaultValue(0);

        builder.Property(x => x.TrackHdPrice)
        .IsRequired()
        .HasColumnType("decimal(18,5)")
        .HasConversion<decimal>()
        .HasDefaultValue(0);

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

        builder.Property(x => x.TrackRentalPrice)
        .IsRequired()
        .HasColumnType("decimal(18,5)")
        .HasConversion<decimal>()
        .HasDefaultValue(0);

        builder.Property(x => x.TrackHdRentalPrice)
        .IsRequired()
        .HasColumnType("decimal(18,5)")
        .HasConversion<decimal>()
        .HasDefaultValue(0);

    }
}
